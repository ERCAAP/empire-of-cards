# Modal 3D Asset Pipeline Plan — Empire of Cards

> Version: 1.0  
> Date: 2026-05-22  
> Goal: Generate Unity-ready 3D assets for Empire of Cards using Modal serverless GPUs, Z-Image reference images, TRELLIS.2 image-to-3D, and deterministic post-processing.

---

## 0. Security Note

The Modal token must never be committed to this repository.

Use local Modal authentication only:

```bash
modal token set --token-id <TOKEN_ID> --token-secret <TOKEN_SECRET>
```

After a token has been pasted into chat or any shared log, rotate it in Modal and create a fresh token before production use.

Secrets for Hugging Face or other providers should be stored with Modal Secrets, not in code:

```bash
modal secret create empire-card-assets HF_TOKEN=...
```

---

## 1. Is Modal + TRELLIS.2 + Z-Image Suitable?

Yes, with one important rule:

> AI-generated 3D is not final-game art by itself. It is a fast asset factory that must include cleanup, decimation, texture normalization, LODs, naming, import presets, and manual approval.

### Why Modal Works

Modal is appropriate because this workload needs burst GPU capacity, not a permanently running server.

Use Modal for:

- Serverless GPU generation.
- A10G for cheaper batch jobs and tests.
- A100/H100-class GPUs for heavier TRELLIS.2 jobs if needed.
- Modal Volumes for model weights, generated assets, and backups.
- Modal Secrets for credentials.
- Deployed functions/API endpoints for repeatable generation.

### Why Z-Image Works

Z-Image can generate strong 2D reference images from text prompts. For our pipeline, it should produce clean, isolated, backgroundless orthographic/isometric references.

Settings:

- Steps: 40 by default, as requested.
- Style: `Empire of Cards` prompt pack style.
- Background: transparent if supported; otherwise flat neutral matte background for removal.
- Output: PNG reference image.

### Why TRELLIS.2 Works

TRELLIS.2 is appropriate for high-fidelity image-to-3D generation and can generate textured assets. It is Linux/GPU-oriented and should run remotely rather than inside Unity.

Use TRELLIS.2 for:

- Base mesh generation from Z-Image references.
- PBR/textured GLB output.
- Faster iteration on tabletop props, staff standees, tokens, and business props.

Do not rely on it for final topology without cleanup.

---

## 2. Production Architecture

```text
Prompt Pack
  ↓
Asset Manifest
  ↓
Z-Image reference generator on Modal
  ↓
Reference image QA
  ↓
TRELLIS.2 image-to-3D on Modal
  ↓
Mesh/material post-process
  ↓
Unity export package
  ↓
Local Unity import + prefab setup
  ↓
Manual visual QA in Game scene
```

## 2.1 Modal Apps

Create one Modal app with three callable workers:

```text
empire-card-asset-studio
├── ReferenceGenerator
│   └── Z-Image text-to-image, 40 steps
├── ThreeDGenerator
│   └── TRELLIS.2 image-to-3D
└── AssetPostProcessor
    └── Blender/trimesh cleanup, LODs, texture packing, Unity export
```

The user already has an older app named `aquarium-3d-studio`; keep Empire of Cards separate to avoid mixing outputs and dependencies.

## 2.2 Modal Storage

Use two Modal Volumes:

```text
empire-card-model-cache
  /models/z-image
  /models/trellis2

empire-card-assets
  /references
  /raw_glb
  /processed
  /unity
  /backups
  /manifests
  /logs
```

Why:

- Model weights are cached once.
- Generated assets survive container shutdown.
- Outputs can be backed up and reprocessed.
- Batch jobs are reproducible.

## 2.3 Backup Strategy

Every asset gets:

```text
asset_id/
├── prompt.txt
├── generation_config.json
├── reference.png
├── raw.glb
├── processed_LOD0.fbx
├── processed_LOD1.fbx
├── processed_LOD2.fbx
├── textures/
│   ├── albedo.png
│   ├── normal.png
│   ├── orm.png
│   └── emission.png
├── unity_meta.json
└── preview_turntable.mp4 or preview.png
```

Backups:

- Modal Volume copy.
- Local repo-adjacent export under `Artifacts/generated-3d-assets/`.
- Do not store large binary generated assets in git by default.
- Store approved import metadata and manifests in git.

