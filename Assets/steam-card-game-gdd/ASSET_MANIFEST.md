# Empire of Cards Complete Asset Manifest

> Full-game asset bible. Görsel hedef: Toy Diorama. Her asset küçük, okunur, sistem bilgisini taşıyan ve sabit izometrik kameradan anlaşılır olmalıdır.

## 1. Manifest Rules

Her asset satırı şu alanları taşır:

- `Asset ID`: production ve prompt referansı.
- `Category`: Character, Animation, Business Module, Environment, Card, UI, VFX, Audio, Material, Lighting, Debug.
- `Venture`: Shared, FastFood, Cafe, TechApp, Giyim, Market, Meta.
- `Usage`: gameplay kullanım yeri.
- `Priority`: P0, P1, P2.
- `Gameplay State`: hangi state'i temsil eder.
- `Variants`: gerekli varyasyonlar.
- `Placeholder`: prototip karşılığı.
- `Final Requirement`: final kalite kriteri.
- `Prompt Ref`: `ASSET_PROMPTS.md` içindeki prompt/brief referansı.

Priority:

- `P0`: oynanabilir core loop için zorunlu.
- `P1`: venture kimliği ve tam oyun okunabilirliği için gerekli.
- `P2`: polish, holding/meta, dekoratif varyasyon.

## 2. Character Assets

| Asset ID | Category | Venture | Usage | Priority | Gameplay State | Variants | Placeholder | Final Requirement | Prompt Ref |
|---|---|---|---|---:|---|---|---|---|---|
| EOC_CHAR_Customer_Neutral | Character | Shared | District neutral crowd | P0 | undecided market | 3 body, 3 hair, 2 outfits | capsule gray | tintable toy person, clear silhouette | PROMPT_CHAR_CUSTOMER |
| EOC_CHAR_Customer_Player | Character | Shared | Player customers | P0 | blue ownership | happy, impatient, angry, leaving | neutral mesh blue | same rig, blue readable from overview | PROMPT_CHAR_CUSTOMER |
| EOC_CHAR_Customer_Rival | Character | Shared | Rival customers | P0 | red ownership | happy, impatient, angry, leaving | neutral mesh red | same rig, red readable from overview | PROMPT_CHAR_CUSTOMER |
| EOC_CHAR_Customer_Loyal | Character | Shared | Repeat/gold customers | P1 | loyalty / high rating | gold accent, venture accessory | blue mesh + gold ring | visually valuable but not noisy | PROMPT_CHAR_CUSTOMER |
| EOC_CHAR_Staff_FastFood_Cook | Character | FastFood | Kitchen worker | P0 | kitchen capacity | normal, stressed | capsule apron | hat/apron silhouette, grill readable | PROMPT_CHAR_STAFF_FASTFOOD |
| EOC_CHAR_Staff_FastFood_Cashier | Character | FastFood | Checkout | P0 | service capacity | normal, stressed, quit | capsule cashier | counter-facing pose, readable stress | PROMPT_CHAR_STAFF_FASTFOOD |
| EOC_CHAR_Staff_FastFood_Waiter | Character | FastFood | Service / staff card | P0 | staff install | normal, carrying tray, stressed | capsule tray | linked to Staff slot card | PROMPT_CHAR_STAFF_FASTFOOD |
| EOC_CHAR_Staff_FastFood_Courier | Character | FastFood | Delivery | P0 | delivery capacity | idle, carrying package | capsule backpack | package silhouette | PROMPT_CHAR_STAFF_FASTFOOD |
| EOC_CHAR_Staff_Cafe_Barista | Character | Cafe | Bar quality | P1 | quality / burnout | normal, latte, stressed | recolored staff | apron, cup, barista pose | PROMPT_CHAR_STAFF_CAFE |
| EOC_CHAR_Staff_Cafe_Floor | Character | Cafe | Seating/service | P1 | table service | normal, tray, stressed | recolored waiter | cafe-specific silhouette | PROMPT_CHAR_STAFF_CAFE |
| EOC_CHAR_Staff_Tech_Developer | Character | TechApp | App stability | P1 | feature/backend work | normal, crunch, burnout | desk capsule | laptop posture, stress readable | PROMPT_CHAR_STAFF_TECH |
| EOC_CHAR_Staff_Tech_Designer | Character | TechApp | UX quality | P1 | product quality | normal, presenting | desk capsule | tablet/sketch prop | PROMPT_CHAR_STAFF_TECH |
| EOC_CHAR_Staff_Tech_Support | Character | TechApp | Support backlog | P1 | support capacity | normal, overwhelmed | headset capsule | headset/ticket prop | PROMPT_CHAR_STAFF_TECH |
| EOC_CHAR_Staff_Giyim_Sales | Character | Giyim | Conversion | P1 | sales help | normal, advising | recolored staff | clothing accessory | PROMPT_CHAR_STAFF_GIYIM |
| EOC_CHAR_Staff_Giyim_Tailor | Character | Giyim | Fit/returns | P1 | quality/fit | normal, measuring | recolored staff | tape measure prop | PROMPT_CHAR_STAFF_GIYIM |
| EOC_CHAR_Staff_Market_Cashier | Character | Market | Checkout | P1 | queue service | normal, stressed | cashier capsule | vest + register pose | PROMPT_CHAR_STAFF_MARKET |
| EOC_CHAR_Staff_Market_Courier | Character | Market | WhatsApp delivery | P1 | delivery capacity | package, leaving | courier capsule | grocery bag silhouette | PROMPT_CHAR_STAFF_MARKET |
| EOC_CHAR_Rival_Manager | Character | Shared | Rival action signal | P1 | rival pressure | 1 per venture tint | red manager capsule | red accent, non-combat rival | PROMPT_CHAR_RIVAL |
| EOC_CHAR_Inspector | Character | Shared | Legal event | P1 | audit/legal risk | normal, clipboard | dark capsule | clipboard, serious silhouette | PROMPT_CHAR_META |
| EOC_CHAR_Investor | Character | Meta | Holding/exit | P2 | meta offer | investor, assistant | suited capsule | gold/neutral business accent | PROMPT_CHAR_META |

