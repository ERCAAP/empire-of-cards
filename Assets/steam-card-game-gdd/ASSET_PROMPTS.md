# Empire of Cards Asset Prompts

> Prompt pack for AI concept generation and 3D artist production briefs. All prompts must preserve the Toy Diorama visual target and fixed isometric gameplay readability.

## 1. Global Toy Diorama Style Prompt

Use this block at the top of all image-generation prompts:

```text
Use case: stylized-concept
Asset type: game production concept for a Unity 3D strategy game
Style: Toy Diorama, stylized 3D, polished indie game, miniature tabletop scale, readable silhouette, warm Toy Studio lighting, soft shadows, clean shapes, orthographic/isometric-friendly design, no photorealism.
Visual language: playful but strategic business simulation, clear gameplay states, small toy-like people and props, matte toy plastic and light cardboard/wood board materials, blue player accents, red rival accents, gray neutral market accents.
Camera/view: fixed isometric BoardOverview readability unless the asset is a UI element or isolated prop.
Quality goal: final asset must be readable from a medium-distance isometric camera and support gameplay feedback.
```

## 2. Global Negative Prompt / Avoid List

Use this for all visual prompts:

```text
Avoid: photorealistic humans, realistic food photography, real brands, copyrighted logos, fantasy combat, weapons, monsters, dark cinematic lighting, horror mood, excessive clutter, tiny unreadable UI text, beige-only palette, heavy gradients, overly glossy materials, realistic dirt, grunge, complex background, dramatic poster composition, marketing landing-page layout.
```

## 3. AI Concept Prompt Templates

### PROMPT_CHAR_CUSTOMER

```text
Primary request: Design a small toy diorama customer character set for Empire of Cards.
Subject: neutral/customer-owned miniature people used in a business district simulation.
Variants: neutral gray, player blue, rival red, loyal gold-accent; moods happy, impatient, angry, leaving.
Gameplay readability: ownership color must be clear from isometric camera distance; silhouette must remain readable in a crowd.
Deliverable: character lineup concept sheet, front 3/4 view, simple poses, no background clutter.
```

### PROMPT_CHAR_STAFF_FASTFOOD

```text
Primary request: Design toy diorama fast food staff characters.
Subject: cook, cashier, waiter, courier for a small fast food shop.
Variants: normal, working, stressed, quitting/walkout.
Props: apron, hat, tray, small delivery bag; no real logos.
Gameplay readability: role must be identifiable by silhouette and prop.
Deliverable: character lineup, 3/4 isometric-friendly view, clean shapes.
```

### PROMPT_CHAR_STAFF_CAFE

```text
Primary request: Design toy diorama cafe staff characters.
Subject: barista and floor staff for a cozy cafe.
Variants: normal, latte-making, carrying tray, burnout/stressed.
Props: cup, apron, small towel, tray.
Gameplay readability: barista and floor staff must be distinct from fast food staff.
Deliverable: character lineup concept sheet.
```

### PROMPT_CHAR_STAFF_TECH

```text
Primary request: Design toy diorama tech startup staff characters.
Subject: developer, UX designer, support operator.
Variants: normal work, crunch, stressed, burnout.
Props: laptop, tablet/sketch pad, headset, ticket bubble.
Gameplay readability: each role must be identifiable at small scale.
Deliverable: character lineup with desk prop hints.
```

### PROMPT_CHAR_STAFF_GIYIM

```text
Primary request: Design toy diorama clothing store staff characters.
Subject: sales advisor, tailor, stock/depot worker.
Variants: advising customer, measuring, carrying clothes, stressed.
Props: tape measure, clothing hanger, folded clothes.
Gameplay readability: fashion retail identity without real brands.
Deliverable: character lineup concept sheet.
```

### PROMPT_CHAR_STAFF_MARKET

```text
Primary request: Design toy diorama neighborhood market staff characters.
Subject: cashier, shelf worker, fresh produce worker, courier.
Variants: normal, stocking, delivering, tired night shift.
Props: small vest, basket, grocery bag, price label.
Gameplay readability: neighborhood market role clarity.
Deliverable: character lineup concept sheet.
```

### PROMPT_CHAR_RIVAL

