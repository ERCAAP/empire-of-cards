# UI/UX & 3D Art Bible — Empire of Cards

> Version: 1.0  
> Date: 2026-05-22  
> Role: Presentation, UI/UX, 3D model, lighting, color, and game-feel source for implementation.  
> Gameplay truth remains `GDD.md` and the venture files under `businesses/`.

---

## 0. Purpose

This document defines how Empire of Cards should look, feel, read, and move.

The game is a real-life business simulation dressed as a physical card-table strategy game. The player should not feel like they are placing abstract buffs into colored rectangles. They should feel like they are building a small local business on a tabletop: hiring people, buying equipment, signing suppliers, absorbing crises, watching customers move, and reacting to a rival in the same neighborhood.

The current screenshots already have the correct structural idea:

- Player board at the bottom.
- Shared market in the center.
- Rival board at the top.
- Cards are 3D world objects.
- Venture selection exists.
- Naming flow exists.
- The board is already runtime-built through code.

The issue is presentation depth: too many flat rectangles, too little business identity, too much small text, and not enough physical feedback.

The target is:

> A readable 2.5D business war table where every card placement creates a visible business object, pressure, worker, token, or market movement.

---

## 1. Design Pillars

### 1.1 Business Simulation First

Every visible element must answer a real business question:

- Do I have enough capacity?
- Is my staff stable?
- Is the market flowing to me or the rival?
- Is my quality high enough?
- Am I taking illegal shortcuts?
- Is this growth sustainable?

The UI should avoid vague fantasy language. Use business language:

- Good: `Kitchen bottleneck`, `Payroll pressure`, `Supplier quality`, `Review storm`, `Checkout friction`.
- Bad: `+2 power`, `magic boost`, `enemy damage`, `mana gain`.

### 1.2 Physical Tabletop Feel

The game should feel like a premium board game set on a desk:

- Cards have thickness, shadows, bevels, hover lift, and snap-down impact.
- Slots are recessed trays, not flat colored panels.
- Market share is made of physical blocks/tokens.
- Business props are small tabletop miniatures.
- Staff are simple readable miniatures or standees.
- Events appear as paper slips, warning stamps, badges, fines, or crisis tokens.

### 1.3 Readability Beats Decoration

This is a strategy simulation. The player must understand the board quickly.

Priority order:

1. Current pressure.
2. Legal/rating/cash risk.
3. Which slots are filled.
4. Which market side is winning.
5. What the rival is doing.
6. Decorative business flavor.

No visual polish may hide board state.

### 1.4 Venture Identity Must Be Real

Venture choice cannot be a color swap. Fast Food, Cafe, Grocery, Tech App, and Clothing Store must have different silhouettes, objects, worker types, crisis tokens, and sound/lighting accents.

Phase 1 priority:

1. Fast Food
2. Cafe
3. Grocery Store / Market-Bakkal

Phase 2:

1. Tech App
2. Clothing Store

They are still defined here because the current game exposes them.

---

## 2. Current State Diagnosis

### 2.1 What Works

- The three-zone board matches the GDD.
- The current board already separates Operation, Staff, Marketing, Supplier, Temp Effect, Play, Sell, and Hand.
- `ControlDeskTheme` gives a shared palette.
- `Board3D` already builds physical board zones in world space.
- `CardFactory` already makes 3D card objects with world-space card faces.
- `VentureBoardThemeProfile` already supports venture-specific board props.
- The naming flow already supports Tech App category choice.

### 2.2 What Feels Weak

- The board reads as colored debug geometry rather than a business table.
- The venture selection screen is too text-heavy and crowded.
- The first impression is dark and flat.
- Slot labels are too small to carry meaning by themselves.
- Business identity is mostly color and text, not objects.
- Staff roles like waiter, busser/komi, cashier, manager, developer, and support are not visually embodied enough.
- Rival board is present but not emotionally readable as a competing business.
- Market share blocks are useful but need stronger customer-flow meaning.

### 2.3 Design Correction

Do not replace the architecture. Upgrade the presentation layer.

The existing runtime-built board should evolve like this:

```text
Colored lane + cube slot
→ recessed tabletop tray + icon/label
→ physical card dock + state lights
→ venture prop appears when card is played
→ tokens and VFX show why the state changed
```

---

## 3. Full UX Flow

## 3.1 Main Menu

The main menu should immediately sell the fantasy:

> "I am about to start a small business empire from a messy desk."

### Layout

- Background is a 3D desk scene viewed from a slight top-down angle.
- Center/top: `EMPIRE OF CARDS` logo as a physical brass/neon desk sign.
- Lower center: primary buttons.
- Right side: latest saved business dossier.
- Left side: small rotating venture props: burger tray, coffee cup, grocery crate, laptop, clothes rack.

### Visual Objects

- Ledger notebook.
- Business plan paper.
- Coins/cash chips.
- Customer tokens.
- Contract paper with supplier stamp.
- Small rival red marker in the distance.
- Desk lamp casting warm light.

