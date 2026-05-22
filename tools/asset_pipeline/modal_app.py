from __future__ import annotations

import json
import os
import shlex
import subprocess
from pathlib import Path
from typing import Any

import modal


APP_NAME = "empire-card-asset-studio"
MODEL_CACHE_VOLUME = "empire-card-model-cache"
ASSET_VOLUME = "empire-card-assets"
REFERENCE_SECRET_NAME = "huggingface"

LOCAL_ROOT = Path(__file__).resolve().parent
MANIFEST_PATH = LOCAL_ROOT / "asset_manifest.json"
GLOBAL_STYLE_PATH = LOCAL_ROOT / "prompts" / "global_style.txt"
ASSET_REFERENCE_STYLE_PATH = LOCAL_ROOT / "prompts" / "asset_reference_style.txt"
NEGATIVE_PATH = LOCAL_ROOT / "prompts" / "negative.txt"

ASSET_ROOT = Path("/assets")
REFERENCE_ROOT = ASSET_ROOT / "references"
RAW_GLB_ROOT = ASSET_ROOT / "raw_glb"
PROCESSED_ROOT = ASSET_ROOT / "processed"
UNITY_ROOT = ASSET_ROOT / "unity"
LOG_ROOT = ASSET_ROOT / "logs"

DEFAULT_Z_IMAGE_MODEL = "Tongyi-MAI/Z-Image"


app = modal.App(APP_NAME)
model_cache = modal.Volume.from_name(MODEL_CACHE_VOLUME, create_if_missing=True)
asset_store = modal.Volume.from_name(ASSET_VOLUME, create_if_missing=True)
reference_secret = modal.Secret.from_name(REFERENCE_SECRET_NAME)


reference_image = (
    modal.Image.debian_slim(python_version="3.11")
    .apt_install("git", "libgl1", "libglib2.0-0")
    .pip_install(
        "accelerate",
        "git+https://github.com/huggingface/diffusers",
        "onnxruntime",
        "pillow",
        "pyyaml",
        "rembg",
        "safetensors",
        "sentencepiece",
        "torch",
        "transformers",
    )
)


trellis_image = (
    modal.Image.debian_slim(python_version="3.11")
    .apt_install("git", "ffmpeg", "libgl1", "libglib2.0-0")
    .pip_install("pyyaml")
    .run_commands(
        "git clone --depth 1 https://github.com/microsoft/TRELLIS.2 /opt/TRELLIS.2 || true",
        "if [ -f /opt/TRELLIS.2/requirements.txt ]; then pip install -r /opt/TRELLIS.2/requirements.txt; fi",
    )
)


postprocess_image = (
    modal.Image.debian_slim(python_version="3.11")
    .apt_install("blender", "ffmpeg")
    .pip_install("pyyaml")
)


def _load_manifest_file(path: Path) -> dict[str, Any]:
    with path.open("r", encoding="utf-8") as handle:
        return json.load(handle)


def _read_text(path: Path) -> str:
    return path.read_text(encoding="utf-8").strip()


def load_manifest() -> dict[str, Any]:
    return _load_manifest_file(MANIFEST_PATH)


def first_batch_ids() -> list[str]:
    manifest = load_manifest()
    return [asset["id"] for asset in manifest.get("first_batch", [])]


def find_asset(asset_id: str) -> dict[str, Any]:
    manifest = load_manifest()
    for asset in manifest.get("first_batch", []):
        if asset.get("id") == asset_id:
            return asset
    raise KeyError(f"Unknown asset id: {asset_id}")


def build_reference_prompt(asset: dict[str, Any]) -> tuple[str, str]:
    global_style = _read_text(ASSET_REFERENCE_STYLE_PATH)
    negative = _read_text(NEGATIVE_PATH)
    view = load_manifest().get("defaults", {}).get("reference_view", "orthographic three-quarter top-down view")
    prompt = (
        f"{global_style}\n\n"
        f"Create exactly one isolated Unity-ready 3D asset reference image for {asset['id']}.\n"
        f"{asset['prompt']}\n"
        f"The image must contain only this single asset centered in frame. "
        f"Use an alpha-ready pure flat background that can be removed completely, with no floor plane, "
        f"no studio set, no horizon line, no contact shadow, no cast shadow, no desk, no lamp, no cards, no extra props. "
        f"Do not include any readable text, fake letters, UI labels, logos, or decorative writing. "
        f"Use {view}. Keep the whole object uncropped with clean silhouette for image-to-3D reconstruction."
    )
    return prompt, negative