```text
Primary request: Design a rival business representative character.
Subject: small toy diorama manager with red accent, used for rival actions.
Variants: discount campaign, premium quality, aggressive marketing, crisis.
Gameplay readability: should signal competitor pressure, not villain/combat.
Deliverable: 3/4 character concept with small signage prop.
```

### PROMPT_CHAR_META

```text
Primary request: Design meta-event business NPCs.
Subject: inspector and investor for legal audit and holding exit events.
Variants: inspector with clipboard, investor with offer document.
Gameplay readability: serious business role, still toy diorama style.
Deliverable: small character concept pair.
```

### PROMPT_PROP_SHARED

```text
Primary request: Design shared toy diorama business props.
Subject: cash register, queue markers, business sign, generic service counter.
Variants: normal, busy, warning/pulse state.
Gameplay readability: each prop must act as a clear gameplay anchor.
Deliverable: isolated prop sheet, isometric 3/4 view, simple materials.
```

### PROMPT_PROP_FASTFOOD

```text
Primary request: Design fast food toy diorama props.
Subject: grill, counter, tables, delivery shelf, ingredient crates.
Variants: clean, busy, overloaded, risky/cheap supplier.
Gameplay readability: kitchen pressure and delivery backlog must be visually obvious.
Deliverable: prop sheet with scale consistency.
```

### PROMPT_PROP_CAFE

```text
Primary request: Design cafe toy diorama props.
Subject: espresso machine, bar counter, pastry display, seating, takeaway window.
Variants: calm, busy, premium, overloaded.
Gameplay readability: cozy cafe identity and bar bottleneck must be clear.
Deliverable: prop sheet, warm Toy Studio lighting.
```

### PROMPT_PROP_TECH

```text
Primary request: Design tech app toy diorama props.
Subject: desk pods, laptops, server nodes, support desk, app rating panel.
Variants: stable, hot/crashing, ticket backlog, fake review glitch.
Gameplay readability: app/system states must read without text-heavy UI.
Deliverable: prop sheet for a tabletop startup office.
```

### PROMPT_PROP_GIYIM

```text
Primary request: Design clothing store toy diorama props.
Subject: window display, clothing racks, fitting room, checkout, returns desk.
Variants: stocked, empty, viral demand, return pressure.
Gameplay readability: stock shortage and returns must be visible.
Deliverable: isometric prop sheet, no real fashion brands.
```

### PROMPT_PROP_MARKET

```text
Primary request: Design neighborhood market toy diorama props.
Subject: shelves, fresh produce bins, fridge, veresiye book, delivery area.
Variants: full stock, low stock, spoiled/SKT risk, cash pressure.
Gameplay readability: small market identity and cash/stock risks must be clear.
Deliverable: isometric prop sheet.
```

### PROMPT_ENV_DISTRICT

```text
Primary request: Design the shared district board environment.
Subject: tabletop diorama market district with paths for neutral gray, player blue, and rival red customers.
Components: base board, customer paths, spawn nodes, trend/event spots.
Gameplay readability: customer flow and market share must be readable before decoration.
Deliverable: wide isometric board concept.
```

### PROMPT_ENV_PLAYER

```text
Primary request: Design player business base modules for all ventures.
Subject: modular toy diorama business footprint with blue accent and slot board compatibility.
Variants: fast food, cafe, tech office, clothing store, market.
Gameplay readability: each venture must share layout logic but feel distinct.
Deliverable: five small isometric module concepts.
```

### PROMPT_ENV_RIVAL

```text
Primary request: Design rival business base modules.
Subject: competing toy diorama businesses with red accents.
Variants: discount, premium, aggressive marketing, crisis state.
Gameplay readability: rival action must be readable at top board area.
Deliverable: compact isometric concepts.
```

### PROMPT_ENV_META

```text
Primary request: Design holding scale / exit backdrop.
Subject: toy diorama business growing into a holding, brand sign expanding, investor event stage.
Gameplay readability: should feel like scale progression, not a separate cinematic poster.
Deliverable: wide scale-stage concept.
```

### PROMPT_CARD_FRAME

```text
Primary request: Design card frame system for Empire of Cards.
Subject: five card frame types: Install, Burst, Policy, Risk, Reaction.
Style: toy board-game compatible but clean digital UI, no ornate fantasy card style.
Variants: venture tint support, empty art area, icon area, cost area, slot target marker.
Gameplay readability: frames must remain readable in bottom offer band and small slot board.
Deliverable: UI card frame sheet.
```