### Buttons

Primary button order:

1. `NEW RUN`
2. `LOAD RUN`
3. `SETTINGS`
4. `QUIT`

Button design:

- Rectangular business UI buttons, not pill buttons.
- 4-8 px corner radius maximum.
- Clear hover state: light border + subtle scale.
- Click state: quick depress animation.

### Main Menu Color

- Background: very dark warm office brown/black.
- Logo: gold/cream.
- New Run: confident green.
- Load: desk blue.
- Delete/Quit: muted red/brown.

Main menu should not show long explanation text. The title, buttons, and desk scene should do the work.

---

## 3.2 Venture Selection

The current screen asks the right question but presents too much text. It should become a business dossier selection screen.

### Target Layout

Five venture tiles in a row on desktop:

```text
┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐
│ FAST FOOD  │ │ CAFE       │ │ TECH APP   │ │ CLOTHING   │ │ GROCERY    │
│ 3D preview │ │ 3D preview │ │ 3D preview │ │ 3D preview │ │ 3D preview │
│ pressure   │ │ pressure   │ │ pressure   │ │ pressure   │ │ pressure   │
└────────────┘ └────────────┘ └────────────┘ └────────────┘ └────────────┘
```

Each tile needs:

- Venture name.
- One 3D or rendered icon preview.
- One-line fantasy.
- Three pressure chips.
- First-turn suggestion.

### Text Length Rule

Each tile should fit without paragraph blocks.

Example:

```text
FAST FOOD
Rush traffic. Review risk. Kitchen pressure.

Pressure: Speed / Hygiene / Delivery
Open: Counter + Cook + Clean supply
```

### Selection Feedback

Selected tile:

- Raises like a physical board piece.
- Border glows with venture color.
- Tiny preview prop animates.
- Start button changes label:
  - `START FAST FOOD`
  - `START CAFE`
  - `START GROCERY`
  - `NAME YOUR APP`

### Venture Selection Background

Do not fully hide the board behind a dark overlay. Use the actual desk as context, blurred/dimmed only enough for readability.

---

## 3.3 Naming Flow

The naming modal should feel like signing the business registration paper.

### For Physical Businesses

Use:

- Paper form.
- Storefront sign preview.
- Small stamp animation when confirmed.

Copy:

```text
Name your company
This name appears on your storefront sign and save file.
```

Avoid long instructional paragraphs. Use one sentence plus a pressure summary.

### For Tech App

Use:

- Laptop screen / app store form.
- Input field as a product naming field.
- Category selection as app-market positioning.

Flow:

1. Name app.
2. Pick category.
3. Start first sprint.

Tech category buttons should show pressure types:

- Creative Tooling: workflow retention, subscription pressure.
- Health & Fitness: habit streaks, churn, trust.
- Lifestyle: social proof and content loops.
- AI Features: infra cost, safety, expectation pressure.
- Mobile Game: acquisition cost, reviews, retention.
- Hyper Casual: fast installs, low loyalty, ad pressure.

---

## 3.4 Active Board UX

At any time the player must read:

- Cash.
- Turn/season.
- Board pressure.
- Rating.
- Legal risk.
- Market share.
- Current brief/result.
- Available cards.
- Valid drop targets.
- Rival last move.

### Screen Zones

```text
┌──────────────────────────────────────────┐
│ Top HUD: cash / turn / pressure / rating │
├──────────────────────────────────────────┤
│ Rival board: visible rival build         │
├──────────────────────────────────────────┤
│ Shared market: customer flow + share     │
├──────────────────────────────────────────┤
│ Player board: business + slots           │
├──────────────────────────────────────────┤
│ Hand: cards + shop/deck/end turn         │
└──────────────────────────────────────────┘
```

### Top HUD

Keep it compact. It should be a business dashboard, not a fantasy HUD.

Required items:

- Cash.
- Tier/company stage.
- Turn + season.
- Pressure summary.
- Market share bar.
- Rating.
- Legal risk.

Optional expandable details:

- Cash flow.
- Payroll.
- Supplier costs.
- Tax/loan/insurance.
- Staff stability.

### Board Brief

The brief/report panel should be short and actionable:

- `PROBLEM`: what is wrong.
- `NEXT MOVE`: what type of card solves it.
- `WHY`: one sentence after resolution.

Example:

```text
PROBLEM  Kitchen cannot absorb demand.
NEXT MOVE  Play Cook, Kitchen Upgrade, or Clean Supply before more marketing.
```

### Hand UX

Cards should fan slightly but not overlap text. Hover should lift and enlarge enough to read. Dragging should:

- Raise card.
- Dim impossible slots.
- Pulse valid slots.
- Show one-line drop outcome near cursor.

Example:

```text
Drop on STAFF → +1.8 capacity, +0.8 stability, -$45 payroll
```

---

## 4. Board Layout Specification

## 4.1 Three Main Zones

### Player Zone

Purpose:

> "This is my actual business."

Visual language:

