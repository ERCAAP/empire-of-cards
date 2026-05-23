# UI/UX & 3D Prompt Pack — Empire of Cards

> Version: 1.0  
> Date: 2026-05-22  
> Use with: image generation, concept art, 3D model briefs, Blender blocking, Figma/UI mockups, Steam page visuals, and in-game presentation tasks.  
> Companion doc: `UI_UX_3D_ART_BIBLE.md`

---

## 0. How To Use These Prompts

Each prompt is written to be copied into a visual generation tool or handed to an artist/modeler. Keep the `Global Style Block` attached to every prompt unless the prompt says otherwise.

For image generation:

1. Use the `Global Style Block`.
2. Add the specific screen/model prompt.
3. Add the `Global Negative Prompt`.
4. For UI screens, request 16:9, 1920x1080.
5. For isolated assets, request orthographic or three-quarter view on a transparent/simple background.

For 3D modeling:

1. Use the `3D Model Production Rules`.
2. Use the relevant model prompt.
3. Keep silhouettes readable from a top-down Unity camera.
4. Avoid tiny detail that cannot be seen during gameplay.

---

## 1. Global Style Block

```text
Empire of Cards, a premium tabletop business strategy game, stylized 2.5D low-poly/toon 3D art, physical board game pieces on a dark warm wooden desk, readable card-game UI, business simulation mood, clean beveled shapes, chunky tabletop miniatures, soft shadows, warm desk lamp lighting from upper left, subtle cool fill light, non-realistic but mature satirical business tone, high contrast readable labels, no fantasy magic, no medieval elements, no casual mobile clutter, Steam PC game quality, orthographic-friendly top-down composition, realistic business objects simplified into toy-like miniatures.
```

## 2. Global Negative Prompt

```text
No fantasy spells, no monsters, no medieval weapons, no sci-fi hologram overload, no childish toy aesthetic, no excessive neon, no casino chips theme, no cluttered unreadable UI, no tiny illegible text, no blurred UI, no photorealistic grime, no horror lighting, no cute chibi characters, no mobile idle-game chaos, no random icons, no abstract rectangles without business meaning, no giant decorative gradients, no purple-blue dominated gradient background, no flat debug prototype look.
```

## 3. 3D Model Production Rules Prompt

```text
Create a low-poly stylized Unity-ready tabletop miniature asset for Empire of Cards. The asset must be readable from a top-down orthographic camera, use simple clean geometry, beveled edges, compact silhouette, one main material plus 2-3 accent materials, no tiny details, no photorealism. It should look like a premium board game piece representing a real business object. Keep polygon count low, shape language clear, and leave room for color variants. The model should sit on a tabletop board slot and cast a soft shadow.
```

---

## 4. Main Menu Prompts

## 4.1 Main Menu Background Concept

```text
Global Style Block.

Main menu background for "Empire of Cards": a cinematic top-down office desk scene, dark warm wood table, brass and cream "EMPIRE OF CARDS" physical desk sign, scattered business plan papers, ledger notebook, gold coins, customer tokens, supplier contract stamp, small rival red marker in the distance, small business props around the edges: burger tray, coffee cup, grocery crate, laptop, clothing rack. Center space kept clean for menu buttons. Warm desk lamp glow from upper left, soft vignette, premium Steam strategy game mood, readable and uncluttered, 16:9 composition.

Global Negative Prompt.
```

## 4.2 Main Menu UI Mockup

```text
Global Style Block.

Design a 1920x1080 main menu UI for a PC Steam business card strategy game. Background is a physical 3D business desk scene. Title "EMPIRE OF CARDS" at top center as a brass/cream sign, not flat text. Buttons in lower center: NEW RUN, LOAD RUN, SETTINGS, QUIT. Buttons are rectangular, crisp, high contrast, slight bevel, no pill shapes. Right side has a compact latest save business dossier panel. Left side has small decorative venture props. UI must feel mature, readable, strategic, and business-themed.

Global Negative Prompt.
```

## 4.3 Main Menu Button Set

