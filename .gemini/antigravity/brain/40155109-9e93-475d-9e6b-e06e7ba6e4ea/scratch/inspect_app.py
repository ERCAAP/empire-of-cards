import os
import modal

app = modal.App("inspect-trellis-app")

trellis_image = (
    modal.Image.debian_slim(python_version="3.11")
    .apt_install("git", "ffmpeg", "libgl1", "libglib2.0-0")
    .pip_install("pyyaml")
    .run_commands(
        "git clone --depth 1 https://github.com/microsoft/TRELLIS.2 /opt/TRELLIS.2 || true",
        "if [ -f /opt/TRELLIS.2/requirements.txt ]; then pip install -r /opt/TRELLIS.2/requirements.txt; fi",
    )
)

@app.function(
    image=trellis_image
)
def inspect_container():
    import os
    
    result = {}
    path = "/opt/TRELLIS.2"
    app_path = os.path.join(path, "app.py")
    if os.path.exists(app_path):
        with open(app_path, "r", encoding="utf-8") as f:
            content = f.read()
            # Let's get the functions or blocks that define the pipeline parameters and to_glb parameters
            result["app_preview"] = content
    else:
        result["app_preview"] = "app.py not found"
    return result

@app.local_entrypoint()
def main():
    print("Inspecting app.py from TRELLIS.2...")
    info = inspect_container.remote()
    # Save output to scratch directory locally to examine
    import json
    out_file = "/Users/omerercan/Documents/card-games-buisnes/.gemini/antigravity/brain/40155109-9e93-475d-9e6b-e06e7ba6e4ea/scratch/trellis_app_code.py"
    with open(out_file, "w", encoding="utf-8") as f:
        f.write(info["app_preview"])
    print(f"Saved app.py contents to {out_file}")