- Brightest readable zone.
- Warmest edge highlights.
- Most detailed props.
- Staff figures face toward operation lane.
- Cards and props show what the business has become.

### Shared Market Zone

Purpose:

> "This is the neighborhood/customer pool we are fighting over."

Visual language:

- Neutral tray in the center.
- Customer tokens flow left/right.
- Player-controlled blocks are blue.
- Rival-controlled blocks are red.
- Unclaimed blocks are warm gray.
- Movement should be visible after each turn.

Market zone should not look like a progress bar only. It should look like a contested street/customer lane.

### Rival Zone

Purpose:

> "The rival is making similar business decisions under partial information."

Visual language:

- Darker red/brown board.
- Fewer details than player zone but enough to read strategy.
- Rival card silhouettes and pressure markers.
- A `LAST CARD` or `RIVAL MOVE` display.
- Warning glow when rival takes market share.

Rival board should feel dangerous but not visually louder than player board.

---

## 4.2 Slot Families

### Operation

Meaning:

- Physical/core business capacity.
- Kitchen, bar, shelf, backend, display.

Visual:

- Blue/steel tray.
- Bigger slots.
- Strongest object props.
- Equipment appears here.

### Staff

Meaning:

- People who make the business work.

Visual:

- Green tray.
- Staff cards can spawn small standees/miniatures.
- Staff miniatures should stand near the operation they support.

### Marketing

Meaning:

- Demand generation.
- Visibility, local buzz, ads, influencers, loyalty.

Visual:

- Purple/magenta tray.
- Props: phone, flyer stack, ad megaphone, poster, neon sign.
- Effects should project toward market zone.

### Supplier

Meaning:

- Quality, cost, reliability, sourcing.

Visual:

- Amber/brown tray.
- Props: crate, contract, delivery box, invoice, cloud bill, fabric roll.
- Supplier cards should visibly connect to quality/rating.

### Temp Effect / Risk

Meaning:

- Crisis, scandal, illegal shortcut, temporary buff/debuff.

Visual:

- Orange/red vertical rail.
- Cards should feel like urgent stamps, warning papers, fines, notices.
- Active risk should pulse slowly.

### Play / Sell

Meaning:

- Utility zones.

Visual:

- Smaller, right-side rail.
- Play = strong red/orange.
- Sell = muted brown/cash.
- Use clear icons, not only text.

---

## 5. Shared Visual Style

## 5.1 Art Style

Target:

> Stylized low-poly + toon lighting + board-game physicality.

Rules:

- Low-poly, clean bevels, chunky silhouettes.
- No realistic grime.
- No tiny hyper-detailed objects.
- Use stylized readable props.
- Use non-black colored outlines or dark rim shadows.
- Keep all objects readable from top-down camera.

Good references by function:

- Balatro: card juice, readable reward feedback, confident color accents.
- Cultist Simulator: tabletop atmosphere and card layout mood.
- Overcooked-like stylization: simple business/kitchen objects readable from above.
- Monopoly-like business toy language: buildings, tokens, money, ownership.

Do not copy their art. Use them as functional references.

## 5.2 Shape Language

### Player

- Rectangular, stable, slightly rounded business trays.
- Blue/green ownership signals.
- Clean highlight edges.

### Rival

- Sharper, darker, red-tinted trays.
- More angular warning accents.
- Less inviting lighting.

### Market

- Neutral blocks, customer dots, flow arrows.
- Central tray should feel shared, not owned.

### Crisis

- Stamps, warning triangles, inspection papers, legal notices, red tape.
- Crisis visuals should be physically on the table.

---

## 6. Color System

## 6.1 Base Palette

Use the current `ControlDeskTheme` direction but increase contrast and material richness.

| Role | Color | Hex Target | Use |
|---|---|---:|---|
| Desk dark | Warm black-brown | `#151210` | Background and outer table |
| Felt surface | Charcoal brown | `#211C18` | Main board surface |
| Panel | Near black | `#141418` | HUD panels |
| Text primary | Warm cream | `#F0E8D4` | Main text |
| Text muted | Warm gray | `#AAA49A` | Secondary text |
| Money | Gold | `#FFD447` | Cash and reward |
| Player | Sky blue | `#4594F5` | Market/player ownership |
| Rival | Red | `#E43E3D` | Rival ownership |
| Success | Green | `#5DDE7A` | Valid, recovery |
| Warning | Amber | `#F5A33C` | Pressure |
| Danger | Red-orange | `#F05A42` | Crisis/legal |

## 6.2 Slot Colors

| Slot | Color | Hex Target | Meaning |
|---|---|---:|---|
| Operation | Blue steel | `#465D8A` | Capacity/core business |
| Staff | Green | `#4C7A56` | People/stability |
| Marketing | Plum | `#93599A` | Demand/visibility |
| Supplier | Ochre | `#9E783D` | Quality/cost |
| Temp Effect | Burnt orange | `#B96B38` | Risk/crisis |
| Play | Coral red | `#BA403D` | Immediate action |
| Sell | Brown | `#8B5634` | Cash-out |