## 3. Animation Assets

| Asset ID | Category | Venture | Usage | Priority | Gameplay State | Variants | Placeholder | Final Requirement | Prompt Ref |
|---|---|---|---|---:|---|---|---|---|---|
| EOC_ANIM_Customer_Walk | Animation | Shared | District movement | P0 | flow | neutral, hurry, leaving | root walk | short loop, readable direction | BRIEF_ANIM_CUSTOMER |
| EOC_ANIM_Customer_Wait | Animation | Shared | Queue impatience | P0 | queue pressure | calm, impatient, angry | idle sway | foot tap / watch check | BRIEF_ANIM_CUSTOMER |
| EOC_ANIM_Customer_Review | Animation | Shared | Rating feedback | P0 | review burst | positive, negative | icon pop | phone/review gesture | BRIEF_ANIM_CUSTOMER |
| EOC_ANIM_Staff_Work | Animation | Shared | Work loop | P0 | capacity | role-specific | idle/work | loop must not distract | BRIEF_ANIM_STAFF |
| EOC_ANIM_Staff_Stress | Animation | Shared | Stress event | P0 | staff pressure | medium, high | red pulse | visible from overview | BRIEF_ANIM_STAFF |
| EOC_ANIM_Staff_Quit | Animation | Shared | Quit event | P0 | staff collapse | walkout | move away | emotional but short | BRIEF_ANIM_STAFF |
| EOC_ANIM_Card_Snap | Animation | Shared | Slot commit | P0 | card placement | install, policy, risk | scale pop | DOTween-friendly timing | BRIEF_ANIM_UI |
| EOC_ANIM_Card_Drag | Animation | Shared | Drag/inspect | P0 | card UX | hover, lift, invalid | UI tween | linked line support | BRIEF_ANIM_UI |
| EOC_ANIM_Event_Focus | Animation | Shared | Micro-cinematic | P1 | event intro/outro | stress, customer, rival | camera blend | synced with camera director | BRIEF_ANIM_EVENT |

