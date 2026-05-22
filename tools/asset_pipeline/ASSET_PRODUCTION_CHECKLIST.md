# Empire of Cards Asset Production Checklist

This checklist is the production gate for generated assets. Every asset is drawn, reviewed, and converted separately.

## Non-Negotiable Rule

Do not batch-generate final assets. Produce one reference PNG, inspect it, approve or reject it, then move to the next asset.

Every approved reference must be a transparent cutout with no background. No floor, no room, no wall, no horizon line, no desk, no lamp, no extra cards, no labels, no fake text, no shadow, and no extra props.

## Workflow

1. Generate a single reference:

```bash
modal run tools/asset_pipeline/modal_app.py::generate_reference --asset-id <asset_id>
```

2. Download that single reference:

```bash
modal volume get --force empire-card-assets /references/<asset_id> Artifacts/generated-3d-assets/references/
```

3. Inspect `Artifacts/generated-3d-assets/references/<asset_id>/reference.png`.

4. If approved, generate the 3D asset:

```bash
modal run tools/asset_pipeline/modal_app.py::generate_3d_asset --asset-id <asset_id>
```

5. Download and inspect the processed model before Unity import.

## Approval Checks

- [ ] One asset only.
- [ ] Transparent background.
- [ ] No floor, wall, room, desk, lamp, extra cards, labels, fake text, shadows, or background props.
- [ ] Full silhouette visible and uncropped.
- [ ] Top-down/isometric readability is strong.
- [ ] Mesh target is simple: few loose parts, clear bevels, no hair-thin details.
- [ ] Shared Empire of Cards material language is preserved.
- [ ] Venture colors are accents only; the whole set still feels unified.
- [ ] Mobile/low-spec silhouette would survive LOD reduction.

## First Batch Status

| Asset ID | Reference | 3D | Unity Import | Notes |
| --- | --- | --- | --- | --- |
| `global_slot_operation` | Review after rembg pass | Not started | Not started | First transparent workflow test. |
| `global_card_generic` | Not started | Not started | Not started | Generate only after slot approval. |
| `global_market_block` | Not started | Not started | Not started | Generate only after card approval. |
| `ff_grill_fryer` | Not started | Not started | Not started | Fast Food prop. |
| `ff_staff_cook` | Not started | Not started | Not started | Staff standee. |
| `cf_espresso_machine` | Not started | Not started | Not started | Cafe prop. |
| `cf_staff_barista` | Not started | Not started | Not started | Staff standee. |
| `gr_checkout_counter` | Not started | Not started | Not started | Grocery prop. |
| `gr_produce_crate` | Not started | Not started | Not started | Grocery prop. |