def ensure_asset_dirs(asset_id: str) -> dict[str, Path]:
    dirs = {
        "reference": REFERENCE_ROOT / asset_id,
        "raw": RAW_GLB_ROOT / asset_id,
        "processed": PROCESSED_ROOT / asset_id,
        "unity": UNITY_ROOT / asset_id,
        "logs": LOG_ROOT / asset_id,
    }
    for path in dirs.values():
        path.mkdir(parents=True, exist_ok=True)
    return dirs


def make_background_transparent(image_path: Path, output_path: Path) -> None:
    from PIL import Image

    try:
        from rembg import new_session, remove

        session = new_session("u2netp")
        image_bytes = image_path.read_bytes()
        cutout = remove(
            image_bytes,
            session=session,
            alpha_matting=True,
            alpha_matting_foreground_threshold=240,
            alpha_matting_background_threshold=20,
            alpha_matting_erode_size=8,
        )
        output_path.parent.mkdir(parents=True, exist_ok=True)
        output_path.write_bytes(cutout)
        return
    except Exception:
        pass

    image = Image.open(image_path).convert("RGBA")
    pixels = image.load()
    width, height = image.size

    corner_samples = [
        pixels[0, 0],
        pixels[width - 1, 0],
        pixels[0, height - 1],
        pixels[width - 1, height - 1],
    ]
    bg = tuple(sum(sample[i] for sample in corner_samples) / len(corner_samples) for i in range(3))

    hard_threshold = 34.0
    soft_threshold = 86.0
    for y in range(height):
        for x in range(width):
            r, g, b, a = pixels[x, y]
            distance = ((r - bg[0]) ** 2 + (g - bg[1]) ** 2 + (b - bg[2]) ** 2) ** 0.5
            if distance <= hard_threshold:
                pixels[x, y] = (r, g, b, 0)
            elif distance < soft_threshold:
                alpha = int(255 * ((distance - hard_threshold) / (soft_threshold - hard_threshold)))
                pixels[x, y] = (r, g, b, min(a, alpha))

    output_path.parent.mkdir(parents=True, exist_ok=True)
    image.save(output_path)


@app.cls(
    image=reference_image,
    gpu="A10G",
    volumes={"/cache": model_cache, "/assets": asset_store},
    secrets=[reference_secret],
    timeout=60 * 45,
)
class ReferenceGenerator:
    @modal.enter()
    def load(self) -> None:
        import torch
        from diffusers import ZImagePipeline

        model_id = os.environ.get("Z_IMAGE_MODEL_ID", DEFAULT_Z_IMAGE_MODEL)
        token = os.environ.get("HF_TOKEN")
        dtype = torch.bfloat16 if torch.cuda.is_available() else torch.float32

        self.pipe = ZImagePipeline.from_pretrained(
            model_id,
            torch_dtype=dtype,
            token=token,
            trust_remote_code=True,
            low_cpu_mem_usage=False,
            cache_dir="/cache/models/z-image",
        )
        if torch.cuda.is_available():
            self.pipe = self.pipe.to("cuda")

    @modal.method()
    def generate(
        self,
        asset_id: str,
        prompt: str,
        negative: str,
        width: int = 1024,
        height: int = 1024,
        steps: int = 40,
        seed: int = 1201,
    ) -> dict[str, str]:
        import torch

        dirs = ensure_asset_dirs(asset_id)

        generator = torch.Generator(device="cuda" if torch.cuda.is_available() else "cpu").manual_seed(seed)

        result = self.pipe(
            prompt=prompt,
            negative_prompt=negative,
            width=width,
            height=height,
            num_inference_steps=steps,
            guidance_scale=float(os.environ.get("Z_IMAGE_GUIDANCE_SCALE", "4")),
            cfg_normalization=False,
            generator=generator,
        )

        raw_output_path = dirs["reference"] / "raw_reference.png"
        output_path = dirs["reference"] / "reference.png"
        result.images[0].save(raw_output_path)
        make_background_transparent(raw_output_path, output_path)

        config = {
            "asset_id": asset_id,
            "seed": seed,
            "steps": steps,
            "width": width,
            "height": height,
            "model": os.environ.get("Z_IMAGE_MODEL_ID", DEFAULT_Z_IMAGE_MODEL),
            "prompt": prompt,
            "negative_prompt": negative,
            "raw_reference_path": str(raw_output_path),
            "transparent_reference_path": str(output_path),
        }
        (dirs["reference"] / "generation_config.json").write_text(json.dumps(config, indent=2), encoding="utf-8")
        asset_store.commit()

        return {
            "asset_id": asset_id,
            "reference_path": str(output_path),
            "config_path": str(dirs["reference"] / "generation_config.json"),
        }