## 4. Business Module Assets

| Asset ID | Category | Venture | Usage | Priority | Gameplay State | Variants | Placeholder | Final Requirement | Prompt Ref |
|---|---|---|---|---:|---|---|---|---|---|
| EOC_PROP_Shared_CashRegister | Business Module | Shared | Checkout anchor | P0 | queue/cash | normal, busy, broken | cube + screen | toy register, pulseable | PROMPT_PROP_SHARED |
| EOC_PROP_Shared_QueueMarkers | Business Module | Shared | Waiting path | P0 | queue pressure | blue/red/neutral | floor dots | clear path direction | PROMPT_PROP_SHARED |
| EOC_PROP_Shared_BusinessSign | Business Module | Shared | Brand/scale | P0 | identity/scale | startup, chain, holding | text board | readable without real logo | PROMPT_PROP_SHARED |
| EOC_PROP_Shared_SlotBoard | Business Module | Shared | Persistent cards | P0 | control board | 5 slot groups | flat board | cards readable, linked highlights | PROMPT_UI_SLOT_BOARD |
| EOC_PROP_FastFood_Grill | Business Module | FastFood | Kitchen operation | P0 | capacity/overload | normal, hot, dirty | cube heat icon | heat/stress VFX anchor | PROMPT_PROP_FASTFOOD |
| EOC_PROP_FastFood_Counter | Business Module | FastFood | Service | P0 | checkout/service | clean, busy | rectangular counter | queue anchor + staff position | PROMPT_PROP_FASTFOOD |
| EOC_PROP_FastFood_TableSet | Business Module | FastFood | Seating | P0 | seat capacity | empty, occupied, dirty | cube table | tiny tray/food readable | PROMPT_PROP_FASTFOOD |
| EOC_PROP_FastFood_DeliveryShelf | Business Module | FastFood | Delivery pressure | P0 | package backlog | empty, stacked, overflow | shelf blocks | package stack grows | PROMPT_PROP_FASTFOOD |
| EOC_PROP_FastFood_IngredientCrates | Business Module | FastFood | Supplier quality | P1 | quality/risk | fresh, cheap, spoiled | colored crates | supplier state visible | PROMPT_PROP_FASTFOOD |
| EOC_PROP_Cafe_EspressoMachine | Business Module | Cafe | Bar operation | P1 | bar capacity | normal, steaming, overloaded | box + cup | iconic silhouette | PROMPT_PROP_CAFE |
| EOC_PROP_Cafe_BarCounter | Business Module | Cafe | Barista anchor | P1 | service/quality | small, upgraded | box counter | warm cafe identity | PROMPT_PROP_CAFE |
| EOC_PROP_Cafe_PastryDisplay | Business Module | Cafe | Basket/quality | P1 | add-on sales | full, empty | glass box | visible pastry states | PROMPT_PROP_CAFE |
| EOC_PROP_Cafe_Seating | Business Module | Cafe | Ambience | P1 | loyalty/dwell | stool, couch, table | table set | comfortable toy style | PROMPT_PROP_CAFE |
| EOC_PROP_Cafe_TakeawayWindow | Business Module | Cafe | Quick demand | P1 | takeaway pressure | closed, active | window box | queue anchor | PROMPT_PROP_CAFE |
| EOC_PROP_Tech_DeskPod | Business Module | TechApp | Staff work | P1 | team capacity | dev, design, support | desk cube | role readable by prop | PROMPT_PROP_TECH |
| EOC_PROP_Tech_ServerNode | Business Module | TechApp | Backend | P0 | stability/crash | normal, hot, failed | tower cube | heat/crash VFX anchor | PROMPT_PROP_TECH |
| EOC_PROP_Tech_SupportDesk | Business Module | TechApp | Support | P1 | backlog | normal, tickets | desk + icon | ticket bubble anchor | PROMPT_PROP_TECH |
| EOC_PROP_Tech_AppRatingPanel | Business Module | TechApp | Store rating | P1 | rating/reviews | good, bad, glitch | panel stars | crack/glow states | PROMPT_PROP_TECH |
| EOC_PROP_Giyim_WindowDisplay | Business Module | Giyim | Vitrin pull | P1 | foot traffic | basic, premium, viral | display box | mannequin/rack silhouette | PROMPT_PROP_GIYIM |
| EOC_PROP_Giyim_Rack | Business Module | Giyim | Stock | P1 | stock health | full, low, empty | rack line | color/size readable | PROMPT_PROP_GIYIM |
| EOC_PROP_Giyim_FittingRoom | Business Module | Giyim | Try-on queue | P1 | queue/fit | available, busy | door box | queue anchor | PROMPT_PROP_GIYIM |
| EOC_PROP_Giyim_ReturnsDesk | Business Module | Giyim | Returns crisis | P1 | return pressure | normal, busy | small counter | complaint anchor | PROMPT_PROP_GIYIM |
| EOC_PROP_Market_Shelf | Business Module | Market | Stock | P0 | variety/stock | full, low, empty | shelf cube | readable SKUs, not cluttered | PROMPT_PROP_MARKET |
| EOC_PROP_Market_FreshBin | Business Module | Market | Fresh product | P1 | quality/fire | fresh, low, spoiled | crate | spoilage readable | PROMPT_PROP_MARKET |
| EOC_PROP_Market_Fridge | Business Module | Market | Cold goods | P1 | supplier/quality | normal, broken | box fridge | light/status state | PROMPT_PROP_MARKET |
| EOC_PROP_Market_VeresiyeBook | Business Module | Market | Credit risk | P1 | cash trap | closed, open, warning | book cube | event anchor | PROMPT_PROP_MARKET |
| EOC_PROP_Market_DeliveryArea | Business Module | Market | WhatsApp orders | P1 | delivery backlog | empty, stacked | shelf | grocery bag stack | PROMPT_PROP_MARKET |