```text
Global Style Block.

Create a UI button set for Empire of Cards: rectangular business dashboard buttons with subtle bevels, 4-8 px corner radius, dark panel base, cream text, clear hover border, pressed state, disabled state. Variants: green primary NEW RUN, blue LOAD RUN, amber SETTINGS, muted red QUIT/DELETE. No glossy mobile style, no pill buttons. Buttons should fit a Steam PC strategy game.

Global Negative Prompt.
```

---

## 5. Venture Selection Prompts

## 5.1 Full Venture Selection Screen

```text
Global Style Block.

Design the venture selection screen for Empire of Cards. Five physical business dossier tiles sit across a dark tabletop: FAST FOOD, CAFE, TECH APP, CLOTHING STORE, GROCERY STORE. Each tile has a small 3D miniature preview, one-line pressure summary, and three small pressure chips. The selected tile is raised with a colored rim glow. Background shows the game board dimmed behind the overlay. Bottom center has a green rectangular START button. Text is readable, not paragraph-heavy. Mature business simulation tone.

Global Negative Prompt.
```

## 5.2 Fast Food Venture Tile

```text
Global Style Block.

Create a venture selection tile for FAST FOOD. Physical tabletop dossier card with a miniature fast food counter, grill steam, small table, delivery bag, and review star sign. Accent colors tomato red, fryer orange, mustard yellow, dark steel. Pressure chips: Speed, Hygiene, Delivery. One-line mood: Rush traffic, review risk, kitchen pressure. Readable from a 1920x1080 UI screen.

Global Negative Prompt.
```

## 5.3 Cafe Venture Tile

```text
Global Style Block.

Create a venture selection tile for CAFE. Physical tabletop dossier card with a miniature espresso machine, coffee cup steam, pastry display, small table, plant, Google Maps pin. Accent colors espresso brown, cream, brass, soft green. Pressure chips: Quality, Ambience, Regulars. One-line mood: Premium quality, loyal regulars, slow service risk. Readable from a 1920x1080 UI screen.

Global Negative Prompt.
```

## 5.4 Grocery Venture Tile

```text
Global Style Block.

Create a venture selection tile for GROCERY STORE / MARKET. Physical tabletop dossier card with a miniature shelf, checkout scanner, produce crate, fridge, WhatsApp order phone, bread basket. Accent colors fresh green, produce red/yellow, fridge cyan, cardboard tan. Pressure chips: Freshness, Checkout, Loyalty. One-line mood: Repeat traffic, tight margins, spoilage pressure. Readable from a 1920x1080 UI screen.

Global Negative Prompt.
```

## 5.5 Tech App Venture Tile

```text
Global Style Block.

Create a venture selection tile for TECH APP. Physical tabletop dossier card with a miniature laptop, smartphone app screen, server stack, bug token, analytics chart, cloud bill. Accent colors cyan blue, dark graphite, signal green. Pressure chips: Stability, Growth, Reviews. One-line mood: Slow opening, explosive upside, backend pressure. Must still look like a tabletop board game piece, not a full sci-fi hologram.

Global Negative Prompt.
```

## 5.6 Clothing Store Venture Tile

```text
Global Style Block.

Create a venture selection tile for CLOTHING STORE. Physical tabletop dossier card with a miniature display mannequin, clothing rack, folded clothes table, checkout, return box, fabric swatches. Accent colors rose, charcoal, fabric cream, soft gold. Pressure chips: Display, Stock, Returns. One-line mood: Trend timing, stock discipline, visual merchandising. Readable from a 1920x1080 UI screen.

Global Negative Prompt.
```

---

## 6. Naming Flow Prompts

## 6.1 Company Naming Modal

```text
Global Style Block.

Design a "Name your company" modal for a tabletop business card strategy game. The modal looks like a business registration paper on a dark desk, with a small storefront sign preview, input field, and rectangular START RUN button. Warm cream paper, dark ink, subtle stamp area, no long paragraphs. Include visual hint that the name appears on the storefront sign. Clean readable PC UI, 1920x1080 overlay.

Global Negative Prompt.
```

## 6.2 Tech App Naming Modal