@app.cls(
    image=trellis_image,
    gpu="A100-40GB",
    volumes={"/cache": model_cache, "/assets": asset_store},
    secrets=[reference_secret],
    timeout=60 * 90,
)
class ThreeDGenerator:
    @modal.method()
    def generate_from_reference(self, asset_id: str) -> dict[str, str]:
        dirs = ensure_asset_dirs(asset_id)
        reference_path = dirs["reference"] / "reference.png"
        output_path = dirs["raw"] / "raw.glb"
        log_path = dirs["logs"] / "trellis.log"

        if not reference_path.exists():
            raise FileNotFoundError(f"Missing reference image for {asset_id}: {reference_path}")

        command_template = os.environ.get("TRELLIS_COMMAND_TEMPLATE")
        if not command_template:
            raise RuntimeError(
                "TRELLIS_COMMAND_TEMPLATE is not set. Configure it in the Modal secret after validating "
                "the installed TRELLIS.2 CLI entrypoint. Required placeholders: {input} and {output}."
            )

        command = command_template.format(
            input=shlex.quote(str(reference_path)),
            output=shlex.quote(str(output_path)),
            asset_id=shlex.quote(asset_id),
        )
        completed = subprocess.run(
            command,
            shell=True,
            cwd="/opt/TRELLIS.2",
            text=True,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            check=False,
        )
        log_path.write_text(completed.stdout, encoding="utf-8")

        if completed.returncode != 0:
            asset_store.commit()
            raise RuntimeError(f"TRELLIS.2 failed for {asset_id}. See {log_path}")
        if not output_path.exists():
            asset_store.commit()
            raise FileNotFoundError(f"TRELLIS.2 did not create {output_path}")

        asset_store.commit()
        return {"asset_id": asset_id, "raw_glb_path": str(output_path), "log_path": str(log_path)}