## 6.3 Venture Accent Colors

| Venture | Accent | Secondary | Avoid |
|---|---|---|---|
| Fast Food | Tomato red/orange | Mustard yellow, steel | Looking like generic fire/magic |
| Cafe | Espresso brown | Cream, brass, plant green | Beige-only palette |
| Grocery | Fresh green | Produce red/yellow, fridge cyan | One-note green |
| Tech App | Cyan/blue | Graphite, signal green | Overly neon cyberpunk |
| Clothing | Rose/magenta | Charcoal, fabric cream, gold | Pure pink toy-store look |

Use venture accents as highlights, not full-screen washes.

---

## 7. Lighting & Rendering

## 7.1 Lighting Goal

The board should feel like a focused desk under a lamp:

- Warm key light from upper-left.
- Soft cool fill from front/screen side.
- Slight rim highlight on cards and props.
- Shadows under cards, tokens, and props.
- Background falls into darkness but board remains readable.

## 7.2 Recommended Unity Setup

Use URP with:

- One main directional/key light.
- One soft area/point desk lamp if performance allows.
- Ambient warm fill.
- Contact shadows for cards and props.
- Post-process volume for subtle bloom, vignette, and color adjustments.

Unity URP supports post-processing through Volume components and overrides, including bloom and color adjustments. Use these effects lightly; they should not soften card text or HUD text.

## 7.3 Post-Processing Rules

- Bloom only on money, legal warning, selected tile, and neon-like accents.
- Vignette subtle, never enough to hide corners of the board.
- No heavy depth of field during gameplay.
- No blur behind card text.
- Color grading should preserve warm desk identity and slot contrast.

## 7.4 Text Rendering

Use TextMeshPro SDF font assets for all scalable UI and card text. SDF is important because card text is transformed, scaled, and viewed in world space.

Required TMP setup:

- SDF font assets.
- Turkish character support: `Ç ç Ğ ğ İ ı Ö ö Ş ş Ü ü`.
- Outline material presets for card names and board labels.
- Drop shadow material presets for HUD text.
- Fallback font asset configured.

---

## 8. Typography

## 8.1 Font Roles

| Role | Recommended Font | Use |
|---|---|---|
| Main UI | Nunito or Inter | Menus, tooltips, body text |
| Card titles | Oswald / Barlow Condensed | Card names and lane headers |
| Numbers | Space Mono / JetBrains Mono | Cash, turn, stats |
| Big effects | Bangers / Bungee | Result, combo, crisis stamp |

## 8.2 Text Size Rules

At 1920x1080:

- Main menu title: 68-84 px.
- Menu buttons: 24-30 px.
- HUD labels: 12-16 px.
- HUD values: 20-34 px.
- Card title on hover: 28-34 px equivalent.
- Card body on hover: 16-20 px equivalent.
- Non-hover card text may be summarized; full reading happens on hover.

## 8.3 Contrast Rules

Important standard text should target at least 4.5:1 contrast against its background. Large text and large visual elements should target at least 3:1. Inactive text still needs enough contrast to be understood.

Do not place important text directly on busy 3D backgrounds without a dark backing panel.

---

## 9. Card Design

## 9.1 Card Anatomy

Physical card structure:

```text
┌────────────────────────────┐
│ Accent bar / family color  │
│ Card name         Cost chip│
│ Icon / business object     │
│ Role chip                  │
│ Short effect summary       │
│ Projected delta            │
│ Cost / upkeep / risk       │
└────────────────────────────┘
```

### Required Card Signals

- Card family.
- Slot target.
- Cost.
- Main effect.
- Risk/upkeep if any.
- Venture identity.

### Text Rule

Card front should not carry full rules text. It should carry decision text.

Good:

```text
Moves queue friction off the grill line.
+Capacity, +Stability, Payroll cost
```

Bad:

```text
When played into a valid Staff slot, this applies a capacity modifier...
```

## 9.2 Card Type Visuals

| Card Type | Shape/Signal | Color |
|---|---|---|
| Business/Operation | Equipment frame | Blue |
| Employee/Staff | Portrait/standee frame | Green |
| Marketing | Megaphone/phone/poster frame | Purple |
| Supplier | Contract/crate/invoice frame | Amber |
| Event/Crisis | Stamped paper frame | Orange/red |
| Risk/Illegal | Red tape / warning mark | Red-orange |
| Reaction | Repair/check mark | Green/blue |

## 9.3 Card Movement

### Draw

- Card comes from deck stack.
- Slight fan spread.
- Soft paper sound.

### Hover

- Card lifts 10-15 cm in world space.
- Scale 1.15-1.25.
- Glow outline.
- Full readable text state.

### Drag

- Card raises.
- Valid slots pulse.
- Invalid slots darken.
- Board preview line shows projected result.

### Drop

- Valid: quick snap, small bounce, thud, slot glow.
- Invalid: shake, red outline, return to hand.

