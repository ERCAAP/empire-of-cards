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
    modal.Image.from_registry("pytorch/pytorch:2.6.0-cuda12.4-cudnn9-devel")
    .env({"TORCH_CUDA_ARCH_LIST": "8.0;8.6;9.0"})
    .apt_install("git", "ffmpeg", "libgl1", "libglib2.0-0", "libjpeg-dev")
    .pip_install(
        "imageio", "imageio-ffmpeg", "tqdm", "easydict", "opencv-python-headless",
        "ninja", "trimesh", "transformers", "gradio==6.0.1", "tensorboard", "pandas",
        "lpips", "zstandard", "kornia", "timm", "rembg", "scipy", "einops"
    )
    .pip_install("flash-attn==2.7.3", extra_options="--no-build-isolation")
    .run_commands(
        "pip install git+https://github.com/EasternJournalist/utils3d.git@9a4eb15e4021b67b12c460c7057d642626897ec8"
    )
    .run_commands(
        "git clone --recursive --depth 1 https://github.com/microsoft/TRELLIS.2 /opt/TRELLIS.2 || true",
        "cd /opt/TRELLIS.2 && git submodule update --init --recursive",
    )
    .run_commands(
        "git clone -b v0.4.0 https://github.com/NVlabs/nvdiffrast.git /tmp/nvdiffrast && pip install /tmp/nvdiffrast --no-build-isolation",
        "git clone -b renderutils https://github.com/JeffreyXiang/nvdiffrec.git /tmp/nvdiffrec && pip install /tmp/nvdiffrec --no-build-isolation",
        "git clone --recursive https://github.com/JeffreyXiang/CuMesh.git /tmp/CuMesh && pip install /tmp/CuMesh --no-build-isolation",
        "git clone --recursive https://github.com/JeffreyXiang/FlexGEMM.git /tmp/FlexGEMM && pip install /tmp/FlexGEMM --no-build-isolation",
        "cd /opt/TRELLIS.2 && pip install ./o-voxel --no-build-isolation",
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


def manifest_defaults() -> dict[str, Any]:
    return load_manifest().get("defaults", {})


def manifest_assets() -> list[dict[str, Any]]:
    manifest = load_manifest()
    assets = manifest.get("assets")
    if assets:
        return assets
    return manifest.get("first_batch", [])


def manifest_groups() -> dict[str, list[str]]:
    return load_manifest().get("groups", {})


def first_batch_ids() -> list[str]:
    return [asset["id"] for asset in manifest_assets()]


def find_asset(asset_id: str) -> dict[str, Any]:
    for asset in manifest_assets():
        if asset.get("id") == asset_id:
            return asset
    raise KeyError(f"Unknown asset id: {asset_id}")


def resolve_asset_profile(asset: dict[str, Any]) -> dict[str, Any]:
    defaults = manifest_defaults()
    kind_profiles = defaults.get("kind_profiles", {})
    profile: dict[str, Any] = {}
    profile.update(kind_profiles.get(asset.get("kind", ""), {}))
    profile.update(asset)
    return profile


def build_reference_prompt(asset: dict[str, Any]) -> tuple[str, str]:
    profile = resolve_asset_profile(asset)
    defaults = manifest_defaults()
    global_style = _read_text(ASSET_REFERENCE_STYLE_PATH)
    negative = _read_text(NEGATIVE_PATH)
    view = profile.get("reference_view", defaults.get("reference_view", "orthographic three-quarter top-down view"))
    lighting = defaults.get(
        "reference_lighting",
        "warm upper-left key light with subtle cool fill from upper-right, evenly lit silhouette, no visible ground shadow",
    )
    material_style = defaults.get(
        "reference_material_style",
        "premium tabletop miniature finish, rich texture accents, minimalist forms, readable bevels",
    )
    prompt = (
        f"{global_style}\n\n"
        f"Create exactly one isolated Unity-ready 3D asset reference image for {asset['id']}.\n"
        f"{asset['prompt']}\n"
        f"Lighting direction: {lighting}. Material target: {material_style}. "
        f"The image must contain only this single asset centered in frame. "
        f"Use an alpha-ready pure flat background that can be removed completely, with no floor plane, "
        f"no studio set, no horizon line, no contact shadow, no cast shadow, no desk, no lamp, no cards, no extra props. "
        f"Do not include any readable text, fake letters, UI labels, logos, or decorative writing. "
        f"Use {view}. Keep the whole object uncropped with clean silhouette for image-to-3D reconstruction. "
        "Avoid floating fragments, thin antenna-like details, loose particles, smoke plumes, or accessories detached from the main mesh."
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


def unity_import_paths(asset: dict[str, Any], defaults: dict[str, Any] | None = None) -> dict[str, Path]:
    defaults = defaults or manifest_defaults()
    unity_import = defaults.get("unity_import", {})
    project_root = LOCAL_ROOT.parents[1]
    art_root = project_root / unity_import.get("art_root", "Assets/_EmpireOfCards/Art/Generated3D")
    material_root = project_root / unity_import.get("material_root", "Assets/_EmpireOfCards/Materials/Generated3D")
    art_path = art_root / asset.get("unity_art_subdir", asset["id"])
    material_path = material_root / asset.get("unity_material_subdir", asset["id"])
    return {
        "art_root": art_root,
        "material_root": material_root,
        "art_path": art_path,
        "material_path": material_path,
    }


def build_processed_metadata(asset: dict[str, Any], defaults: dict[str, Any] | None = None) -> dict[str, Any]:
    defaults = defaults or manifest_defaults()
    profile = resolve_asset_profile(asset)
    unity_paths = unity_import_paths(asset, defaults)
    return {
        "asset_id": asset["id"],
        "category": asset.get("category"),
        "kind": asset.get("kind"),
        "desktop_tri_budget": profile.get("desktop_tri_budget"),
        "low_tri_budget": profile.get("low_tri_budget"),
        "trellis_resolution": profile.get("trellis_resolution", defaults.get("trellis_resolution", 1024)),
        "texture_size": profile.get("texture_size", defaults.get("texture_sizes", {}).get("desktop", 2048)),
        "unity_target_max_dimension": profile.get("unity_target_max_dimension"),
        "lod_ratios": defaults.get("lod_ratios", {}),
        "art_destination": str(unity_paths["art_path"]),
        "material_destination": str(unity_paths["material_path"]),
        "quality_profile": {
            "resolution": "high",
            "efficiency": "desktop",
            "topology": "arbitrary",
            "texture": "rich",
            "style": "minimalist",
        },
    }


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
        asset: dict[str, Any],
        profile: dict[str, Any],
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
            "category": asset.get("category"),
            "kind": asset.get("kind"),
            "seed": seed,
            "steps": steps,
            "width": width,
            "height": height,
            "model": os.environ.get("Z_IMAGE_MODEL_ID", DEFAULT_Z_IMAGE_MODEL),
            "unity_target_max_dimension": profile.get("unity_target_max_dimension"),
            "texture_size": profile.get("texture_size"),
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
    @modal.enter()
    def load(self) -> None:
        import os
        import sys
        if "/opt/TRELLIS.2" not in sys.path:
            sys.path.append("/opt/TRELLIS.2")

        import torch
        os.environ['OPENCV_IO_ENABLE_OPENEXR'] = '1'
        os.environ["PYTORCH_CUDA_ALLOC_CONF"] = "expandable_segments:True"

        from trellis2.pipelines import Trellis2ImageTo3DPipeline
        from trellis2.renderers import EnvMap
        import cv2

        token = os.environ.get("HF_TOKEN")
        if token:
            from huggingface_hub import login
            try:
                login(token=token)
            except Exception as e:
                print(f"Warning: HF login failed: {e}")
        
        os.environ["HF_HUB_CACHE"] = "/cache/models/trellis"
        os.environ["HF_HOME"] = "/cache/models/trellis"
        self.pipeline = Trellis2ImageTo3DPipeline.from_pretrained(
            "microsoft/TRELLIS.2-4B"
        )
        self.pipeline.cuda()

        # Load environment map for rendering snapshots if available
        hdri_path = "/opt/TRELLIS.2/assets/hdri/forest.exr"
        if os.path.exists(hdri_path):
            try:
                self.envmap = EnvMap(torch.tensor(
                    cv2.cvtColor(cv2.imread(hdri_path, cv2.IMREAD_UNCHANGED), cv2.COLOR_BGR2RGB),
                    dtype=torch.float32, device='cuda'
                ))
            except Exception as e:
                print(f"Warning: Failed to load EnvMap: {e}")
                self.envmap = None
        else:
            self.envmap = None

    @modal.method()
    def generate_from_reference(self, asset_profile: dict[str, Any], defaults: dict[str, Any]) -> dict[str, str]:
        import os
        import sys
        if "/opt/TRELLIS.2" not in sys.path:
            sys.path.append("/opt/TRELLIS.2")

        from PIL import Image
        import torch
        import o_voxel

        asset_id = asset_profile["id"]
        dirs = ensure_asset_dirs(asset_id)
        reference_path = dirs["reference"] / "reference.png"
        output_path = dirs["raw"] / "raw.glb"

        if not reference_path.exists():
            raise FileNotFoundError(f"Missing reference image for {asset_id}: {reference_path}")

        # Get decimation budget from manifest
        tri_budget = asset_profile.get("desktop_tri_budget", 1500)
        decimation_target = max(1000, int(tri_budget))

        print(f"Loading reference image from {reference_path}...")
        image = Image.open(reference_path)

        print("Preprocessing image...")
        image = self.pipeline.preprocess_image(image)

        print("Running TRELLIS.2 pipeline...")
        resolution = str(asset_profile.get("trellis_resolution", defaults.get("trellis_resolution", "1024")))
        
        outputs, latents = self.pipeline.run(
            image,
            preprocess_image=False,
            pipeline_type={
                "512": "512",
                "1024": "1024_cascade",
                "1536": "1536_cascade",
            }.get(resolution, "1024_cascade"),
            return_latent=True,
        )

        print("Decoding latents to mesh...")
        shape_slat, tex_slat, res = latents
        mesh = self.pipeline.decode_latent(shape_slat, tex_slat, res)[0]
        mesh.simplify(16777216) # limit for nvdiffrast

        print(f"Exporting PBR textured GLB (Target triangles: {decimation_target})...")
        texture_size = int(asset_profile.get("texture_size", defaults.get("texture_sizes", {}).get("desktop", 2048)))
        
        glb = o_voxel.postprocess.to_glb(
            vertices            =   mesh.vertices,
            faces               =   mesh.faces,
            attr_volume         =   mesh.attrs,
            coords              =   mesh.coords,
            attr_layout         =   self.pipeline.pbr_attr_layout,
            grid_size           =   res,
            aabb                =   [[-0.5, -0.5, -0.5], [0.5, 0.5, 0.5]],
            decimation_target   =   decimation_target,
            texture_size        =   texture_size,
            remesh              =   bool(asset_profile.get("remesh", defaults.get("remesh", True))),
            remesh_band         =   int(asset_profile.get("remesh_band", defaults.get("remesh_band", 1))),
            remesh_project      =   int(asset_profile.get("remesh_project", defaults.get("remesh_project", 0))),
        )

        output_path.parent.mkdir(parents=True, exist_ok=True)
        glb.export(output_path, extension_webp=True)
        (dirs["raw"] / "generation_config.json").write_text(
            json.dumps(
                {
                    "asset_id": asset_id,
                    "trellis_resolution": resolution,
                    "texture_size": texture_size,
                    "decimation_target": decimation_target,
                    "remesh": bool(asset_profile.get("remesh", defaults.get("remesh", True))),
                    "remesh_band": int(asset_profile.get("remesh_band", defaults.get("remesh_band", 1))),
                    "remesh_project": int(asset_profile.get("remesh_project", defaults.get("remesh_project", 0))),
                    "raw_glb_path": str(output_path),
                },
                indent=2,
            ),
            encoding="utf-8",
        )

        torch.cuda.empty_cache()
        asset_store.commit()

        return {
            "asset_id": asset_id,
            "raw_glb_path": str(output_path),
        }


@app.function(
    image=postprocess_image,
    volumes={"/assets": asset_store},
    timeout=60 * 30,
)
def postprocess_asset(asset_profile: dict[str, Any], defaults: dict[str, Any], processed_metadata: dict[str, Any]) -> dict[str, str]:
    asset_id = asset_profile["id"]
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
        "--lod1-ratio",
        str(defaults.get("lod_ratios", {}).get("lod1", 0.5)),
        "--lod2-ratio",
        str(defaults.get("lod_ratios", {}).get("lod2", 0.22)),
        "--target-max-dimension",
        str(asset_profile.get("unity_target_max_dimension", 0.82)),
    ]
    completed = subprocess.run(command, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    log_path = dirs["logs"] / "postprocess.log"
    log_path.write_text(completed.stdout, encoding="utf-8")
    if completed.returncode != 0:
        asset_store.commit()
        raise RuntimeError(f"Postprocess failed for {asset_id}. See {log_path}")

    processed_metadata = dict(processed_metadata)
    processed_metadata["processed_root"] = str(dirs["processed"])
    processed_metadata["log_path"] = str(log_path)
    (dirs["processed"] / "import_manifest.json").write_text(json.dumps(processed_metadata, indent=2), encoding="utf-8")
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
    profile = resolve_asset_profile(asset)
    defaults = manifest_defaults()
    chosen_seed = int(profile.get("reference_seed", seed)) if seed == 1201 else seed
    chosen_steps = int(profile.get("reference_steps", defaults.get("reference_steps", steps))) if steps == 40 else steps
    ref = ReferenceGenerator()
    print(f"Generating one isolated transparent reference: {asset_id}")
    print(
        ref.generate.remote(
            asset_id,
            prompt,
            negative,
            asset,
            profile,
            int(profile.get("reference_width", defaults.get("reference_width", 1024))),
            int(profile.get("reference_height", defaults.get("reference_height", 1024))),
            chosen_steps,
            chosen_seed,
        )
    )


@app.local_entrypoint()
def generate_3d_asset(asset_id: str) -> None:
    asset = find_asset(asset_id)
    profile = resolve_asset_profile(asset)
    defaults = manifest_defaults()
    processed_metadata = build_processed_metadata(asset, defaults)
    print(f"Generating one 3D asset from approved transparent reference: {asset_id}")
    three_d = ThreeDGenerator()
    print(three_d.generate_from_reference.remote(profile, defaults))
    print(postprocess_asset.remote(profile, defaults, processed_metadata))


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

    defaults = manifest_defaults()
    batch = manifest_assets()[start : start + count]
    ref = ReferenceGenerator()
    three_d = ThreeDGenerator()

    for index, asset in enumerate(batch):
        asset_id = asset["id"]
        prompt, negative = build_reference_prompt(asset)
        print(f"Generating reference: {asset_id}")
        print(
            ref.generate.remote(
                asset_id,
                prompt,
                negative,
                asset,
                resolve_asset_profile(asset),
                int(asset.get("reference_width", defaults.get("reference_width", 1024))),
                int(asset.get("reference_height", defaults.get("reference_height", 1024))),
                int(asset.get("reference_steps", defaults.get("reference_steps", 40))),
                int(asset.get("reference_seed", 1201 + start + index)),
            )
        )

        if references_only:
            continue

        print(f"Generating 3D: {asset_id}")
        profile = resolve_asset_profile(asset)
        processed_metadata = build_processed_metadata(asset, defaults)
        print(three_d.generate_from_reference.remote(profile, defaults))
        print(postprocess_asset.remote(profile, defaults, processed_metadata))


@app.local_entrypoint()
def list_assets(group: str = "") -> None:
    groups = manifest_groups()
    assets = manifest_assets()
    asset_index = {asset["id"]: asset for asset in assets}

    if group:
        ids = groups.get(group, [])
        if not ids:
            raise KeyError(f"Unknown or empty asset group: {group}")
        for asset_id in ids:
            asset = asset_index[asset_id]
            print(f"{asset['id']}\t{asset.get('priority', 'P?')}\t{asset.get('category', '')}\t{asset.get('kind', '')}")
        return

    for asset in assets:
        print(f"{asset['id']}\t{asset.get('priority', 'P?')}\t{asset.get('category', '')}\t{asset.get('kind', '')}")


BLENDER_CLEANUP_SCRIPT = r'''
import argparse
import math
import os
from pathlib import Path

import bpy
from mathutils import Vector


def parse_args():
    parser = argparse.ArgumentParser()
    parser.add_argument("--input", required=True)
    parser.add_argument("--output-base", required=True)
    parser.add_argument("--lod1-ratio", type=float, default=0.5)
    parser.add_argument("--lod2-ratio", type=float, default=0.22)
    parser.add_argument("--target-max-dimension", type=float, default=0.82)
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


def normalize_scene(target_max_dimension):
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
    bpy.context.view_layer.update()

    size = [max_corner[i] - min_corner[i] for i in range(3)]
    max_dimension = max(size)
    if max_dimension <= 0:
        raise RuntimeError("Imported mesh has invalid bounds")

    scale_factor = target_max_dimension / max_dimension
    for obj in bpy.context.scene.objects:
        if obj.parent is None:
            obj.scale *= scale_factor

    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    bpy.context.view_layer.update()

    min_corner = [math.inf, math.inf, math.inf]
    max_corner = [-math.inf, -math.inf, -math.inf]
    for obj in meshes:
        for corner in obj.bound_box:
            world = obj.matrix_world @ Vector(corner)
            for i in range(3):
                min_corner[i] = min(min_corner[i], world[i])
                max_corner[i] = max(max_corner[i], world[i])

    offset = Vector((
        -((min_corner[0] + max_corner[0]) * 0.5),
        -min_corner[1],
        -((min_corner[2] + max_corner[2]) * 0.5),
    ))
    for obj in bpy.context.scene.objects:
        if obj.parent is None:
            obj.location += offset

    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.transform_apply(location=True, rotation=False, scale=False)


def save_textures(output_base):
    out_dir = os.path.dirname(output_base)
    os.makedirs(out_dir, exist_ok=True)
    print(f"Extracting textures from materials...")
    for img in bpy.data.images:
        if img.name in {"Render Result", "Viewer Node"} or not img.has_data:
            continue
        
        name_lower = img.name.lower()
        suffix = "albedo"
        if "normal" in name_lower:
            suffix = "normal"
        elif "roughness" in name_lower or "metallic" in name_lower or "orm" in name_lower:
            suffix = "orm"
        elif "emissive" in name_lower or "emission" in name_lower:
            suffix = "emission"
        else:
            suffix = img.name.replace(" ", "_").replace(".", "_")
            
        filename = f"{output_base}_{suffix}.png"
        img.filepath_raw = filename
        img.file_format = "PNG"
        img.save()
        print(f"Extracted and saved texture to: {filename}")


def create_lods(lod1_ratio, lod2_ratio):
    # 1. Rename original imported meshes to suffix _LOD0
    lod0_meshes = [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]
    for obj in lod0_meshes:
        base_name = obj.name.replace(" ", "_")
        for s in ["_LOD0", "_LOD1", "_LOD2"]:
            if base_name.endswith(s):
                base_name = base_name[:-len(s)]
        obj.name = f"{base_name}_LOD0"

    # 2. Duplicate and decimate for LOD1 and LOD2
    bpy.ops.object.select_all(action="DESELECT")
    for obj in lod0_meshes:
        # Duplicate for LOD1
        obj.select_set(True)
        bpy.context.view_layer.objects.active = obj
        bpy.ops.object.duplicate()
        lod1_obj = bpy.context.active_object
        lod1_obj.name = obj.name.replace("_LOD0", "_LOD1")
        
        # Apply decimate for LOD1
        mod = lod1_obj.modifiers.new(name="Decimate", type="DECIMATE")
        mod.ratio = lod1_ratio
        bpy.ops.object.modifier_apply(modifier=mod.name)
        bpy.ops.object.select_all(action="DESELECT")

        # Duplicate for LOD2
        obj.select_set(True)
        bpy.context.view_layer.objects.active = obj
        bpy.ops.object.duplicate()
        lod2_obj = bpy.context.active_object
        lod2_obj.name = obj.name.replace("_LOD0", "_LOD2")
        
        # Apply decimate for LOD2
        mod = lod2_obj.modifiers.new(name="Decimate", type="DECIMATE")
        mod.ratio = lod2_ratio
        bpy.ops.object.modifier_apply(modifier=mod.name)
        bpy.ops.object.select_all(action="DESELECT")


def export_fbx(output_base):
    output = f"{output_base}.fbx"
    bpy.ops.export_scene.fbx(
        filepath=output,
        use_selection=False,
        add_leaf_bones=False,
        bake_anim=False,
        object_types={"MESH"},
    )
    print(f"Exported LOD unified FBX: {output}")
    return output


def main():
    args = parse_args()
    clear_scene()
    import_model(args.input)
    normalize_scene(args.target_max_dimension)
    save_textures(args.output_base)
    create_lods(args.lod1_ratio, args.lod2_ratio)
    export_fbx(args.output_base)


if __name__ == "__main__":
    main()
'''
