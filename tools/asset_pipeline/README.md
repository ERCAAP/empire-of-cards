# Empire of Cards Asset Pipeline

This folder contains the Modal serverless GPU scaffold for generating Unity-ready 3D asset references and models.

## Current Status

Ready:

- First-batch asset manifest.
- Global style and negative prompts.
- Modal app scaffold.
- Z-Image reference generator worker.
- One-asset-at-a-time reference generation.
- Transparent PNG cutout pass for references.
- TRELLIS.2 worker shell with configurable command template.
- Blender postprocess entrypoint for raw GLB to FBX.
- Local artifact folders under `Artifacts/generated-3d-assets/`.

Not done yet:

- Modal token is not configured by Codex because secrets must not be written into repo files.
- TRELLIS.2 exact CLI command must be validated on Modal after dependency install.
- No generated models have been imported into Unity yet.

## Security

Rotate any token that has been pasted into chat before production use.

Then authenticate locally:

```bash
modal token set --token-id <TOKEN_ID> --token-secret <TOKEN_SECRET>
```

This machine currently has a Modal secret named `huggingface`, and the reference generator uses it by default. Create or update that secret without committing credentials:

```bash
modal secret create huggingface HF_TOKEN=<HF_TOKEN>
```

If TRELLIS.2 needs a specific CLI command, store it in the same secret or update `modal_app.py` to use a dedicated secret:

```bash
modal secret create huggingface \
  HF_TOKEN=<HF_TOKEN> \
  TRELLIS_COMMAND_TEMPLATE='<validated command with {input} and {output}>'
```

Create Modal volumes and optional Hugging Face secret through the helper:

```bash
HF_TOKEN=<HF_TOKEN> ./tools/asset_pipeline/setup_modal.sh
```

If you do not have a Hugging Face token yet, run it without `HF_TOKEN`; the script will still create the volumes:

```bash
./tools/asset_pipeline/setup_modal.sh
```

## First Test

Generate one isolated transparent reference image first:

```bash
modal run tools/asset_pipeline/modal_app.py::generate_reference --asset-id global_slot_operation
```

Download and inspect that exact asset:

```bash
modal volume get --force empire-card-assets /references/global_slot_operation Artifacts/generated-3d-assets/references/
```

Generate each asset separately. Do not bulk-generate; each reference must be approved before the next asset starts:

```bash
modal run tools/asset_pipeline/modal_app.py::generate_reference --asset-id global_card_generic
modal run tools/asset_pipeline/modal_app.py::generate_reference --asset-id global_market_block
modal run tools/asset_pipeline/modal_app.py::generate_reference --asset-id ff_grill_fryer
```

Generate 3D after TRELLIS.2 command is validated:

```bash
modal run tools/asset_pipeline/modal_app.py::generate_3d_asset --asset-id global_slot_operation
```

The old `generate_first_batch` entrypoint is guarded and fails unless `--allow-batch true` is passed. It should not be used for production asset creation.

## Per-Asset Approval Rule

An asset is not ready for TRELLIS.2 until all checks pass:

- Exactly one object in the image.
- Transparent PNG output with no floor, room, wall, horizon, desk, lamp, card, label, fake text, shadow, or background prop.
- Whole object visible and uncropped.
- Shape is simple enough for reconstruction; no tiny loose parts.
- Color and material match the shared Empire of Cards art bible.
- Asset role is readable from the top-down/isometric board camera.

## First Batch Assets

The first batch is intentionally small, but each item should still be generated and approved one by one:

- `global_slot_operation`
- `global_card_generic`
- `global_market_block`
- `ff_grill_fryer`
- `ff_staff_cook`
- `cf_espresso_machine`
- `cf_staff_barista`
- `gr_checkout_counter`
- `gr_produce_crate`

This proves the pipeline before spending GPU time on the full asset list.

## Output Layout

Modal volume paths:

```text
/assets/references/<asset_id>/reference.png
/assets/raw_glb/<asset_id>/raw.glb
/assets/processed/<asset_id>/EOC_<asset_id>_LOD0.fbx
/assets/logs/<asset_id>/*.log
```

Local artifact mirror:

```text
Artifacts/generated-3d-assets/
```
