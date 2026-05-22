import os
import sys

# Import app and image
from tools.asset_pipeline.modal_app import app, trellis_image

@app.function(
    image=trellis_image,
)
def inspect_code():
    import os
    import sys
    sys.path.append("/opt/TRELLIS.2")
    
    if not os.path.exists("/opt/TRELLIS.2"):
        return "TRELLIS.2 not cloned"
    
    # Search for Trellis2ImageTo3DPipeline definition
    res = []
    for root, dirs, files in os.walk("/opt/TRELLIS.2/trellis2"):
        for f in files:
            if f.endswith(".py"):
                path = os.path.join(root, f)
                with open(path, "r", encoding="utf-8") as file:
                    content = file.read()
                    if "class Trellis2ImageTo3DPipeline" in content or "def from_pretrained" in content:
                        res.append(f"--- File: {path} ---")
                        lines = content.split("\n")
                        for idx, line in enumerate(lines):
                            if "class Trellis2ImageTo3DPipeline" in line or "def from_pretrained" in line:
                                start = max(0, idx - 10)
                                end = min(len(lines), idx + 20)
                                res.append("\n".join(f"{i+1}: {l}" for i, l in zip(range(start, end), lines[start:end])))
    return "\n\n".join(res)

@app.local_entrypoint()
def main():
    print(inspect_code.remote())