```text
Global Style Block.

Design a "Name your app" modal for Empire of Cards. The modal appears as a laptop/app store registration form on the tabletop. It includes a product name input field, short explanation about backend stability, reviews, churn, and growth pressure, and a blue-green rectangular NEXT: PICK CATEGORY button. Keep it readable, compact, mature, and businesslike.

Global Negative Prompt.
```

## 6.3 Tech Category Selection Modal

```text
Global Style Block.

Design a tech app category selection modal for a business card strategy game. Six category buttons appear as app market tiles: Creative Tooling, Health & Fitness, Lifestyle, AI Features, Mobile Game, Hyper Casual. Each tile has a small icon and pressure summary. Selected tile gets blue rim glow. Bottom button says START APP. Visual style: tabletop laptop form mixed with board game UI, dark graphite, cyan accents, readable PC interface.

Global Negative Prompt.
```

---

## 7. Game Board Prompts

## 7.1 Full Board Concept

```text
Global Style Block.

Design the main gameplay board for Empire of Cards, top-down 2.5D tabletop view. Three zones: Rival board at top in darker red-brown, Shared Market in center with customer tokens and 10 ownership blocks, Player board at bottom with clear lanes for Operation, Staff, Marketing, Supplier, Temp Effect, Play, Sell, and card hand. Dark warm wooden desk, recessed colored slot trays, raised cards with shadows, physical tokens, small business props, readable HUD at top, no clutter. 16:9 PC game screenshot composition.

Global Negative Prompt.
```

## 7.2 Player Board Zone

```text
Global Style Block.

Create the player board zone for Empire of Cards. It is a physical tabletop tray area with lanes: Operation blue steel, Staff green, Marketing plum, Supplier amber, Temp Effect orange-red, Play coral, Sell brown. Each slot is recessed, beveled, and sized for 3D cards. Business props appear beside slots. Player zone is warm, readable, and brighter than rival zone. It must feel like "this is my business".

Global Negative Prompt.
```

## 7.3 Shared Market Zone

```text
Global Style Block.

Create the shared market zone for Empire of Cards. Center tabletop tray with 10 chunky market share blocks, neutral gray blocks in center, player blue blocks on one side, rival red blocks on the other. Small customer tokens flow between sides. Include labels or icon areas for Customer Flow, Trust, Market Pull. It should feel like a contested neighborhood/customer pool, not just a progress bar.

Global Negative Prompt.
```

## 7.4 Rival Board Zone

```text
Global Style Block.

Create the rival board zone for Empire of Cards. Darker red-brown tabletop tray at the top of the board, with visible but simplified rival slots for Operation, Staff, Marketing, Supplier, and Signal. It should show rival strategy under partial information. Use red ownership markers, shadowed cards, small warning glow when rival moves. Dangerous but less visually loud than player zone.

Global Negative Prompt.
```

## 7.5 HUD Dashboard

```text
Global Style Block.

Design a compact top HUD dashboard for a PC business card strategy game. Left: cash and company tier. Center: pressure summary, build state, season, turn number. Center-lower: market share bar. Right: rating and legal risk. Dark transparent panels, warm cream text, gold money, blue player, red rival, green safe legal risk, amber/red danger. Clean high contrast, no decorative clutter.

Global Negative Prompt.
```

---

## 8. Card Prompts

## 8.1 Generic 3D Card Model

```text
3D Model Production Rules.

Create a generic physical 3D playing card model for Empire of Cards. Rounded rectangular card, slight thickness, beveled edges, dark card back, front face suitable for a world-space UI texture, subtle edge highlight, casts soft tabletop shadow. Card ratio close to 2.5:3.5. It should look premium and readable in a top-down Unity board game.
```

## 8.2 Card Face Template

```text
Global Style Block.

Design a card face template for Empire of Cards. Physical strategy card UI with accent color bar at top, card name, cost chip, icon/mini illustration area, role chip, short effect summary, projected stat delta, and upkeep/risk line. Dark panel base, warm cream text, high contrast. Make it mature business simulation, not fantasy trading card. Leave room for different card families: Operation, Staff, Marketing, Supplier, Event, Risk, Reaction.

Global Negative Prompt.
```

## 8.3 Operation Card Prompt