---

## 10. Venture-Specific 3D Model Bible

## 10.1 Shared Model Standards

### Scale

Use consistent tabletop scale:

- Card: about 0.82 x 1.14 world units.
- Slot: card size plus 12-18% margin.
- Staff miniatures: small standees, 0.35-0.55 units tall.
- Equipment props: 0.3-0.9 units wide.
- Market tokens: small but chunky, readable from camera.

### Materials

Shared material families:

- `MAT_Table_DarkWood`
- `MAT_Felt_DarkWarm`
- `MAT_Card_Base`
- `MAT_Card_Edge`
- `MAT_Slot_Operation`
- `MAT_Slot_Staff`
- `MAT_Slot_Marketing`
- `MAT_Slot_Supplier`
- `MAT_Slot_Risk`
- `MAT_Token_Player`
- `MAT_Token_Rival`
- `MAT_Token_Neutral`
- `MAT_Money_Gold`
- `MAT_Prop_ActiveGlow`

### Geometry

- Use bevels on all visible cubes.
- Avoid razor-sharp primitive boxes.
- Use simple silhouettes: counter, crate, laptop, rack, fridge.
- Make top-down readability the main modeling test.

---

## 10.2 Fast Food

### Fantasy

High traffic, fast growth, kitchen pressure, hygiene risk, review volatility.

Player should feel:

> "I can grow fast, but every extra customer can break the kitchen."

### Core Business Model

Miniature fast food counter:

- Front counter.
- Grill/fryer block.
- Menu board.
- Small table cluster.
- Delivery bag corner.
- Trash/cleaning marker.

### Operation Props

| Sub-slot | Model |
|---|---|
| Kitchen | Grill plate, fryer, prep counter |
| Service | Cash counter, order screen |
| Seating | Small table set, chairs/stools |
| Delivery | Delivery bag, scooter helmet, order shelf |

### Staff Models

| Role | Visual |
|---|---|
| Cook / Asci | Apron, cap, spatula, stands near grill |
| Cashier / Kasiyer | Counter standee, small register |
| Courier / Kurye | Helmet, delivery backpack |
| Cleaner / Temizlik | Mop bucket, glove color accent |
| Branch Manager / Sube Muduru | Shirt/tie, clipboard |
| Waiter / Garson | Tray standee for service-heavy cards |
| Busser / Komi | Small tray/cart, clears tables |

Garson and komi should be distinct:

- Garson faces customer flow and carries orders.
- Komi faces tables/kitchen and clears bottlenecks.

### Supplier Props

- Meat crate / butcher stamp.
- Vegetable crate.
- Bread basket.
- Drink crate.
- Quality tag.

### Marketing Props

- Flyer stack.
- Google review star sign.
- Social media phone.
- Food app tablet.
- Street poster.

### Crisis Props

- Bad review cloud.
- Hygiene inspection clipboard.
- Broken fryer smoke.
- Delivery fee notice.
- Viral phone recording icon.

### Colors

- Main: tomato red/orange.
- Secondary: mustard yellow.
- Metal: dark steel.
- Cleanliness: white/blue accent.
- Danger: red-orange.

### VFX

- Steam from grill.
- Queue dots moving faster under demand pressure.
- Review stars shaking when rating drops.
- Red inspection stamp for hygiene crisis.

---

## 10.3 Cafe

### Fantasy

Quality, ritual, loyalty, ambience, slower premium growth.

Player should feel:

> "I am selling a daily habit, not just coffee."

### Core Business Model

Miniature cafe corner:

- Espresso bar.
- Grinder.
- Pastry display.
- Two small tables.
- Chalkboard sign.
- Plant or lamp.

### Operation Props

| Sub-slot | Model |
|---|---|
| Bar | Espresso machine, grinder |
| Seating | Small table, chair, plant |
| Takeaway | Cup stack, pickup shelf |
| Vitrin/Pastane | Pastry glass case |

### Staff Models

| Role | Visual |
|---|---|
| Barista | Apron, cup, latte art |
| Cashier | Register, receipt roll |
| Floor Staff | Tray, table cloth |
| Cleaner | Spray bottle, towel |
| Shift Lead | Clipboard, headset |

### Supplier Props

- Coffee bean sack.
- Milk crate.
- Pastry box.
- Ambience partner: plant, speaker, framed art.

### Marketing Props

- Instagram phone.
- Reels camera/spotlight.
- Loyalty stamp card.
- Google Maps pin.

### Crisis Props

- Burnout icon over barista.
- Empty milk crate.
- Broken WiFi router.
- Slow service complaint note.
- Music license warning.

### Colors

- Main: espresso brown.
- Secondary: cream and brass.
- Accent: soft green.
- Warning: amber.

### VFX

- Coffee steam.
- Small heart/latte art pop on quality success.
- Loyalty stamp animation.
- Soft warm glow for rating recovery.

---

## 10.4 Grocery Store / Market-Bakkal

