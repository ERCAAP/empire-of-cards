import modal

app = modal.App("inspect-setup")

inspect_image = (
    modal.Image.debian_slim(python_version="3.11")
    .apt_install("git")
    .run_commands(
        "git clone --depth 1 https://github.com/microsoft/TRELLIS.2 /opt/TRELLIS.2 || true"
    )
)

@app.function(image=inspect_image)
def get_setup_script():
    import os
    setup_path = "/opt/TRELLIS.2/setup.sh"
    if os.path.exists(setup_path):
        with open(setup_path, "r", encoding="utf-8") as f:
            return f.read()
    return "setup.sh does not exist"

@app.local_entrypoint()
def main():
    print(get_setup_script.remote())