```text
Global Style Block.

Create an Operation card for Empire of Cards. Blue steel accent color. The card represents a physical/core business asset such as kitchen, bar, shelf, backend, or display. Include a miniature equipment icon, clear cost chip, and short business effect text. It should feel like buying real capacity, not casting a buff.

Global Negative Prompt.
```

## 8.4 Staff Card Prompt

```text
Global Style Block.

Create a Staff card for Empire of Cards. Green accent color. The card represents hiring a real worker with payroll and stability impact. Include a simple worker portrait/standee icon, role chip, salary/upkeep indicator, and short effect text about capacity, quality, service, or stability. Mature business sim tone.

Global Negative Prompt.
```

## 8.5 Marketing Card Prompt

```text
Global Style Block.

Create a Marketing card for Empire of Cards. Plum/purple accent color. The card represents a real growth action such as flyers, Google Ads, Instagram, loyalty, ASO, influencer, or local posters. Include a phone/poster/megaphone icon, demand effect, cost/upkeep, and possible rating/risk note. Readable PC card UI.

Global Negative Prompt.
```

## 8.6 Supplier Card Prompt

```text
Global Style Block.

Create a Supplier card for Empire of Cards. Amber/ochre accent color. The card represents a real supplier contract, quality source, cloud tool, fabric provider, bakery, butcher, dairy, or distributor. Include crate/contract/invoice visual language, quality/cost tradeoff, and reliability implication.

Global Negative Prompt.
```

## 8.7 Crisis/Event Card Prompt

```text
Global Style Block.

Create a Crisis/Event card for Empire of Cards. Orange-red warning accent. It should look like a stamped urgent business document on a physical card: inspection notice, bad reviews, supplier failure, staff burnout, legal warning, app crash, or spoiled stock. Include clear title area, warning icon, duration chip, and consequence summary. Serious but stylized.

Global Negative Prompt.
```

---

## 9. Shared Tabletop Asset Prompts

## 9.1 Main Desk/Table

```text
3D Model Production Rules.

Create the main gameplay desk/table for Empire of Cards. Large dark warm wooden tabletop with raised outer rim, recessed central felt/control surface, subtle bevels, board-game premium feel. Must support clear tabletop card placement. Include separate lane areas but no tiny labels. Warm brown/black materials, soft edge highlights.
```

## 9.2 Slot Tray

```text
3D Model Production Rules.

Create a recessed card slot tray for Empire of Cards. Rectangular tabletop tray with beveled rim, shallow depression, subtle colored edge glow, sized for one 3D card. Needs variants for Operation blue, Staff green, Marketing plum, Supplier amber, Temp Effect orange-red, Play coral, Sell brown. The tray must clearly indicate valid drop targets.
```

## 9.3 Customer Token

```text
3D Model Production Rules.

Create a chunky customer token for Empire of Cards. Small round tabletop token with simple person silhouette stamped on top. Variants: player blue, rival red, neutral warm gray. Readable from top-down camera, soft bevel, board-game piece style.
```

## 9.4 Money Token

```text
3D Model Production Rules.

Create a gold money token/coin for Empire of Cards. Small chunky coin with simple dollar or Empire mark, beveled rim, stylized board-game material, used for cash gain and cost feedback. Must be readable in small groups and suitable for coin burst animations.
```

## 9.5 Market Share Block

```text
3D Model Production Rules.

Create a market share ownership block for Empire of Cards. Small rectangular chunky board piece, soft bevel, flips or changes color between neutral gray, player blue, and rival red. It represents local customer control. Must look like a physical territory/customer block, not a flat UI bar.
```

## 9.6 Warning Stamp Token

```text
3D Model Production Rules.

Create a crisis/legal warning stamp token for Empire of Cards. Physical red-orange stamped-paper marker, like an inspection stamp or urgent notice. Used for hygiene inspection, legal risk, tax audit, bad review storm, and crisis cards. Must feel serious, readable, and tabletop.
```

---

## 10. Fast Food Asset Prompts

## 10.1 Fast Food Core Diorama

```text
Global Style Block.

Create a fast food tabletop business diorama for Empire of Cards. Miniature counter, grill/fryer, small table cluster, menu board, delivery bag, cleaning bucket, supplier crate. Stylized low-poly, tomato red, fryer orange, mustard yellow, dark steel. It should communicate rush traffic, kitchen pressure, hygiene risk, and fast local growth. Top-down readable.

Global Negative Prompt.
```