### Fantasy

Low margin, repeat traffic, freshness pressure, neighborhood trust.

Player should feel:

> "I earn little per sale, but the neighborhood can come back every day."

### Core Business Model

Miniature neighborhood market:

- Shelf row.
- Checkout counter.
- Fridge.
- Produce crate.
- Bread basket.
- WhatsApp order phone.

### Operation Props

| Sub-slot | Model |
|---|---|
| Shelf / Raf | Product shelf, cans, boxes |
| Fresh Produce / Taze Urun | Produce crate, scale |
| Checkout / Kasa | Register, scanner |
| Neighborhood Delivery / WhatsApp | Phone stand, delivery basket |

### Staff Models

| Role | Visual |
|---|---|
| Cashier | Register standee |
| Shelf Worker / Reyon | Box/cart |
| Fresh Produce Lead | Apron, scale, produce basket |
| Courier | Delivery basket |
| Shift Worker / Vardiya | Key ring, simple vest |

### Supplier Props

- Wholesale crate.
- Distributor box.
- Dairy/bakery crate.
- Drink partner fridge sticker.

### Marketing Props

- WhatsApp phone bubble.
- Neighborhood poster.
- Loyalty card.
- Night-open neon sign.

### Crisis Props

- Broken fridge.
- Spoiled produce token.
- Expiry-date warning.
- Unpaid ledger/veresiye notebook.
- Power outage icon.
- Chain-market threat sign.

### Colors

- Main: fresh green.
- Secondary: produce red/yellow.
- Cold chain: cyan.
- Local trust: warm cream.

### VFX

- Freshness glow on produce.
- Expiry warning pulse.
- Small basket tokens returning each turn.
- Fridge flicker during cold-chain crisis.

---

## 10.5 Tech App

### Fantasy

Slow opening, explosive upside, backend stability, reviews, churn, infra cost, category pressure.

Player should feel:

> "Growth is dangerous until the product can survive it."

### Core Business Model

Miniature startup desk:

- Laptop with app screen.
- Phone mockup.
- Server stack.
- Router/cloud block.
- Sticky notes.
- Bug tokens.

### Operation Props

| Sub-slot | Model |
|---|---|
| Product | Phone/app tile, feature cards |
| Backend | Server rack, database block |
| Growth Pipeline | Funnel board, analytics chart |
| Support/Platform Ops | Headset desk, ticket stack |

### Staff Models

| Role | Visual |
|---|---|
| Developer | Hoodie/laptop standee |
| Designer | Tablet/pen standee |
| Growth | Chart/megaphone standee |
| Support | Headset standee |
| Product Manager | Roadmap board |

### Supplier Props

- Cloud bill.
- Tooling subscription.
- Payment/analytics API block.
- Partner integration plug.

### Marketing Props

- ASO store card.
- Performance ads dashboard.
- Influencer phone.
- Community chat bubble.

### Crisis Props

- Crash warning.
- Bug token.
- Server fire/overload icon.
- Security breach lock.
- Review bomb.
- Churn leak token.

### Colors

- Main: cyan/blue.
- Secondary: dark graphite.
- Success: signal green.
- Warning: magenta/amber.

### VFX

- Data pulse toward market.
- Server heat warning.
- Bug token bounce.
- Review stars flicker.

Important: Tech App can look digital, but it must remain tabletop. Avoid turning the whole board into a sci-fi HUD.

---

## 10.6 Clothing Store

### Fantasy

Seasonal demand, visual display, stock discipline, fit/return pressure.

Player should feel:

> "The storefront must pull people in, but bad stock timing kills margin."

### Core Business Model

Miniature boutique:

- Display window.
- Clothing rack.
- Folded table.
- Fitting mirror.
- Checkout.
- Return box.

### Operation Props

| Sub-slot | Model |
|---|---|
| Display / Vitrin | Mannequin, window platform |
| Shelf/Stock | Folded clothes, rack |
| Checkout / Kasa | Register counter |
| Online Order | Package box, shipping label |

### Staff Models

| Role | Visual |
|---|---|
| Sales Associate | Hanger/tablet |
| Cashier | Register standee |
| Tailor | Measuring tape |
| Stockroom Worker | Box/cart |
| Store Manager | Clipboard, key ring |

### Supplier Props

- Wholesale box.
- Atelier fabric roll.
- Cargo box.
- Photo shoot light/camera.

### Marketing Props

- Instagram phone.
- Influencer ring light.
- Discount tag.
- Shopping ads screen.

### Crisis Props

- Return pile.
- Bad fit complaint.
- Seasonal mismatch tag.
- Empty stock box.
- Damaged fabric.

### Colors

- Main: rose/magenta.
- Secondary: charcoal.
- Premium accent: gold.
- Fabric base: cream.

### VFX

- Trend sparkle.
- Return warning stamp.
- Stock mismatch pulse.
- Display spotlight.

---

## 11. Workers & Character Direction

Staff models should be simple tabletop standees, not full realistic characters.