@app.function(
    image=postprocess_image,
    volumes={"/assets": asset_store},
    timeout=60 * 30,
)
def postprocess_asset(asset_id: str) -> dict[str, str]:
    dirs = ensure_asset_dirs(asset_id)
    raw_glb = dirs["raw"] / "raw.glb"
    if not raw_glb.exists():
        raise FileNotFoundError(f"Missing raw GLB for {asset_id}: {raw_glb}")

    script_path = Path("/root/blender_cleanup.py")
    script_path.write_text(BLENDER_CLEANUP_SCRIPT, encoding="utf-8")

    output_base = dirs["processed"] / f"EOC_{asset_id}"
    command = [
        "blender",
        "--background",
        "--python",
        str(script_path),
        "--",
        "--input",
        str(raw_glb),
        "--output-base",
        str(output_base),
    ]
    completed = subprocess.run(command, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    log_path = dirs["logs"] / "postprocess.log"
    log_path.write_text(completed.stdout, encoding="utf-8")
    if completed.returncode != 0:
        asset_store.commit()
        raise RuntimeError(f"Postprocess failed for {asset_id}. See {log_path}")

    asset_store.commit()
    return {
        "asset_id": asset_id,
        "processed_root": str(dirs["processed"]),
        "log_path": str(log_path),
    }


@app.local_entrypoint()
def generate_reference(asset_id: str, seed: int = 1201, steps: int = 40) -> None:
    asset = find_asset(asset_id)
    prompt, negative = build_reference_prompt(asset)
    defaults = load_manifest().get("defaults", {})
    ref = ReferenceGenerator()
    print(f"Generating one isolated transparent reference: {asset_id}")
    print(
        ref.generate.remote(
            asset_id,
            prompt,
            negative,
            int(defaults.get("reference_width", 1024)),
            int(defaults.get("reference_height", 1024)),
            steps,
            seed,
        )
    )


@app.local_entrypoint()
def generate_3d_asset(asset_id: str) -> None:
    print(f"Generating one 3D asset from approved transparent reference: {asset_id}")
    three_d = ThreeDGenerator()
    print(three_d.generate_from_reference.remote(asset_id))
    print(postprocess_asset.remote(asset_id))


@app.local_entrypoint()
def generate_first_batch(
    references_only: bool = True,
    start: int = 0,
    count: int = 9,
    allow_batch: bool = False,
) -> None:
    if not allow_batch:
        raise RuntimeError(
            "Batch generation is disabled for production assets. Use generate_reference --asset-id <id>, "
            "download the result, inspect the transparent PNG, then continue with the next asset."
        )

    manifest = load_manifest()
    defaults = manifest.get("defaults", {})
    batch = manifest.get("first_batch", [])[start : start + count]
    ref = ReferenceGenerator()
    three_d = ThreeDGenerator()

    for asset in batch:
        asset_id = asset["id"]
        prompt, negative = build_reference_prompt(asset)
        print(f"Generating reference: {asset_id}")
        print(
            ref.generate.remote(
                asset_id,
                prompt,
                negative,
                int(defaults.get("reference_width", 1024)),
                int(defaults.get("reference_height", 1024)),
                int(defaults.get("reference_steps", 40)),
            )
        )

        if references_only:
            continue

        print(f"Generating 3D: {asset_id}")
        print(three_d.generate_from_reference.remote(asset_id))
        print(postprocess_asset.remote(asset_id))


BLENDER_CLEANUP_SCRIPT = r'''
import argparse
import math
from pathlib import Path

import bpy
from mathutils import Vector


def parse_args():
    parser = argparse.ArgumentParser()
    parser.add_argument("--input", required=True)
    parser.add_argument("--output-base", required=True)
    return parser.parse_args()


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()


def import_model(path):
    ext = Path(path).suffix.lower()
    if ext in {".glb", ".gltf"}:
        bpy.ops.import_scene.gltf(filepath=path)
    elif ext == ".fbx":
        bpy.ops.import_scene.fbx(filepath=path)
    else:
        raise ValueError(f"Unsupported model extension: {ext}")


def normalize_scene():
    meshes = [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]
    if not meshes:
        raise RuntimeError("No mesh objects imported")

    bpy.ops.object.select_all(action="DESELECT")
    for obj in meshes:
        obj.select_set(True)
    bpy.context.view_layer.objects.active = meshes[0]

    bpy.ops.object.origin_set(type="ORIGIN_GEOMETRY", center="BOUNDS")

    min_corner = [math.inf, math.inf, math.inf]
    max_corner = [-math.inf, -math.inf, -math.inf]
    for obj in meshes:
        for corner in obj.bound_box:
            world = obj.matrix_world @ Vector(corner)
            for i in range(3):
                min_corner[i] = min(min_corner[i], world[i])
                max_corner[i] = max(max_corner[i], world[i])

    # Use Blender operator transforms for robust normalization across imported hierarchies.
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)


def add_lods():
    meshes = [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]
    for obj in meshes:
        obj.name = obj.name.replace(" ", "_")


def export_fbx(output_base):
    output = f"{output_base}_LOD0.fbx"
    bpy.ops.export_scene.fbx(
        filepath=output,
        use_selection=False,
        add_leaf_bones=False,
        bake_anim=False,
        object_types={"MESH"},
    )
    return output


def main():
    args = parse_args()
    clear_scene()
    import_model(args.input)
    normalize_scene()
    add_lods()
    export_fbx(args.output_base)


if __name__ == "__main__":
    main()
'''