## 10.2 Fast Food Counter Model

```text
3D Model Production Rules.

Create a fast food service counter miniature. Include small register, order pickup shelf, menu sign, red/yellow accents, simple food tray. Must fit on an Operation slot and read clearly from top-down camera.
```

## 10.3 Fast Food Grill/Fryer Model

```text
3D Model Production Rules.

Create a stylized fast food kitchen station miniature: flat grill, fryer basket, small steam/sizzle area, steel body, orange heat accent. Readable from top-down. No realistic grease, no excessive detail.
```

## 10.4 Fast Food Seating Model

```text
3D Model Production Rules.

Create a small fast food table-and-seat set as a tabletop miniature. Chunky square table, two simple stools/chairs, red-orange accents, clean bevels. Used as an Operation prop when seating expands.
```

## 10.5 Delivery Bag Model

```text
3D Model Production Rules.

Create a delivery bag miniature for a fast food business board. Boxy insulated courier bag, red/orange material, small receipt tag, readable top-down. Represents delivery capacity and food app pressure.
```

## 10.6 Fast Food Cook Standee

```text
Global Style Block.

Create a simple tabletop staff standee for a fast food cook. Stylized worker with apron, cap, spatula, confident busy pose, orange/yellow role accents, readable from top-down/isometric camera. Mature business-board-game style, not chibi.

Global Negative Prompt.
```

## 10.7 Fast Food Cashier Standee

```text
Global Style Block.

Create a simple tabletop staff standee for a fast food cashier. Stylized worker at a register, service-facing posture, green staff accent plus red/yellow fast food details. Must communicate queue handling and checkout flow.

Global Negative Prompt.
```

## 10.8 Fast Food Courier Standee

```text
Global Style Block.

Create a simple tabletop staff standee for a fast food courier. Helmet, delivery backpack, quick forward-leaning pose, orange/red delivery bag accent. Must communicate delivery speed and local order pressure.

Global Negative Prompt.
```

## 10.9 Garson / Waiter Standee

```text
Global Style Block.

Create a tabletop staff standee for a waiter/garson in Empire of Cards. The worker carries a food tray, faces customer flow, has a service-focused pose, simple uniform, green staff accent. Must be visually distinct from a kitchen cook and from a busser/komi.

Global Negative Prompt.
```

## 10.10 Komi / Busser Standee

```text
Global Style Block.

Create a tabletop staff standee for a komi/busser in Empire of Cards. The worker carries a clearing tray or small cart, faces tables/kitchen, suggests clearing bottlenecks and resetting seating. Simple uniform, green staff accent, readable from top-down. Must be distinct from waiter/garson.

Global Negative Prompt.
```

## 10.11 Fast Food Crisis Tokens

```text
Global Style Block.

Create a set of fast food crisis tabletop tokens: bad review phone, hygiene inspection clipboard, broken fryer smoke marker, delivery fee notice, viral bad video phone icon. Stylized low-poly/toon, red-orange warning accents, readable from top-down, physical board game pieces.

Global Negative Prompt.
```

---

## 11. Cafe Asset Prompts

## 11.1 Cafe Core Diorama

```text
Global Style Block.

Create a cafe tabletop business diorama for Empire of Cards. Miniature espresso bar, grinder, steaming coffee cup, pastry display, small seating table, chalkboard, plant. Espresso brown, cream, brass, soft green accents. Mood: quality, regulars, ambience, slow service pressure. Top-down readable.

Global Negative Prompt.
```

## 11.2 Espresso Machine Model

```text
3D Model Production Rules.

Create a stylized espresso machine miniature. Compact machine with portafilter shape, steam wand, cup platform, brass/steel and coffee-brown accents. Must read clearly from top-down and fit on a cafe Operation slot.
```

## 11.3 Pastry Display Model

```text
3D Model Production Rules.

Create a small cafe pastry display case miniature. Rectangular glass-like case, simple pastries inside as chunky shapes, cream/brass accents, top-down readable. Represents vitrin/pastane capacity and quality.
```