### PROMPT_CARD_ICONS

```text
Primary request: Design simple behavior icons for card types.
Subject: Install, Burst, Policy, Risk, Reaction icons.
Style: clean geometric icons, toy diorama UI, no text required.
Deliverable: icon sheet on plain background.
```

### PROMPT_SLOT_FRAME

```text
Primary request: Design slot frame system.
Subject: Operation, Staff, Marketing, Supplier, Temp Effect slot frames.
States: empty, valid target, filled, blocked, stressed/warning.
Gameplay readability: target highlight must be clear during card drag.
Deliverable: slot UI frame sheet.
```

### PROMPT_CARD_LINK

```text
Primary request: Design card-to-world link highlight.
Subject: subtle line/glow connecting a slot card to a linked NPC/object/flow.
Variants: player blue, rival red, warning orange/red.
Gameplay readability: must not clutter the board.
Deliverable: VFX/UI concept sheet.
```

### PROMPT_UI_HUD

```text
Primary request: Design compact gameplay HUD icons.
Subject: cash, rating stars, staff stability, legal risk, market share, turn pressure.
Style: toy diorama strategy UI, clean and readable, not corporate dashboard.
Deliverable: HUD icon and small meter concept sheet.
```

### PROMPT_UI_CARD_OFFER

```text
Primary request: Design bottom card offer band and card inspect tooltip.
Subject: 3-5 cards shown over a live isometric board, selected card lifted, target slot highlighted.
Gameplay readability: card offer is not a separate screen; it overlays the live board without blocking district flow.
Deliverable: gameplay UI mockup.
```

### PROMPT_UI_SLOT

```text
Primary request: Design slot replacement UI.
Subject: small context panel with Replace, Upgrade, Merge, Discard/Sell actions.
Gameplay readability: player understands consequence without long text.
Deliverable: UI panel mockup over toy diorama board.
```

### PROMPT_UI_EVENT

```text
Primary request: Design micro-event choice panel.
Subject: event problem near world anchor, 1-3 choice buttons with tradeoff labels.
Gameplay readability: panel must not cover the event source.
Deliverable: gameplay UI mockup.
```

### PROMPT_UI_SUMMARY

```text
Primary request: Design compact end turn summary.
Subject: market share delta, rating delta, cash delta, next pressure, rival action.
Gameplay readability: visual-first, short, returns to board quickly.
Deliverable: UI panel concept.
```

### PROMPT_UI_VENTURE

```text
Primary request: Design venture select cards.
Subject: five venture cards: Fast Food, Cafe, Tech App, Giyim, Market/Bakkal.
Gameplay readability: each card shows core pressure, starting difficulty, and visual identity.
Deliverable: venture select UI mockup.
```

### PROMPT_UI_META

```text
Primary request: Design holding exit/meta screen.
Subject: final market share, holding value, exit reward, next startup advantage.
Style: celebratory Toy Diorama, clean UI, not a marketing poster.
Deliverable: meta progression screen concept.
```

### PROMPT_VFX_CORE

```text
Primary request: Design core gameplay VFX concepts.
Subject: customer color shift, marketing pull, review burst, stress aura, queue pressure, rating crack/glow.
Gameplay readability: small, clear, not noisy, visible from isometric overview.
Deliverable: VFX concept sheet with labeled states.
```

### PROMPT_VFX_RISK

```text
Primary request: Design risk and legal VFX concepts.
Subject: fake review glitch, legal marker, audit warning, risk aura.
Style: business-risk visual language, no horror, no magic.
Deliverable: VFX concept sheet.
```

### PROMPT_VFX_META

```text
Primary request: Design scale upgrade VFX.
Subject: startup to local favorite, chain/platform, holding candidate upgrade pulses.
Gameplay readability: short celebratory effect, does not block board.
Deliverable: VFX concept sheet.
```

## 4. 3D Production Brief Templates

### BRIEF_3D_PROP