## 5. District and Environment Assets

| Asset ID | Category | Venture | Usage | Priority | Gameplay State | Variants | Placeholder | Final Requirement | Prompt Ref |
|---|---|---|---|---:|---|---|---|---|---|
| EOC_ENV_District_BaseBoard | Environment | Shared | Main board floor | P0 | board layout | startup, growth | plane + paths | toy tabletop base | PROMPT_ENV_DISTRICT |
| EOC_ENV_District_PathSet | Environment | Shared | Customer flow | P0 | movement | neutral, player, rival | colored splines | readable lanes | PROMPT_ENV_DISTRICT |
| EOC_ENV_District_SpawnNodes | Environment | Shared | Customer spawn | P0 | market source | 4-8 nodes | empty locators | hidden or decorative | PROMPT_ENV_DISTRICT |
| EOC_ENV_District_TrendSpot | Environment | Shared | Events/trends | P1 | festival/viral/audit | 5 anchors | icon stand | modular event platform | PROMPT_ENV_DISTRICT |
| EOC_ENV_RivalBusiness_Base | Environment | Shared | Rival side | P0 | red competitor | 5 venture skins | red block shop | strategy readable | PROMPT_ENV_RIVAL |
| EOC_ENV_PlayerBusiness_Base | Environment | Shared | Player side | P0 | blue owner | 5 venture skins | blue block shop | slot/world alignment | PROMPT_ENV_PLAYER |
| EOC_ENV_HoldingScale_Backdrop | Environment | Meta | Scale/exit | P2 | holding candidate | brand skyline, investor | larger board | celebratory but readable | PROMPT_ENV_META |

## 6. Card and Slot Assets