---

## 3. Unity Asset Quality Targets

## 3.1 Platform Target

Primary:

- PC / Steam

Secondary:

- Runs acceptably on lower-end desktop and handheld/Steam Deck-like hardware.

Not primary:

- Mobile release

But assets should be mobile-conscious so the art pipeline stays lean.

## 3.2 Mesh Budgets

| Asset Type | LOD0 Desktop | LOD1 Mid | LOD2 Low/Mobile | Notes |
|---|---:|---:|---:|---|
| Card model | 100-300 tris | 80-150 | 40-80 | Mostly reusable |
| Slot tray | 150-500 tris | 100-250 | 50-120 | Repeated many times |
| Token/coin | 100-300 tris | 60-150 | 30-80 | Many on board |
| Small prop | 500-1,500 tris | 250-700 | 100-300 | Tables, crates, laptop |
| Hero prop | 1,500-3,500 tris | 700-1,500 | 300-700 | Espresso machine, fast food counter |
| Staff standee | 800-2,000 tris | 400-900 | 150-400 | Use billboard/standee if needed |
| Main table | 1,000-3,000 tris | 500-1,500 | 250-700 | One instance |

Scene target:

- Normal gameplay visible meshes: under 80k-150k tris.
- Strong target after LODs: under 60k visible tris for board scene.

## 3.3 Texture Budgets

| Asset Type | Desktop | Low/Mobile | Notes |
|---|---:|---:|---|
| Card face atlas | 1024-2048 | 1024 | UI readability matters |
| Small prop | 512 | 256 | Use shared palettes |
| Hero prop | 1024 | 512 | Only key props |
| Staff standee | 512-1024 | 256-512 | Keep simple |
| Table/board | 1024-2048 | 1024 | Can use tiling material |
| Tokens | 256-512 | 128-256 | Many repeated |

Texture output:

- `albedo.png`
- `normal.png` if useful
- `orm.png` packed as Occlusion/Roughness/Metallic
- `emission.png` only for lights/screens/neon

Avoid many unique 2K textures. Use material palettes and atlases.

## 3.4 Materials

Unity material target:

- URP Lit or custom Toon/URP shader.
- Low roughness variation.
- No photorealistic dirt.
- Colored rim/outline through material or mesh edge style.
- Shared material instances where possible.

Material families:

```text
MAT_Table_DarkWood
MAT_Board_FeltDark
MAT_Card_Base
MAT_Card_Edge
MAT_Slot_Operation
MAT_Slot_Staff
MAT_Slot_Marketing
MAT_Slot_Supplier
MAT_Slot_Risk
MAT_Token_Player
MAT_Token_Rival
MAT_Token_Neutral
MAT_Money_Gold
MAT_Prop_FastFood
MAT_Prop_Cafe
MAT_Prop_Grocery
MAT_Prop_Tech
MAT_Prop_Clothing
MAT_Warning_RedOrange
```

---

## 4. File Naming

Use deterministic names:

```text
EOC_<Category>_<Venture>_<AssetName>_<Variant>_<LOD>
```

Examples:

```text
EOC_Prop_FastFood_GrillFryer_A_LOD0.fbx
EOC_Staff_Cafe_Barista_A_LOD1.fbx
EOC_Token_Global_CustomerPlayer_A_LOD0.fbx
EOC_Crisis_Grocery_BrokenFridge_A_LOD0.fbx
```

Texture names:

```text
EOC_Prop_FastFood_GrillFryer_A_albedo.png
EOC_Prop_FastFood_GrillFryer_A_normal.png
EOC_Prop_FastFood_GrillFryer_A_orm.png
```

Unity prefab names:

```text
PF_EOC_Prop_FastFood_GrillFryer_A
PF_EOC_Staff_Cafe_Barista_A
```

---

## 5. Asset Manifest

This is the first production batch. It covers the whole game, with Phase 1 priority marked.

## 5.1 Global Board Assets