## 11.4 Coffee Bean Sack Model

```text
3D Model Production Rules.

Create a coffee bean sack supplier prop. Small burlap-style sack, coffee bean icon, warm brown tones, simple tied top, readable from top-down. Represents bean quality and supplier reliability.
```

## 11.5 Barista Standee

```text
Global Style Block.

Create a tabletop staff standee for a cafe barista. Stylized worker with apron, coffee cup, latte art gesture, calm confident pose, cream/brown cafe colors with green staff accent. Must communicate drink consistency and quality.

Global Negative Prompt.
```

## 11.6 Floor Staff Standee

```text
Global Style Block.

Create a tabletop staff standee for cafe floor staff. Worker carrying tray or wiping table, service flow posture, cafe brown/cream colors with green staff accent. Must communicate table turnover and guest care.

Global Negative Prompt.
```

## 11.7 Cafe Crisis Tokens

```text
Global Style Block.

Create a set of cafe crisis tabletop tokens: barista burnout marker, empty milk crate, broken WiFi router, slow service complaint note, music license warning paper. Stylized low-poly/toon, amber and red warning accents, physical board game pieces, top-down readable.

Global Negative Prompt.
```

---

## 12. Grocery / Market Asset Prompts

## 12.1 Grocery Core Diorama

```text
Global Style Block.

Create a grocery store / neighborhood market tabletop diorama for Empire of Cards. Miniature shelf row, checkout register, fridge, produce crate, bread basket, WhatsApp order phone, delivery basket. Fresh green, produce red/yellow, fridge cyan, cardboard tan. Mood: repeat traffic, freshness, tight margins, neighborhood trust. Top-down readable.

Global Negative Prompt.
```

## 12.2 Shelf Row Model

```text
3D Model Production Rules.

Create a small grocery shelf row miniature. Chunky shelves with simple cans/boxes as colored blocks, green/cream accents, readable from top-down. Represents shelf capacity and stock discipline.
```

## 12.3 Checkout Counter Model

```text
3D Model Production Rules.

Create a grocery checkout counter miniature. Small register, scanner, conveyor strip or counter surface, green/gray accents. Must communicate checkout speed and friction reduction.
```

## 12.4 Fridge Model

```text
3D Model Production Rules.

Create a small grocery fridge miniature. Upright or horizontal cold display, cyan glow, simple door shape, product blocks inside. Represents cold chain and freshness pressure. Top-down readable.
```

## 12.5 Produce Crate Model

```text
3D Model Production Rules.

Create a fresh produce crate miniature. Wooden/cardboard crate with chunky stylized tomatoes, greens, lemons or apples, fresh green and red/yellow accents. Represents taze urun quality and spoilage risk.
```

## 12.6 Grocery Cashier Standee

```text
Global Style Block.

Create a tabletop staff standee for a grocery cashier. Worker at register/scanner, simple vest, green staff accent, local shop feel. Must communicate checkout flow and repeat customer service.

Global Negative Prompt.
```

## 12.7 Shelf Worker / Reyon Standee

```text
Global Style Block.

Create a tabletop staff standee for a grocery shelf worker/reyon staff. Worker carrying a box or restocking cart, green staff accent, practical local market uniform. Must communicate shelf discipline and stock health.

Global Negative Prompt.
```

## 12.8 Grocery Crisis Tokens

```text
Global Style Block.

Create a set of grocery crisis tabletop tokens: broken fridge warning, spoiled produce marker, expiry date stamp, unpaid ledger/veresiye notebook, power outage icon, chain-market threat sign. Stylized low-poly/toon, red-orange warning accents, physical board game pieces, top-down readable.

Global Negative Prompt.
```

---

## 13. Tech App Asset Prompts

## 13.1 Tech App Core Diorama

```text
Global Style Block.

Create a tech app tabletop business diorama for Empire of Cards. Miniature laptop, smartphone app screen, server stack, cloud bill, analytics chart, bug token, sticky notes. Cyan/blue, dark graphite, signal green accents. Mood: backend stability, growth pressure, reviews, churn, infra cost. Must remain a tabletop board game miniature, not full sci-fi.

Global Negative Prompt.
```