### Shared Character Rules

- Big readable head/torso silhouette.
- Simple job prop in hand.
- Color-coded role accent.
- No tiny facial detail required.
- Idle bob or small breathing animation.
- When overloaded, show a small pressure icon above them.

### Role Categories

| Category | Examples | Visual Language |
|---|---|---|
| Service | Waiter, cashier, sales associate | Customer-facing pose |
| Production | Cook, barista, developer, tailor | Tool/equipment-facing pose |
| Support | Cleaner, support agent, stock worker | Utility prop |
| Logistics | Courier, delivery worker | Bag/helmet/basket |
| Management | Branch manager, shift lead, PM | Clipboard/roadmap |

### Morale/Burnout Visuals

Do not add separate complex UI first. Use small overlays:

- Happy/steady: small green check pulse.
- Tired: amber sweat/clock icon.
- Burnout: red cracked icon.
- Poaching risk: rival-red handshake/arrow marker.

---

## 12. Buildings & Business Growth

The "business" should grow visually as the run progresses.

### Stage 1 — Small Local Setup

- One counter/table/shelf/laptop.
- Few props.
- Sparse tokens.
- Small sign with run name.

### Stage 2 — Working Business

- More equipment.
- Staff standees.
- Supplier props.
- More visible customer flow.
- Sign becomes cleaner/brighter.

### Stage 3 — Dominating Local Brand

- Expanded board footprint.
- More polished signage.
- Multiple props active.
- Market share mostly player-colored.
- Rival board visibly under pressure.

This does not require huge 3D buildings. It can be tabletop diorama growth.

---

## 13. Market & Customer Visualization

The shared market is the emotional center of the game.

### Customer Tokens

Use three ownership states:

- Neutral gray: undecided/free customers.
- Player blue: loyal/converted to player.
- Rival red: pulled by rival.

### Customer Segment Tokens

Later, customer segments can use small icon stamps:

- Student: backpack.
- Family: group icon.
- Worker: lunchbox.
- Tourist: camera.
- Regular: loyalty card.

### Movement

After turn resolve:

- Tokens slide toward the winner.
- Market blocks flip/change color.
- Rating loss should make tokens hesitate or move away.
- Rival move should visibly steal a few tokens.

### No Invisible Math

If demand, quality, and rating changed market share, show cause:

```text
Service too slow → rating down → 1 customer block drifted to rival
```

---

## 14. Event, Crisis, Legal Risk

Risk needs to feel like paperwork and public pressure, not just a number.

### Legal Risk Visuals

- Legal risk bar at top right.
- Red tape slowly appears around risk/illegal cards.
- Inspection stamp lands on board when triggered.
- Warning paper appears in Temp Effect rail.

### Crisis Types

| Type | Physical Token |
|---|---|
| Review storm | Bad review card / phone screen |
| Inspection | Clipboard / official stamp |
| Supplier failure | Broken crate / late delivery note |
| Staff crisis | Burnout marker / empty shift card |
| Cash pressure | Invoice stack / red bill |
| Tech crash | Bug token / warning screen |
| Grocery spoilage | Spoiled produce / expiry stamp |

### Crisis UX

Every crisis should show:

- What happened.
- Why it happened.
- What card family solves it.
- Whether the clean solution or risky solution exists.

---

## 15. Animation & Game Feel

## 15.1 Core Timing

| Action | Timing | Feel |
|---|---:|---|
| Card hover | 0.10-0.15s | Crisp lift |
| Card draw | 0.30-0.45s | Fan into hand |
| Card drop | 0.12-0.20s | Snap + thud |
| Invalid drop | 0.20s | Shake + return |
| Market token move | 0.45-0.70s | Board-piece slide |
| Income count | 0.50-0.90s | Cash register tick |
| Crisis stamp | 0.25-0.40s | Heavy impact |
| Rival move | 0.55-0.90s | Suspense + red pulse |

## 15.2 Feedback Rules

Every meaningful action needs:

- Visual change.
- Short sound.
- Board state update.
- One-line explanation when needed.

Example:

```text
Player plays Senior Barista
→ card snaps to Staff
→ barista standee appears near espresso bar
→ quality/rating preview pulses
→ staff payroll chip updates
```

---

## 16. Sound Direction

Sound should support tabletop business pressure.

### Shared SFX

- Card draw: paper slide.
- Card place: soft cardboard thud.
- Card invalid: muted tap/shake.
- Money gain: cash register + coin.
- Market shift: wooden token click.
- Crisis: stamp impact.
- Legal risk: warning beep/stamp.
- Rival move: low red pulse.

### Venture SFX

| Venture | Sounds |
|---|---|
| Fast Food | grill sizzle, fryer pop, order bell |
| Cafe | espresso steam, cup clink, stamp card |
| Grocery | scanner beep, fridge hum, bag rustle |
| Tech App | notification blip, server hum, bug alert |
| Clothing | hanger slide, register, fabric rustle |