| Asset ID | Category | Venture | Usage | Priority | Gameplay State | Variants | Placeholder | Final Requirement | Prompt Ref |
|---|---|---|---|---:|---|---|---|---|---|
| EOC_CARD_Frame_Install | Card | Shared | Persistent install cards | P0 | build/staff/supplier | 5 venture tints | rounded panel | readable at hand + slot size | PROMPT_CARD_FRAME |
| EOC_CARD_Frame_Burst | Card | Shared | One-shot cards | P0 | instant action | 5 venture tints | same frame + flash | distinct from install | PROMPT_CARD_FRAME |
| EOC_CARD_Frame_Policy | Card | Shared | Persistent rules | P0 | policy aura | 5 venture tints | rule icon panel | persistent state clear | PROMPT_CARD_FRAME |
| EOC_CARD_Frame_Risk | Card | Shared | Risk cards | P0 | legal/reputation risk | red/glitch variants | red marked panel | risk visible immediately | PROMPT_CARD_FRAME |
| EOC_CARD_Frame_Reaction | Card | Shared | Event choices | P0 | crisis response | clean/risky/premium | button-card | event panel compatible | PROMPT_CARD_FRAME |
| EOC_CARD_Icon_TypeSet | Card | Shared | Behavior icons | P0 | type recognition | 5 icons | text labels | simple icon language | PROMPT_CARD_ICONS |
| EOC_SLOT_Frame_Operation | Card | Shared | Operation slot | P0 | capacity | empty, filled, blocked | blue outline | target highlight support | PROMPT_SLOT_FRAME |
| EOC_SLOT_Frame_Staff | Card | Shared | Staff slot | P0 | staff capacity | empty, stress, quit risk | blue outline | stress chip support | PROMPT_SLOT_FRAME |
| EOC_SLOT_Frame_Marketing | Card | Shared | Marketing slot | P0 | demand pull | empty, active, expired | blue outline | flow linked state | PROMPT_SLOT_FRAME |
| EOC_SLOT_Frame_Supplier | Card | Shared | Supplier slot | P0 | quality/cost | clean, cheap, premium | blue outline | quality/risk marker | PROMPT_SLOT_FRAME |
| EOC_SLOT_Frame_TempEffect | Card | Shared | Crisis/temp | P0 | timed effects | timer, warning | chip slot | duration readable | PROMPT_SLOT_FRAME |
| EOC_CARD_LinkLine | Card | Shared | Card-world linkage | P0 | hover/inspect | blue, red, warning | thin line | not cluttered | PROMPT_CARD_LINK |

## 7. UI Assets

| Asset ID | Category | Venture | Usage | Priority | Gameplay State | Variants | Placeholder | Final Requirement | Prompt Ref |
|---|---|---|---|---:|---|---|---|---|---|
| EOC_UI_HUD_Cash | UI | Shared | Cash display | P0 | economy | gain, loss, low | coin glyph | pulse states | PROMPT_UI_HUD |
| EOC_UI_HUD_RatingStars | UI | Shared | Rating | P0 | reputation | glow, crack, drop | TMP stars | world/HUD consistent | PROMPT_UI_HUD |
| EOC_UI_HUD_StaffStability | UI | Shared | Staff health | P0 | stress | stable, warning, crisis | person icon | matches stress aura | PROMPT_UI_HUD |
| EOC_UI_HUD_LegalRisk | UI | Shared | Risk | P0 | legal pressure | safe, warning, audit | warning icon | glitch/red state | PROMPT_UI_HUD |
| EOC_UI_HUD_MarketShare | UI | Shared | Market share | P0 | gray/blue/red split | neutral, shift, dominant | 3-color bar | matches customers | PROMPT_UI_HUD |
| EOC_UI_CardOfferBand | UI | Shared | Card selection | P0 | card offer | 3,4,5 cards | bottom panel | does not block board | PROMPT_UI_CARD_OFFER |
| EOC_UI_CardInspectTooltip | UI | Shared | Hover detail | P0 | inspect | valid, invalid, risky | small panel | short, no clutter | PROMPT_UI_CARD_OFFER |
| EOC_UI_SlotReplacementPanel | UI | Shared | Full slot choice | P0 | replace/upgrade/merge/sell | 4 actions | popup panel | consequence visible | PROMPT_UI_SLOT |
| EOC_UI_EventPanel | UI | Shared | Event choice | P0 | crisis decision | 1-3 choices | panel | problem source visible | PROMPT_UI_EVENT |
| EOC_UI_EndTurnSummary | UI | Shared | Resolve summary | P1 | turn result | compact/full | modal | short, visual-first | PROMPT_UI_SUMMARY |
| EOC_UI_VentureSelectCard | UI | Shared | Venture select | P1 | run setup | 5 ventures | card panel | sector pressure readable | PROMPT_UI_VENTURE |
| EOC_UI_ExitMetaScreen | UI | Meta | Holding exit | P2 | meta progression | sell, continue, invest | screen | celebratory but clean | PROMPT_UI_META |