## 13.2 Laptop/App Screen Model

```text
3D Model Production Rules.

Create a laptop and smartphone app-screen miniature. Simple dark graphite laptop with cyan app screen, small phone tile, analytics line. Represents product/app identity. Top-down readable, no tiny UI text.
```

## 13.3 Server Stack Model

```text
3D Model Production Rules.

Create a server stack miniature for a tech app board. Small stacked server blocks, cyan status lights, dark graphite body, warning light variant. Represents backend stability and capacity.
```

## 13.4 Developer Standee

```text
Global Style Block.

Create a tabletop staff standee for a developer. Stylized worker with laptop, hoodie or simple office clothes, blue/cyan tech accent plus green staff signal. Must communicate building and stabilizing backend/product.

Global Negative Prompt.
```

## 13.5 Product Manager Standee

```text
Global Style Block.

Create a tabletop staff standee for a product manager. Worker with roadmap board or clipboard, dark office clothing, cyan/green accents. Must communicate planning features and retention pressure.

Global Negative Prompt.
```

## 13.6 Tech Crisis Tokens

```text
Global Style Block.

Create a set of tech app crisis tabletop tokens: crash warning screen, bug token, server overload marker, security breach lock, review bomb phone, churn leak icon. Stylized board game pieces, cyan/blue tech base with red-orange warning accents, top-down readable.

Global Negative Prompt.
```

---

## 14. Clothing Store Asset Prompts

## 14.1 Clothing Store Core Diorama

```text
Global Style Block.

Create a clothing store tabletop business diorama for Empire of Cards. Miniature display mannequin, clothing rack, folded clothes table, fitting mirror, checkout counter, return box, fabric swatches. Rose, charcoal, fabric cream, soft gold accents. Mood: trend timing, display battles, stock discipline, return pressure. Top-down readable.

Global Negative Prompt.
```

## 14.2 Display Mannequin Model

```text
3D Model Production Rules.

Create a small display mannequin miniature for a clothing store board. Simple mannequin silhouette on base, rose/gold accent garment, readable from top-down. Represents vitrin/display pull.
```

## 14.3 Clothing Rack Model

```text
3D Model Production Rules.

Create a small clothing rack miniature. Simple horizontal rack with chunky hanging clothes shapes, charcoal/cream/rose colors, readable from top-down. Represents stock and display capacity.
```

## 14.4 Sales Associate Standee

```text
Global Style Block.

Create a tabletop staff standee for a clothing store sales associate. Worker holding hanger or tablet, stylish but simple, rose/charcoal clothing, green staff accent. Must communicate floor conversion and customer assistance.

Global Negative Prompt.
```

## 14.5 Tailor Standee

```text
Global Style Block.

Create a tabletop staff standee for a tailor. Worker with measuring tape or fabric roll, cream/rose/charcoal accents, practical pose. Must communicate fit quality and return reduction.

Global Negative Prompt.
```

## 14.6 Clothing Crisis Tokens

```text
Global Style Block.

Create a set of clothing store crisis tabletop tokens: return pile, bad fit complaint, seasonal mismatch tag, empty stock box, damaged fabric marker, discount pressure tag. Stylized low-poly/toon physical board game pieces, rose/charcoal base with red-orange warning accents.

Global Negative Prompt.
```

---

## 15. Event & Feedback Prompts

## 15.1 Card Placement Feedback

```text
Global Style Block.

Visualize a 3D business card being dragged and placed onto a valid tabletop slot in Empire of Cards. The card is raised above the table, valid slot pulses green, invalid slots dim, projected business effect appears as a small readable tooltip, card snaps down with glow and soft shadow. Strategy board game UI, no clutter.

Global Negative Prompt.
```

## 15.2 Market Share Shift Feedback

```text
Global Style Block.

Visualize market share shifting in Empire of Cards. Customer tokens slide across the shared market tray, one neutral block flips to player blue, rival red blocks remain at the far side, small cause text reads "Rating recovered → customers returned". Physical board game pieces, readable top-down, satisfying but not flashy.

Global Negative Prompt.
```

## 15.3 Legal Risk Warning Feedback