| ID | Asset | Priority |
|---|---|---|
| global_table_main | Main dark desk/table | P1 |
| global_board_surface | Recessed board/felt surface | P1 |
| global_slot_operation | Operation slot tray | P1 |
| global_slot_staff | Staff slot tray | P1 |
| global_slot_marketing | Marketing slot tray | P1 |
| global_slot_supplier | Supplier slot tray | P1 |
| global_slot_temp | Temp/risk slot tray | P1 |
| global_slot_play | Play slot tray | P1 |
| global_slot_sell | Sell slot tray | P1 |
| global_card_generic | Generic 3D card body | P1 |
| global_card_stack | Deck/discard card stack | P1 |
| global_customer_token_player | Player customer token | P1 |
| global_customer_token_rival | Rival customer token | P1 |
| global_customer_token_neutral | Neutral customer token | P1 |
| global_market_block | Market-share block | P1 |
| global_money_coin | Money token/coin | P1 |
| global_warning_stamp | Warning/legal stamp | P1 |
| global_contract_paper | Supplier contract paper | P2 |
| global_invoice_stack | Bill/invoice stack | P2 |

## 5.2 Fast Food Assets

| ID | Asset | Priority |
|---|---|---|
| ff_core_counter | Fast food counter | P1 |
| ff_grill_fryer | Grill/fryer station | P1 |
| ff_seating_set | Tables and stools | P1 |
| ff_delivery_bag | Delivery bag | P1 |
| ff_menu_board | Menu board | P2 |
| ff_cleaning_bucket | Cleaning bucket/mop | P2 |
| ff_supplier_meat_crate | Meat/butcher crate | P1 |
| ff_supplier_veg_crate | Vegetable crate | P1 |
| ff_supplier_bread_basket | Bread basket | P2 |
| ff_marketing_flyers | Flyer stack | P1 |
| ff_marketing_google_sign | Google review/star sign | P1 |
| ff_staff_cook | Cook standee | P1 |
| ff_staff_cashier | Cashier standee | P1 |
| ff_staff_courier | Courier standee | P1 |
| ff_staff_cleaner | Cleaner standee | P2 |
| ff_staff_manager | Branch manager standee | P2 |
| ff_staff_waiter | Garson/waiter standee | P2 |
| ff_staff_busser | Komi/busser standee | P2 |
| ff_crisis_review_storm | Bad review phone token | P1 |
| ff_crisis_hygiene | Hygiene inspection clipboard | P1 |
| ff_crisis_broken_fryer | Broken fryer smoke marker | P2 |
| ff_crisis_delivery_fee | Delivery fee notice | P2 |

## 5.3 Cafe Assets

| ID | Asset | Priority |
|---|---|---|
| cf_espresso_machine | Espresso machine | P1 |
| cf_grinder | Coffee grinder | P2 |
| cf_pastry_display | Pastry display case | P1 |
| cf_table_set | Cafe table/chairs | P1 |
| cf_chalkboard | Chalkboard menu | P2 |
| cf_plant_lamp | Plant/lamp ambience prop | P2 |
| cf_supplier_beans | Coffee bean sack | P1 |
| cf_supplier_milk_crate | Milk crate | P1 |
| cf_supplier_pastry_box | Pastry supplier box | P2 |
| cf_marketing_instagram_phone | Instagram phone | P1 |
| cf_marketing_loyalty_card | Loyalty stamp card | P1 |
| cf_staff_barista | Barista standee | P1 |
| cf_staff_cashier | Cafe cashier standee | P1 |
| cf_staff_floor | Floor staff standee | P1 |
| cf_staff_cleaner | Cleaner standee | P2 |
| cf_staff_shift_lead | Shift lead standee | P2 |
| cf_crisis_burnout | Barista burnout token | P1 |
| cf_crisis_milk_shortage | Empty milk crate token | P1 |
| cf_crisis_wifi | Broken WiFi router token | P2 |
| cf_crisis_slow_service | Slow service complaint note | P1 |
| cf_crisis_music_license | Music license warning | P2 |

## 5.4 Grocery / Market Assets