```text
Asset ID:
Category:
Venture:
Gameplay purpose:
Scale: must fit Toy Diorama board and fixed isometric camera.
Silhouette: readable from BoardOverview camera.
Pivot: bottom center unless interactive point requires another pivot.
Materials: Toy plastic / matte board / UI glow as specified.
States: normal, active, stressed/broken/empty if needed.
Collider: simple box/capsule; gameplay colliders separate from visual mesh.
Animation hooks: list moving parts or VFX anchor points.
LOD: one simplified LOD optional; prioritize clean silhouette.
Export: FBX/GLB, named EOC_[Category]_[Venture]_[AssetName]_[Variant].
```

### BRIEF_3D_CHARACTER

```text
Asset ID:
Role:
Venture:
Rig: shared simple humanoid rig preferred.
Scale: customer and staff should share consistent toy scale.
Silhouette: role must read from isometric overview.
Tint zones: ownership color or role accent must be material-driven.
Required animations: idle, walk, work/serve, wait, stress, quit/leave.
Props: attach points for tray, phone, laptop, package, clipboard.
Export: rigged FBX with animation clips or separate clips.
```

### BRIEF_3D_UI

```text
Asset ID:
UI context:
Screen/panel:
States:
Resolution: vector/source preferred where possible.
Interaction states: normal, hover, selected, disabled, warning.
Animation hooks: scale pulse, glow, snap, error shake.
Readability: must work over Toy Diorama background.
```

### BRIEF_MAT_CUSTOMER

```text
Material set: customer ownership.
Colors: neutral gray, player blue, rival red, loyal gold accent.
Rules: colors must remain readable under Toy Studio light; avoid neon saturation and muddy dark values.
Shader: URP Lit or simple custom tint shader.
```

### BRIEF_MAT_TOY

```text
Material set: toy plastic.
Finish: matte/satin, soft highlight, low texture noise.
Usage: characters, props, business modules.
Rules: no photoreal skin pores, no realistic grime, no heavy roughness noise.
```

### BRIEF_MAT_BOARD

```text
Material set: tabletop board/cardboard/wood.
Finish: warm matte, light stylized grain optional.
Rules: must not dominate beige palette; keep paths and ownership colors readable.
```

### BRIEF_MAT_UI

```text
Material set: UI glow accents.
Colors: player blue, rival red, warning orange/red, recovery gold.
Rules: glow must communicate state without bloom clutter.
```

### BRIEF_LIGHT_TOY

```text
Lighting rig: Toy Studio.
Base: warm directional light, soft shadows, ambient fill.
Accent: blue player side, red rival side, warm neutral district.
Crisis: local highlight only, no global darkening.
MVP: no day/night cycle.
```

### BRIEF_ANIM_CUSTOMER

```text
Animation family: customer.
Clips: walk, wait calm, wait impatient, angry leave, happy review, negative review.
Rules: loops short, readable from isometric camera, no exaggerated realism.
```

### BRIEF_ANIM_STAFF

```text
Animation family: staff.
Clips: idle, work, serve, stress medium, stress high, quit/walkout, celebrate/recover.
Rules: role prop should stay readable; stress animation should work with stress aura VFX.
```

### BRIEF_ANIM_UI

```text
Animation family: card/UI.
Clips or tween specs: hover lift, drag, snap to slot, invalid shake, replacement pop, stat pulse.
Rules: quick and tactile, no long animation delays.
```

### BRIEF_ANIM_EVENT

```text
Animation family: micro-event.
Use: event intro/outro, NPC approach, customer complaint, rival signal.
Rules: 3-8 second event window, synced to EventFocus camera.
```

### BRIEF_AUDIO_UI

```text
Audio family: UI/card.
Tone: light toy snap, clean, short, non-fatiguing.
Variants: hover, commit, replace, invalid, cash gain/loss.
```

### BRIEF_AUDIO_EVENT

```text
Audio family: events.
Tone: business tension, not horror/alarm-heavy.
Variants: crisis start, bad review, positive review, staff stress.
```

### BRIEF_AUDIO_AMBIENCE

```text
Audio family: ambience.
Tone: subtle crowd/shop ambience, loopable, low distraction.
Variants: calm, busy, angry, rival pull.
```

### BRIEF_AUDIO_META

```text
Audio family: meta/scale.
Tone: short celebratory business success.
Variants: local favorite, chain/platform, holding exit.
```

### BRIEF_DEBUG

```text
Debug helper:
Use: editor-only camera anchors, flow paths, event anchors, slot links.
Rules: not included in final player-facing build; clear gizmo colors.
```
