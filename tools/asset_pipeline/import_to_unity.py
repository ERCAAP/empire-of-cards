import json
import shutil
import subprocess
import sys
from pathlib import Path


def load_processed_manifest(processed_dir: Path, asset_id: str) -> dict:
    manifest_path = processed_dir / asset_id / "import_manifest.json"
    if manifest_path.exists():
        return json.loads(manifest_path.read_text(encoding="utf-8"))
    return {
        "asset_id": asset_id,
        "art_destination": "Assets/_EmpireOfCards/Art/Generated3D/" + asset_id,
        "material_destination": "Assets/_EmpireOfCards/Materials/Generated3D/" + asset_id,
    }


def download_processed_asset(project_root: Path, asset_id: str) -> Path:
    target_root = project_root / "Artifacts" / "generated-3d-assets" / "processed"
    target_root.mkdir(parents=True, exist_ok=True)
    cmd = [
        "modal",
        "volume",
        "get",
        "--force",
        "empire-card-assets",
        f"/processed/{asset_id}",
        str(target_root),
    ]
    print(f"Downloading processed files for {asset_id} from Modal volume...")
    subprocess.run(cmd, check=True)
    return target_root


def copy_processed_files(project_root: Path, processed_root: Path, asset_id: str) -> list[str]:
    metadata = load_processed_manifest(processed_root, asset_id)
    processed_dir = processed_root / asset_id
    if not processed_dir.exists():
        raise FileNotFoundError(f"Processed asset folder not found: {processed_dir}")

    art_dir = project_root / metadata["art_destination"]
    material_dir = project_root / metadata["material_destination"]
    art_dir.mkdir(parents=True, exist_ok=True)
    material_dir.mkdir(parents=True, exist_ok=True)

    copied_files: list[str] = []
    for file in processed_dir.iterdir():
        suffix = file.suffix.lower()
        if suffix == ".fbx":
            dest = art_dir / file.name
        elif suffix in {".png", ".jpg", ".jpeg", ".tga", ".webp"}:
            dest = material_dir / file.name
        elif suffix == ".json":
            dest = art_dir / file.name
        else:
            continue

        shutil.copy2(file, dest)
        copied_files.append(str(dest))
        print(f"Copied {file.name} -> {dest}")

    if not copied_files:
        raise RuntimeError(f"No importable files were found for {asset_id} under {processed_dir}")

    return copied_files


def main(asset_id: str) -> None:
    project_root = Path(__file__).resolve().parents[2]
    processed_root = download_processed_asset(project_root, asset_id)
    copied_files = copy_processed_files(project_root, processed_root, asset_id)
    print(f"Imported {len(copied_files)} files for {asset_id} into Unity asset folders.")


if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python tools/asset_pipeline/import_to_unity.py <asset_id>")
        sys.exit(1)
    main(sys.argv[1])