## 8. VFX Assets

| Asset ID | Category | Venture | Usage | Priority | Gameplay State | Variants | Placeholder | Final Requirement | Prompt Ref |
|---|---|---|---|---:|---|---|---|---|---|
| EOC_VFX_CustomerColorShift | VFX | Shared | Market ownership | P0 | gray/blue/red migration | neutral->blue, blue->red | material tween | smooth, readable | PROMPT_VFX_CORE |
| EOC_VFX_MarketingPull | VFX | Shared | Demand pull | P0 | campaign | player, rival | arrows | subtle curved flow | PROMPT_VFX_CORE |
| EOC_VFX_ReviewBurst | VFX | Shared | Rating change | P0 | review feedback | positive, negative, fake | floating icons | minimal text | PROMPT_VFX_CORE |
| EOC_VFX_StressAura | VFX | Shared | Staff stress | P0 | stress | medium, high, breaking | red ring | readable from overview | PROMPT_VFX_CORE |
| EOC_VFX_QueuePressure | VFX | Shared | Overload | P0 | delay | small, medium, severe | floor pulse | anchors queue | PROMPT_VFX_CORE |
| EOC_VFX_RiskGlitch | VFX | Shared | Black hat/legal | P0 | fake/risk | mild, severe | flicker | not visually noisy | PROMPT_VFX_RISK |
| EOC_VFX_LegalMarker | VFX | Shared | Audit | P1 | legal event | warning, audit incoming | red stamp | event anchor | PROMPT_VFX_RISK |
| EOC_VFX_RatingCrackGlow | VFX | Shared | Reputation | P0 | rating damage/recovery | crack, glow | star pulse | HUD/world match | PROMPT_VFX_CORE |
| EOC_VFX_ScaleUpgrade | VFX | Meta | Growth stage | P1 | scale up | local favorite, chain, holding | confetti | short, readable | PROMPT_VFX_META |

## 9. Material and Lighting Assets

| Asset ID | Category | Venture | Usage | Priority | Gameplay State | Variants | Placeholder | Final Requirement | Prompt Ref |
|---|---|---|---|---:|---|---|---|---|---|
| EOC_MAT_Customer_GrayBlueRedGold | Material | Shared | Customer ownership | P0 | market share | gray, blue, red, gold | flat colors | Toy Studio readable | BRIEF_MAT_CUSTOMER |
| EOC_MAT_ToyPlastic_Base | Material | Shared | Characters/props | P0 | toy style | matte, satin | URP lit | soft highlights | BRIEF_MAT_TOY |
| EOC_MAT_Cardboard_Wood | Material | Shared | board/slot | P0 | tabletop | light, dark | flat tan | not beige-dominant | BRIEF_MAT_BOARD |
| EOC_MAT_UI_GlowAccents | Material | Shared | UI/VFX | P0 | highlights | blue, red, gold, warning | emissive color | no bloom overload | BRIEF_MAT_UI |
| EOC_LIGHT_ToyStudio_BaseRig | Lighting | Shared | Scene lighting | P0 | readability | base, event-safe | directional + ambient | fixed warm rig | BRIEF_LIGHT_TOY |
| EOC_LIGHT_LocalCrisisAccent | Lighting | Shared | Crisis highlights | P0 | stress/legal/rating | red, purple, gold | point light | local only | BRIEF_LIGHT_TOY |

## 10. Audio Assets