| ID | Asset | Priority |
|---|---|---|
| gr_shelf_row | Grocery shelf row | P1 |
| gr_checkout_counter | Checkout counter/scanner | P1 |
| gr_fridge | Fridge/cold display | P1 |
| gr_produce_crate | Fresh produce crate | P1 |
| gr_bread_basket | Bread basket | P2 |
| gr_whatsapp_phone | WhatsApp order phone | P1 |
| gr_delivery_basket | Delivery basket | P2 |
| gr_supplier_wholesale_crate | Wholesale crate | P1 |
| gr_supplier_dairy_crate | Dairy crate | P1 |
| gr_supplier_drink_crate | Drink partner crate | P2 |
| gr_marketing_neighborhood_poster | Neighborhood poster | P1 |
| gr_marketing_loyalty_card | Loyalty card | P1 |
| gr_marketing_night_open_sign | Night-open sign | P2 |
| gr_staff_cashier | Grocery cashier standee | P1 |
| gr_staff_shelf_worker | Shelf worker/reyon standee | P1 |
| gr_staff_fresh_lead | Fresh produce lead standee | P1 |
| gr_staff_courier | Grocery courier standee | P2 |
| gr_staff_shift | Shift worker standee | P2 |
| gr_crisis_broken_fridge | Broken fridge warning | P1 |
| gr_crisis_spoilage | Spoiled produce token | P1 |
| gr_crisis_expiry | Expiry-date stamp | P1 |
| gr_crisis_veresiye | Unpaid ledger/veresiye notebook | P2 |
| gr_crisis_power_outage | Power outage icon | P2 |
| gr_crisis_chain_market | Chain-market threat sign | P2 |

## 5.5 Tech App Assets

| ID | Asset | Priority |
|---|---|---|
| tc_laptop_phone | Laptop + app phone | P2 |
| tc_server_stack | Server stack | P2 |
| tc_router_cloud | Router/cloud block | P2 |
| tc_analytics_chart | Analytics dashboard | P2 |
| tc_bug_token | Bug token | P2 |
| tc_cloud_bill | Cloud bill | P2 |
| tc_staff_developer | Developer standee | P2 |
| tc_staff_designer | Designer standee | P3 |
| tc_staff_growth | Growth marketer standee | P3 |
| tc_staff_support | Support standee | P3 |
| tc_staff_pm | Product manager standee | P2 |
| tc_crisis_crash | Crash warning screen | P2 |
| tc_crisis_server_overload | Server overload marker | P2 |
| tc_crisis_security | Security breach lock | P2 |
| tc_crisis_review_bomb | Review bomb phone | P2 |
| tc_crisis_churn | Churn leak icon | P2 |

## 5.6 Clothing Store Assets

| ID | Asset | Priority |
|---|---|---|
| cl_display_mannequin | Display mannequin | P2 |
| cl_clothing_rack | Clothing rack | P2 |
| cl_folded_table | Folded clothes table | P2 |
| cl_checkout_counter | Clothing checkout | P2 |
| cl_return_box | Return box | P2 |
| cl_fabric_swatches | Fabric swatches | P2 |
| cl_photo_light | Photo shoot light | P3 |
| cl_staff_sales | Sales associate standee | P2 |
| cl_staff_cashier | Clothing cashier standee | P3 |
| cl_staff_tailor | Tailor standee | P2 |
| cl_staff_stockroom | Stockroom worker standee | P3 |
| cl_staff_manager | Store manager standee | P3 |
| cl_crisis_returns | Return pile token | P2 |
| cl_crisis_bad_fit | Bad fit complaint | P2 |
| cl_crisis_season_mismatch | Seasonal mismatch tag | P2 |
| cl_crisis_empty_stock | Empty stock box | P2 |
| cl_crisis_damaged_fabric | Damaged fabric marker | P2 |

---

## 6. Modal Implementation Plan

## 6.1 Repo Structure

Add these scripts later:

```text
tools/asset_pipeline/
├── modal_app.py
├── asset_manifest.yaml
├── prompts/
│   ├── global_style.txt
│   └── negative.txt
├── postprocess/
│   ├── blender_cleanup.py
│   ├── build_lods.py
│   ├── pack_textures.py
│   └── unity_export.py
└── README.md
```

Do not commit generated binaries by default.

Generated outputs:

```text
Artifacts/generated-3d-assets/
├── references/
├── raw_glb/
├── processed/
├── unity/
├── previews/
└── manifest_results.json
```

## 6.2 Modal App Shape

Pseudo-architecture:

