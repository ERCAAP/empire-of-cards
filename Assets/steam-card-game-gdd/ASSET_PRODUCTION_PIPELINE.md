# Empire of Cards Asset Production Pipeline

> Pipeline for turning the asset bible and prompt pack into Unity-ready production assets.

## 1. Production Order

### Phase 0: P0 Placeholder

Goal: prove gameplay.

Includes:

- capsule/cube customers, staff, props.
- flat color ownership materials.
- simple UGUI panels.
- DOTween placeholder pulses.
- basic audio ticks.

Exit criteria:

- Card placement creates a slot visual and world manifestation.
- Gray/blue/red customer flow is readable.
- Staff stress and queue pressure are visible.

### Phase 1: P0 Final Style Pass

Goal: first playable Toy Diorama quality for Fast Food core loop.

Includes:

- final-ish shared customers.
- Fast Food staff and business modules.
- card frames, slot frames, HUD, event panel.
- core VFX and Toy Studio light rig.

Exit criteria:

- Fast Food first-turn, card inspect, slot placement, resolve and staff crisis are visually coherent.

### Phase 2: P1 Venture Expansion

Goal: all five ventures visually distinct.

Includes:

- Cafe, Tech App, Giyim, Market staff.
- venture-specific business modules.
- venture-specific UI tint/icon support.
- rival modules.

Exit criteria:

- Each venture can be identified from BoardOverview without reading text.

### Phase 3: P2 Polish and Meta

Goal: full-game polish.

Includes:

- holding/exit assets.
- advanced decorative props.
- more animation variety.
- audio ambience and scale upgrade effects.

Exit criteria:

- Exit/meta loop has visual payoff.
- Board remains readable under all major event types.

## 2. Naming Convention

Use:

```text
EOC_[Category]_[Venture]_[AssetName]_[Variant]
```

Examples:

- `EOC_CHAR_FastFood_Waiter_Stressed`
- `EOC_PROP_Cafe_EspressoMachine_Overloaded`
- `EOC_UI_Shared_CardFrame_Risk`
- `EOC_VFX_Shared_StressAura_High`
- `EOC_AUDIO_Shared_CardCommit_Install`

Category values:

- `CHAR`
- `ANIM`
- `PROP`
- `ENV`
- `CARD`
- `SLOT`
- `UI`
- `VFX`
- `MAT`
- `LIGHT`
- `AUDIO`
- `DEBUG`

Venture values:

- `Shared`
- `FastFood`
- `Cafe`
- `TechApp`
- `Giyim`
- `Market`
- `Meta`

## 3. Unity Folder Mapping

```text
Assets/EmpireOfCards/
  Art/
    Characters/
    BusinessModules/
    District/
    Cards/
    UI/
    VFX/
    Materials/
    Lighting/
    Audio/
    Debug/
  Prefabs/
    Characters/
    BusinessModules/
    Board/
    Cards/
    UI/
    VFX/
  Data/
    AssetManifests/
    Ventures/
    Cards/
    Events/
    Levels/
  Scenes/
```

GDD and production docs stay under:

```text
Assets/steam-card-game-gdd/
```

## 4. Import Settings

### 4.1 Meshes

- Scale must match Toy Diorama board units.
- Apply transforms before export.
- Pivot bottom-center for characters and freestanding props.
- Pivot at interaction point only when gameplay needs it.
- Keep mesh simple; readability over detail.
- Use separate child transform anchors for VFX if needed.

### 4.2 Textures and Materials

- Prefer simple materials and color blocks.
- Avoid noisy photoreal textures.
- Customer ownership colors must be material-driven.
- Shared material sets should cover gray, blue, red, gold.
- UI glow materials must avoid bloom clutter.

### 4.3 Animation

- Shared simple humanoid rig preferred for people.
- Clips should be short and loopable.
- Stress and quit animations must align with event timing.
- UI/card animations can be DOTween specs instead of skeletal clips.

### 4.4 Audio

- Short one-shot sounds for UI.
- Loopable ambience only where needed.
- No sound should mask event choice readability.
- Crisis alerts should be distinct but not fatiguing.

## 5. Collider and Anchor Rules

- Visual meshes and gameplay colliders should be separate when useful.
- Customers and staff use simple capsule colliders.
- Props use box colliders only if interaction/pathing needs it.
- Every business module that can receive a card must expose:
  - placement anchor
  - VFX anchor
  - camera/event anchor if relevant
  - linked slot ID

## 6. Prompt Workflow

For each final asset:

1. Pick Asset ID from `ASSET_MANIFEST.md`.
2. Pick prompt or brief ref from `ASSET_PROMPTS.md`.
3. Generate or brief concept.
4. Validate against BoardOverview readability.
5. Produce model/UI/VFX/audio.
6. Import with naming convention.
7. Create prefab or material.
8. Link in AssetManifestDefinition.

Prompt quality checklist:

- Subject is clear.
- Use case is stated.
- Toy Diorama style block included.
- Venture and gameplay state included.
- Variants listed.
- Avoid list included.
- Final deliverable format clear.

## 7. Readability QA

Test every P0/P1 asset from:

- BoardOverview camera.
- CardInspect camera.
- EventFocus camera if event-related.

Pass criteria:

- Role is identifiable.
- Ownership color is readable.
- State is readable: normal, busy, stressed, broken, empty.
- Asset does not hide customer flow.
- UI does not cover event source.
- VFX communicates pressure without visual noise.

## 8. Gameplay Coverage QA

Every asset batch must answer:

- Which card or event uses this asset?
- Which world state does it represent?
- Which slot or board anchor links to it?
- What is the placeholder if final asset is missing?
- What is the degraded fallback if VFX/audio is missing?

No final gameplay card should exist without:

- card frame,
- slot target,
- linked world manifestation,
- at least one feedback VFX or world label,
- fallback placeholder.

## 9. Batch Planning

### Batch A: Core Board

- District base board.
- Flow paths.
- Player/rival base.
- Toy Studio light rig.
- Customer tint materials.

### Batch B: First Turn Fast Food

- Fast Food cook, cashier, waiter.
- Grill, counter, tables, delivery shelf.
- Card offer band, staff slot, operation slot.
- Card commit and stress aura VFX.

### Batch C: Resolve and Event Feedback

- Customer color shift.
- Queue pressure.
- Review burst.
- Event panel.
- Staff quit animation.

### Batch D: Venture Expansion

- Cafe bar module and barista.
- Tech server/support module and staff.
- Giyim vitrin/rack/fitting room.
- Market shelf/fresh bin/veresiye book.

### Batch E: Meta and Polish

- Holding exit scene assets.
- Scale upgrade VFX/audio.
- Additional customer/staff variants.

## 10. Production Acceptance Criteria

- Asset manifest has no P0 gaps.
- P0 assets have placeholder and final target.
- P1 venture modules cover all five ventures.
- Prompt refs exist for every final target.
- Unity naming convention is consistent.
- Board remains readable under Toy Studio lighting.
- All generated or modeled assets support fixed isometric camera readability.
