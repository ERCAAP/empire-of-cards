import os
import modal

app = modal.App("inspect-trellis-env")

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
    import glob
    
    result = {}
    path = "/opt/TRELLIS.2"
    if os.path.exists(path):
        result["exists"] = True
        result["contents"] = os.listdir(path)
        # Find all python files
        py_files = glob.glob(os.path.join(path, "*.py"))
        result["py_files"] = [os.path.basename(f) for f in py_files]
        
        # Check if there is an example.py and inspect its content/doc
        example_path = os.path.join(path, "example.py")
        if os.path.exists(example_path):
            with open(example_path, "r", encoding="utf-8") as f:
                result["example_preview"] = f.read()[:4000]
    else:
        result["exists"] = False
        result["contents"] = []
    return result

@app.local_entrypoint()
def main():
    print("Inspecting TRELLIS container...")
    info = inspect_container.remote()
    import pprint
    pprint.pprint(info)