Use short sounds. Avoid noisy loops during decision-making.

---

## 17. Accessibility & UX Safety

### Contrast

Important standard text should target 4.5:1 contrast. Large text can target 3:1 minimum. Do not rely on subtle gray-on-dark combinations for important info.

### Color Blind Safety

Do not use only color to distinguish:

- Player vs rival.
- Slot type.
- Valid vs invalid.
- Legal danger.

Add:

- Icons.
- Shape differences.
- Motion.
- Text labels.
- Border patterns.

### Text Scale

Settings should eventually include:

- UI scale.
- Card text scale.
- High contrast mode.
- Reduced motion.
- Disable camera shake.

### Readability Checks

Every screen must pass:

- Can the player read the main decision in 3 seconds?
- Can the player identify legal/rating/cash danger without squinting?
- Can the player distinguish player/rival market ownership without color alone?
- Can the player understand a card's slot target before dragging it?

---

## 18. Implementation Roadmap

## 18.1 P1 — Fix First Impression and Board Readability

1. Replace flat venture cards with venture dossier tiles.
2. Add richer desk/table material language to `Board3D`.
3. Add bevel/rounded meshes or model prefabs for cards, slots, tokens, and board trays.
4. Upgrade `ControlDeskTheme` colors for higher text contrast.
5. Add Phase 1 venture props:
   - Fast Food: counter, grill/fryer, tables, delivery bag.
   - Cafe: espresso bar, pastry case, tables, bean sack.
   - Grocery: shelf, checkout, fridge, produce crate.
6. Add clearer hover/drop states.
7. Improve card face hierarchy.
8. Make market-share blocks feel like customer tokens/ownership pieces.

## 18.2 P2 — Staff and Crisis Identity

1. Add staff standees for Phase 1 roles.
2. Add crisis tokens:
   - Hygiene inspection.
   - Review storm.
   - Barista burnout.
   - Bean shortage.
   - Fridge failure.
   - Expiry warning.
3. Add main menu 3D desk scene.
4. Add venture-specific SFX.
5. Add result animations for market movement.

## 18.3 P3 — Steam Polish

1. Final menu background and logo treatment.
2. Better post-processing and lighting.
3. Full card icon set.
4. More animated props.
5. High contrast/accessibility settings.
6. Steam capsule-aligned branding.

---

## 19. Implementation Notes for Current Codebase

Current relevant files:

- `Assets/_EmpireOfCards/Scripts/Presentation/ControlDeskTheme.cs`
- `Assets/_EmpireOfCards/Scripts/World/Board3D.cs`
- `Assets/_EmpireOfCards/Scripts/World/CardFactory.cs`
- `Assets/_EmpireOfCards/Scripts/UI/VentureSelectionUI.cs`
- `Assets/_EmpireOfCards/Scripts/Bootstrap/HUDBuilder.cs`
- `Assets/_EmpireOfCards/Scripts/Bootstrap/Data/V4ContentFactory.cs`
- `Assets/_EmpireOfCards/Scripts/Data/VentureProfiles.cs`

Recommended technical path:

1. Keep runtime-built board for now.
2. Introduce reusable mesh/prefab helpers gradually.
3. Add 3D model prefabs only after the shape language is stable.
4. Use `VentureBoardThemeProfile.props` as the bridge between gameplay cards and visual business props.
5. Do not hardcode venture-specific visuals directly into unrelated managers.
6. Keep inter-manager communication through `EventBus`.
7. Keep CardData immutable at runtime.

---

## 20. Acceptance Criteria

A screen or model pass is successful only if:

- The venture is identifiable without reading the title.
- Player/rival/neutral market ownership is obvious.
- Cards are readable on hover.
- Slot target is obvious before dragging.
- Legal risk and rating pressure are visible.
- A new player can understand the current problem from the board brief.
- Phase 1 ventures feel like different businesses, not reskins.
- The board still fits 1920x1080, 1366x768, and 1280x800.
- Text does not overlap or depend on tiny labels.
- The game still feels like a serious business sim, not a generic toy board.

---

## 21. Reference Sources

These references informed presentation and accessibility decisions:

- Unity URP post-processing documentation: https://docs.unity.cn/6000.1/Documentation/Manual/urp/post-processing-bloom.html
- TextMeshPro SDF font documentation: https://docs.unity.cn/Packages/com.unity.textmeshpro%404.0/manual/FontAssetsSDF.html
- Microsoft Xbox Accessibility Guideline 102, contrast: https://learn.microsoft.com/en-us/gaming/accessibility/xbox-accessibility-guidelines/102
- Microsoft Xbox Accessibility Guideline 103, multiple cue channels: https://learn.microsoft.com/en-us/gaming/accessibility/xbox-accessibility-guidelines/103
- Balatro Steam page, card-game presentation reference: https://store.steampowered.com/app/2379780/Balatro/
- Cultist Simulator Steam page, tabletop/card atmosphere reference: https://store.steampowered.com/app/718670/Cultist_Simulator/