```python
import modal

app = modal.App("empire-card-asset-studio")

model_cache = modal.Volume.from_name("empire-card-model-cache", create_if_missing=True)
asset_store = modal.Volume.from_name("empire-card-assets", create_if_missing=True)
secrets = [modal.Secret.from_name("empire-card-assets")]

z_image = (
    modal.Image.debian_slim(python_version="3.11")
    .pip_install("torch", "diffusers", "transformers", "accelerate", "safetensors", "pillow")
)

trellis_image = (
    modal.Image.debian_slim(python_version="3.11")
    # install TRELLIS.2 deps after validating the exact repo requirements
)

@app.cls(
    image=z_image,
    gpu="A10G",
    volumes={"/cache": model_cache, "/assets": asset_store},
    secrets=secrets,
    timeout=60 * 30,
)
class ReferenceGenerator:
    def generate(self, asset_id: str, prompt: str, steps: int = 40):
        ...

@app.cls(
    image=trellis_image,
    gpu="A100-40GB",
    volumes={"/cache": model_cache, "/assets": asset_store},
    timeout=60 * 60,
)
class ThreeDGenerator:
    def generate_from_image(self, asset_id: str, image_path: str, resolution: int = 512):
        ...

@app.function(
    image=modal.Image.debian_slim(python_version="3.11").apt_install("blender"),
    volumes={"/assets": asset_store},
    timeout=60 * 30,
)
def postprocess(asset_id: str):
    ...
```

## 6.3 GPU Selection

| Stage | GPU | Reason |
|---|---|---|
| Z-Image reference tests | A10G | Lower cost, enough for image generation tests |
| Z-Image final refs | A10G / L40S | Depends on speed/cost |
| TRELLIS.2 low-res tests | A10G if install works | Cost control |
| TRELLIS.2 final assets | A100-40GB or better | Safer VRAM and throughput |
| Bulk postprocess | CPU | Blender/trimesh cleanup |

Start cheap:

1. Generate 3 assets on A10G.
2. Test TRELLIS.2 install and output quality.
3. Move final batch to A100 only after quality is proven.

---

## 7. Generation Settings

## 7.1 Z-Image Reference Settings

Default:

```json
{
  "steps": 40,
  "width": 1024,
  "height": 1024,
  "guidance_scale": "model_default_or_5_to_7_if_supported",
  "background": "transparent_or_flat_neutral",
  "view": "orthographic_three_quarter_top_down",
  "style": "Empire of Cards tabletop low-poly/toon"
}
```

Reference prompt format:

```text
[Global Style Block]

Create an isolated 3D asset reference image for [ASSET_NAME].
[Asset-specific prompt from UI_UX_3D_PROMPT_PACK.md]
Plain neutral background or transparent background.
Orthographic three-quarter top-down view.
No text labels unless the asset itself requires a sign.

[Global Negative Prompt]
```

## 7.2 TRELLIS.2 Settings

Start with:

```json
{
  "resolution": 512,
  "output_format": "glb",
  "materials": "pbr",
  "seed": "stored_per_asset",
  "input_image": "reference.png"
}
```

Only raise resolution after testing:

- 512 for most props.
- 1024 for hero props if quality gain is worth cost.
- Avoid 1536 unless a hero asset truly needs it.

## 7.3 Mesh Cleanup Settings

For every generated asset:

```json
{
  "center_origin": true,
  "scale_to_unit_box": true,
  "remove_hidden_geometry": true,
  "merge_close_vertices": true,
  "generate_lods": true,
  "lod_ratios": [1.0, 0.45, 0.18],
  "max_texture_size_desktop": 1024,
  "max_texture_size_low": 512,
  "export_fbx": true,
  "keep_raw_glb": true
}
```

---

## 8. Unity Import Plan

## 8.1 Import Folder Structure

Approved assets should land here:

```text
Assets/_EmpireOfCards/Art/Generated3D/
├── Global/
├── FastFood/
├── Cafe/
├── Grocery/
├── TechApp/
└── ClothingStore/

Assets/_EmpireOfCards/Materials/Generated/
Assets/_EmpireOfCards/Prefabs/Generated3D/
```

## 8.2 Unity Model Settings

Default:

- Scale factor normalized by postprocessor.
- Generate colliders: off by default.
- Read/write mesh: off for final unless runtime manipulation needs it.
- Mesh compression: medium for small props, low/off for card/slot if visual artifacts appear.
- Import blend shapes: off.
- Import cameras/lights: off.
- Animation: off for static props.