```text
Global Style Block.

Visualize legal risk rising in Empire of Cards. A red-orange warning stamp lands on a Temp Effect card, legal risk bar pulses amber to red, thin red tape wraps around a risky card, board remains readable. Serious business consequence, not fantasy damage.

Global Negative Prompt.
```

## 15.4 Rival Move Feedback

```text
Global Style Block.

Visualize rival action feedback in Empire of Cards. Rival board at top pulses dark red, a rival card silhouette snaps into a slot, a red customer token moves toward rival side in shared market, compact banner says "RIVAL MOVE: Price pressure". Dangerous but readable strategy UI.

Global Negative Prompt.
```

## 15.5 Income Feedback

```text
Global Style Block.

Visualize income resolution in Empire of Cards. Gold money tokens pop from player business props toward the cash HUD, cash number counts upward, customer tokens settle, board remains visible and not cluttered. Premium tabletop business game feel, satisfying cash register feedback.

Global Negative Prompt.
```

---

## 16. Steam / Marketing Visual Prompts

## 16.1 Steam Capsule Key Art

```text
Global Style Block.

Create Steam capsule key art for Empire of Cards. A dramatic tabletop business war scene: player blue business board at bottom, rival red business board at top, shared market tokens in center, physical cards flying/placing, small fast food/cafe/grocery props visible, gold coins, legal warning stamp, warm desk lamp. Title space for "EMPIRE OF CARDS". Mature satirical business strategy, readable at small size, high contrast.

Global Negative Prompt.
```

## 16.2 Screenshot Composition Prompt

```text
Global Style Block.

Create a polished gameplay screenshot composition for Empire of Cards. Show a player card being placed, Phase 1 business props visible, shared market blocks moving, rival pressure at top, compact HUD with cash/rating/legal risk. Must communicate: card strategy, real business simulation, local market competition, tabletop physicality. 16:9 Steam screenshot.

Global Negative Prompt.
```

## 16.3 Trailer Shot Prompt

```text
Global Style Block.

Create a trailer shot concept for Empire of Cards. Camera sweeps over a dark tabletop board as cards snap into slots, a cook standee appears, customer tokens move toward player, rival red board responds, legal warning stamp slams down. Warm desk lighting, satisfying physical board-game motion, business strategy tension.

Global Negative Prompt.
```

---

## 17. Implementation Prompt For Codex / Developer Agent

Use this when asking an implementation agent to convert the art bible into code changes.

```text
You are working in the Unity 6 project Empire of Cards. Read `Assets/steam-card-game-gdd/GDD.md`, the relevant venture file, `Assets/steam-card-game-gdd/UI_UX_3D_ART_BIBLE.md`, and `Assets/steam-card-game-gdd/UI_UX_3D_PROMPT_PACK.md` before changing code.

Goal: improve the game presentation while preserving architecture. Do not rewrite gameplay. Keep EventBus communication, WiringService references, Init pattern, and CardData runtime immutability.

Focus on one implementation slice:
[INSERT SLICE: venture selection / board trays / card visuals / Phase 1 props / main menu / market share tokens / staff standees / crisis tokens]

Existing likely files:
- `Assets/_EmpireOfCards/Scripts/Presentation/ControlDeskTheme.cs`
- `Assets/_EmpireOfCards/Scripts/World/Board3D.cs`
- `Assets/_EmpireOfCards/Scripts/World/CardFactory.cs`
- `Assets/_EmpireOfCards/Scripts/UI/VentureSelectionUI.cs`
- `Assets/_EmpireOfCards/Scripts/Bootstrap/HUDBuilder.cs`
- `Assets/_EmpireOfCards/Scripts/Bootstrap/Data/V4ContentFactory.cs`
- `Assets/_EmpireOfCards/Scripts/Data/VentureProfiles.cs`

Acceptance:
- Board remains readable at 1920x1080, 1366x768, and 1280x800.
- No text overlap.
- Venture identity is visible without reading the title.
- Player/rival/neutral market state is readable without color alone.
- Slot target is obvious before dragging.
- Legal/rating/cash pressure remains visible.
- Phase 1 ventures feel distinct.
```