| Asset ID | Category | Venture | Usage | Priority | Gameplay State | Variants | Placeholder | Final Requirement | Prompt Ref |
|---|---|---|---|---:|---|---|---|---|---|
| EOC_AUDIO_CardHover | Audio | Shared | Card inspect | P1 | hover | valid, invalid | soft tick | light toy UI | BRIEF_AUDIO_UI |
| EOC_AUDIO_CardCommit | Audio | Shared | Card placement | P0 | commit | install, burst, risk | click | satisfying short snap | BRIEF_AUDIO_UI |
| EOC_AUDIO_SlotReplace | Audio | Shared | Replacement | P0 | replace/merge | replace, upgrade, sell | pop | slight tension | BRIEF_AUDIO_UI |
| EOC_AUDIO_Cash | Audio | Shared | Economy | P0 | gain/loss/low | 3 variants | coin tick | readable but not arcade | BRIEF_AUDIO_UI |
| EOC_AUDIO_Review | Audio | Shared | Rating | P0 | up/down/fake | 3 variants | chime/thud | emotional feedback | BRIEF_AUDIO_EVENT |
| EOC_AUDIO_CrisisAlert | Audio | Shared | Event start | P0 | crisis | staff, customer, legal | alert | no alarm fatigue | BRIEF_AUDIO_EVENT |
| EOC_AUDIO_CustomerCrowd | Audio | Shared | District ambience | P1 | flow | calm, busy, angry | murmur loop | subtle, loopable | BRIEF_AUDIO_AMBIENCE |
| EOC_AUDIO_StaffStress | Audio | Shared | Staff pressure | P1 | stress | medium, high | sigh/tick | not annoying | BRIEF_AUDIO_EVENT |
| EOC_AUDIO_ScaleUpgrade | Audio | Meta | Growth | P2 | scale stage | local, chain, holding | flourish | short celebratory | BRIEF_AUDIO_META |

## 11. Camera and Debug Helper Assets

| Asset ID | Category | Venture | Usage | Priority | Gameplay State | Variants | Placeholder | Final Requirement | Prompt Ref |
|---|---|---|---|---:|---|---|---|---|---|
| EOC_DEBUG_CameraAnchorMarker | Debug | Shared | Camera setup | P0 | camera anchors | player, market, rival, event | gizmo sphere | editor-only marker | BRIEF_DEBUG |
| EOC_DEBUG_FlowPathMarker | Debug | Shared | Customer paths | P0 | flow authoring | neutral, blue, red | gizmo line | editor-only marker | BRIEF_DEBUG |
| EOC_DEBUG_EventAnchorMarker | Debug | Shared | Event setup | P0 | event focus | 5 venture types | gizmo cube | editor-only marker | BRIEF_DEBUG |
| EOC_DEBUG_SlotLinkMarker | Debug | Shared | Card-world links | P1 | linked highlight | line, endpoint | gizmo line | editor-only marker | BRIEF_DEBUG |

## 12. P0 First Implementable Batch

P0 batch must ship before deeper venture expansion:

- Neutral/player/rival customers with tintable materials.
- Fast Food cook, cashier, waiter, courier.
- Cash register, queue markers, grill, counter, table set, delivery shelf.
- Shared district board, flow paths, player/rival base shops.
- Card frames, slot frames, card offer band, slot replacement panel.
- HUD cash/rating/staff/legal/market share.
- Customer color shift, marketing pull, stress aura, review burst, queue pressure.
- Card commit, slot replace, cash, review, crisis alert audio.
- Toy Studio base light rig.

## 13. Completeness Checklist

- Every persistent card has a card frame, slot frame, linked world asset, and hover highlight.
- Every venture has at least one readable P1 module for Operation, Staff, Marketing, Supplier, Temp Effect.
- Every event category has at least one visual anchor, VFX feedback, audio cue, and UI panel state.
- Every customer ownership state is readable under Toy Studio lighting.
- Every P0 asset has a placeholder path and final production target.
- Every final asset has a prompt or 3D brief reference in `ASSET_PROMPTS.md`.