## 8.3 Unity Texture Settings

Default:

- Albedo: sRGB on.
- Normal: normal map type.
- ORM: sRGB off.
- Max size: 512 or 1024 depending asset.
- Compression: platform default, test visual quality.
- Mip maps: on for 3D props.
- Filter: bilinear/trilinear depending final look.

## 8.4 Prefab Setup

Every approved model becomes a prefab:

```text
PF_EOC_<Category>_<Venture>_<AssetName>_A
```

Prefab must include:

- Mesh renderer.
- Correct material.
- Optional simple collider only if interactable.
- LODGroup if multiple LODs exist.
- No scripts unless needed.
- Pivot placed at tabletop contact point.

---

## 9. Quality Gate

Before an asset is accepted:

- It reads clearly from the default board camera.
- It matches the venture color/material language.
- It does not look photorealistic or noisy.
- It does not contain unreadable generated text.
- It has LODs.
- It has texture sizes within budget.
- It has clean origin and scale.
- It casts a useful soft shadow.
- It does not exceed polygon budget.
- It can be used on desktop and scaled down for handheld/low mode.

Reject or regenerate if:

- Silhouette is confusing.
- Texture has text artifacts.
- Model has melted geometry.
- It looks like a generic mobile asset pack.
- It has too many disconnected floating pieces.
- It is visually too close to another venture.

---

## 10. Step-by-Step Execution

## Step 1 — Token & Modal Setup

- Rotate the pasted Modal token.
- Run `modal token set` locally with the fresh token.
- Create Modal secrets.
- Create Modal volumes.

## Step 2 — Build Minimal Modal App

- Implement `ReferenceGenerator` only.
- Generate 3 test references:
  - `ff_grill_fryer`
  - `cf_espresso_machine`
  - `gr_checkout_counter`
- Verify style consistency.

## Step 3 — Add TRELLIS.2 Worker

- Clone/install TRELLIS.2 inside Modal image.
- Cache weights in `empire-card-model-cache`.
- Generate raw GLB for the 3 test assets.
- Check raw geometry and material output.

## Step 4 — Add Postprocessor

- Normalize scale/origin.
- Export FBX.
- Generate LODs.
- Resize/pack textures.
- Create preview PNG.

## Step 5 — Import First 3 Into Unity

- Place in generated art folders.
- Create prefabs.
- Drop them into `Board3D` as manually referenced or runtime-loaded props.
- Verify in Game scene.

## Step 6 — Phase 1 Batch

Generate all P1 Global + Fast Food + Cafe + Grocery assets.

Do not generate Tech App/Clothing full batch until Phase 1 visual quality is approved.

## Step 7 — Integration

- Replace primitive theme props in `VentureBoardThemeProfile` with generated prefabs.
- Keep fallback primitive props for missing assets.
- Add model quality mode:
  - High: LOD0/1024 textures.
  - Medium: LOD1/512 textures.
  - Low: LOD2/256-512 textures.

## Step 8 — Backup & Manifest Lock

- Export manifest results.
- Copy approved assets locally.
- Keep raw GLB and references in Modal Volume.
- Store approved metadata in git.

---

## 11. First Batch Recommendation

Do not start with all assets. Start with 9 assets:

Global:

1. `global_slot_operation`
2. `global_card_generic`
3. `global_market_block`

Fast Food:

4. `ff_grill_fryer`
5. `ff_staff_cook`

Cafe:

6. `cf_espresso_machine`
7. `cf_staff_barista`

Grocery:

8. `gr_checkout_counter`
9. `gr_produce_crate`

This proves:

- Hard-surface prop quality.
- Staff standee quality.
- Token/slot quality.
- Unity import quality.
- Venture color distinction.

Only after this should the full P1 batch run.

---

## 12. References

- Modal Volumes: https://modal.com/docs/guide/volumes
- Modal Secrets: https://modal.com/docs/guide/secrets
- Modal Secret API: https://modal.com/docs/reference/modal.Secret
- TRELLIS.2 official GitHub: https://github.com/microsoft/TRELLIS.2
- TRELLIS.2 project page: https://microsoft.github.io/TRELLIS.2/
- Z-Image Hugging Face model: https://huggingface.co/Tongyi-MAI/Z-Image

