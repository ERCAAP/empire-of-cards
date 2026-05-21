# PLANS.md — Empire of Cards Full Development Roadmap

> Her faz icinde isler oncelik sirasina gore siralanmistir.
> Bir fazi bitirmeden sonrakine gecme.
> Her is icin: GDD oku → Mevcut kodu oku → Yaz → Wire et → Test et

---

## PHASE 0: AUDIT & DIAGNOSIS (Mevcut Durum Analizi)

**Goal:** Understand exactly what exists, what's broken, what's missing.

- [ ] 0.1 — Scan ALL .cs files, list every class and its responsibility
- [ ] 0.2 — Map every EventBus event → find which are published, which are subscribed, which are orphaned
- [ ] 0.3 — Map WiringService → find all wired references, find unwired managers
- [ ] 0.4 — List every system GDD describes that has NO code implementation
- [ ] 0.5 — List every system that has code but is INCOMPLETE (stub, placeholder, partial)
- [ ] 0.6 — Check GameEnums.cs → verify all enums match GDD requirements
- [ ] 0.7 — Check all venture files (fast_food.md, cafe.md, market_bakkal.md) → compare to code
- [ ] 0.8 — Produce a gap report: GDD requirement → code status (done/partial/missing)

**Output:** Complete gap analysis document. This drives all subsequent phases.

---

## PHASE 1: CORE SYSTEMS COMPLETION (Cekirdek Sistemler)

**Goal:** Every foundational system works end-to-end.

### 1A. Turn System (Tur Sistemi)
- [ ] 1A.1 — Verify 7-phase turn cycle works: Draw → Planning → Play → Resolve → CrisisReaction → Rival → MarketUpdate
- [ ] 1A.2 — Each phase must have clear entry/exit conditions
- [ ] 1A.3 — Phase transitions fire correct EventBus events
- [ ] 1A.4 — Turn counter increments correctly, win/lose checked at turn end
- [ ] 1A.5 — Hard cap at turn 30

### 1B. Card System (Kart Sistemi)
- [ ] 1B.1 — CardData ScriptableObjects for ALL card types in GDD
- [ ] 1B.2 — CardDataFactory creates correct starter decks per venture
- [ ] 1B.3 — Card draw system (DeckManager) — draw pile, discard, reshuffle
- [ ] 1B.4 — Card play system — drag Card3D to SlotZone3D, validate placement
- [ ] 1B.5 — Card resolve system — each card type resolves its effects
- [ ] 1B.6 — Card types: Business, Employee, Action, Upgrade, Event

### 1C. Board & Slots (Masa & Slotlar)
- [ ] 1C.1 — 5 slot families work: Operation, Staff, Marketing, Supplier, TempEffect
- [ ] 1C.2 — Slot capacity limits enforced (starting: 4+5+3+2+3 = 17)
- [ ] 1C.3 — Slot expansion system (GDD Section 4.5)
- [ ] 1C.4 — Cards snap to correct slot types
- [ ] 1C.5 — Board state queryable (BoardQueries.cs)
- [ ] 1C.6 — DropZoneType enum integrated with physical board

### 1D. Resource System (Kaynak Sistemi)
- [ ] 1D.1 — PlayerResources: Money, Actions, Slots tracked correctly
- [ ] 1D.2 — Resource changes fire EventBus events
- [ ] 1D.3 — UI reflects resource changes in real-time
- [ ] 1D.4 — Action points limit cards played per turn

---

## PHASE 2: HUMAN RESOURCES SYSTEM (Eleman Yonetimi)

**Goal:** Complete staff lifecycle — hire to fire.

### 2A. Hiring Pipeline (Ise Alim Sureci)
- [ ] 2A.1 — Job posting card (ilan verme karti)
- [ ] 2A.2 — Applicant pool generation (basvuru havuzu olusturma)
- [ ] 2A.3 — Applicant quality based on: salary offered, reputation, location, season
- [ ] 2A.4 — Interview/trial period mechanic (1-2 turn deneme suresi)
- [ ] 2A.5 — Hire confirmation → employee card placed in Staff slot

### 2B. Staff Levels & Progression (Kademe Sistemi)
- [ ] 2B.1 — Stajyer (Intern): cheap, low skill, needs training, makes mistakes
- [ ] 2B.2 — Calisan (Regular): standard cost, reliable
- [ ] 2B.3 — Usta (Master): expensive, high quality output, specialties
- [ ] 2B.4 — Sube Muduru (Branch Manager): unlocks branch expansion
- [ ] 2B.5 — Promotion system: training + time + money → level up
- [ ] 2B.6 — Usta secimi (Master Chef selection): unique skills per usta, affects menu quality

### 2C. Salary & Payroll (Maas & Bordro)
- [ ] 2C.1 — Each employee has base salary + possible bonuses
- [ ] 2C.2 — Salary paid at MarketUpdate phase each turn
- [ ] 2C.3 — Underpaying = morale drop, overpaying = loyalty boost
- [ ] 2C.4 — Salary inflation over time (linked to InflationSystem)
- [ ] 2C.5 — Salary negotiation events (employee demands raise)

### 2D. Employee State & Morale (Calisan Durumu)
- [ ] 2D.1 — Morale system: happy → neutral → unhappy → quitting
- [ ] 2D.2 — Burnout: overworked employees lose quality, may quit
- [ ] 2D.3 — Training events boost morale
- [ ] 2D.4 — Conflicts between employees
- [ ] 2D.5 — Sick days / absent employees

### 2E. Firing & Separation (Isten Cikarma)
- [ ] 2E.1 — Firing costs tazminat (severance)
- [ ] 2E.2 — Firing good employees hurts reputation
- [ ] 2E.3 — Employee can quit on their own
- [ ] 2E.4 — Rival can poach your employees (offer higher salary)
- [ ] 2E.5 — Wrongful termination → lawsuit risk

---

## PHASE 3: ECONOMY DEEP SYSTEM (Derin Ekonomi)

**Goal:** Real financial simulation.

### 3A. Cash Flow (Nakit Akisi)
- [ ] 3A.1 — Income: customer payments based on menu prices × demand
- [ ] 3A.2 — Fixed costs: rent, utilities, insurance premiums
- [ ] 3A.3 — Variable costs: ingredient costs, salary, marketing spend
- [ ] 3A.4 — Net profit calculated end of each turn
- [ ] 3A.5 — Cash flow statement viewable in UI (AnalyticsPanelUI)

### 3B. Banking & Credit (Banka & Kredi)
- [ ] 3B.1 — Bank loan system: borrow money, pay interest
- [ ] 3B.2 — Credit score: based on payment history
- [ ] 3B.3 — Loan denial if credit too low
- [ ] 3B.4 — Debt spiral mechanic: unpaid debt → higher interest → harder to escape
- [ ] 3B.5 — Emergency loan (high interest, fast approval)

### 3C. Tax & Legal Finance (Vergi)
- [ ] 3C.1 — Tax calculation per turn (TaxCalculator.cs)
- [ ] 3C.2 — Tax period system (quarterly audits — TaxPeriodSystem.cs)
- [ ] 3C.3 — Tax evasion option: save money but risk audit
- [ ] 3C.4 — Tax audit event: pay back taxes + penalty if caught
- [ ] 3C.5 — Legal fees for lawsuits

### 3D. Insurance & Risk (Sigorta)
- [ ] 3D.1 — Insurance types: fire, theft, employee injury, customer incident
- [ ] 3D.2 — Premium costs vs. coverage
- [ ] 3D.3 — Claim system when bad events happen
- [ ] 3D.4 — No insurance = full loss when disaster strikes

### 3E. Market Forces (Piyasa Gucleri)
- [ ] 3E.1 — Inflation system: costs rise over time
- [ ] 3E.2 — Seasonal demand shifts
- [ ] 3E.3 — Neighborhood economic events (new housing = more customers, road construction = less traffic)
- [ ] 3E.4 — Stock system for supplies: bulk buy cheaper but ties up cash

---

## PHASE 4: CUSTOMER & MARKET SIMULATION (Musteri Simulasyonu)

**Goal:** Customers feel like real people making real choices.

- [ ] 4.1 — Customer segments: ogrenci, aile, isci, turist, yasli
- [ ] 4.2 — Each segment has: budget, taste preference, time preference, loyalty tendency
- [ ] 4.3 — Customer satisfaction = f(food quality, service speed, price, ambiance, cleanliness)
- [ ] 4.4 — Review/rating system (1-5 stars)
- [ ] 4.5 — Bad reviews compound: 1 bad review = small dip, 3 in a row = reputation crisis
- [ ] 4.6 — Word of mouth: high satisfaction → organic customer growth
- [ ] 4.7 — Customer loyalty program cards
- [ ] 4.8 — Peak hours vs dead hours
- [ ] 4.9 — Customer complaints → resolution cards
- [ ] 4.10 — Market share: your customers / total available customers in district

---

## PHASE 5: FOOD SECTOR DEEP MECHANICS (Yemek Sektoru Detay)

**Goal:** Fast Food / Cafe / Market each feel distinct and real.

### 5A. Kitchen & Production
- [ ] 5A.1 — Menu system: each item = recipe + ingredients + preparation time
- [ ] 5A.2 — Ingredient quality tiers: cheap (low quality), standard, premium
- [ ] 5A.3 — Usta (head chef) specialty bonuses: Italian usta → pasta quality +2
- [ ] 5A.4 — Kitchen capacity: max orders per turn based on equipment + staff
- [ ] 5A.5 — Food waste: unsold prepared food = loss
- [ ] 5A.6 — Recipe unlocking: discover new recipes through upgrades or events

### 5B. Hygiene & Inspection
- [ ] 5B.1 — Hygiene score: affected by staff, equipment age, cleanliness investment
- [ ] 5B.2 — Health inspector visits: random event, checks hygiene score
- [ ] 5B.3 — Fail inspection: fine + forced closure (1-2 turns) + reputation damage
- [ ] 5B.4 — Bribe inspector: risk/reward (if caught = worse penalty)

### 5C. Supplier Chain
- [ ] 5C.1 — Multiple supplier options per ingredient category
- [ ] 5C.2 — Supplier reliability: cheap supplier sometimes delivers late/wrong
- [ ] 5C.3 — Exclusive supplier contracts: better price but locked in
- [ ] 5C.4 — Supply shortage events: seasonal ingredient unavailable
- [ ] 5C.5 — Bulk ordering: cheaper per unit but cash tied up

---

## PHASE 6: MARKETING & GROWTH (Pazarlama)

- [ ] 6.1 — Marketing channel cards with different ROI profiles
- [ ] 6.2 — Digital: Google Ads (fast, expensive), Instagram (slow, cheap), TikTok (viral chance)
- [ ] 6.3 — Traditional: brosur, afis, yerel gazete (local reach, slow)
- [ ] 6.4 — App partnerships: Yemeksepeti/Getir listing (commission cut)
- [ ] 6.5 — Influencer: one-shot big exposure, variable quality
- [ ] 6.6 — Grand opening event: big one-time boost
- [ ] 6.7 — Brand reputation meter: affects all marketing effectiveness
- [ ] 6.8 — Marketing ROI tracking visible to player

---

## PHASE 7: RIVAL AI SYSTEM (Rakip Yapay Zekasi)

- [ ] 7.1 — Rival makes all same decisions player can (hire, fire, price, market, expand)
- [ ] 7.2 — 3 personality types: Agresif, Muhafazakar, Dengeli
- [ ] 7.3 — Rival decision tree reacts to player moves (RivalDecisionTree.cs)
- [ ] 7.4 — Rival economy simulation parallel to player (RivalEconomy.cs)
- [ ] 7.5 — Rival can poach your best employees
- [ ] 7.6 — Rival can start price war
- [ ] 7.7 — Rival visible zone: player sees rival's slots but not exact finances
- [ ] 7.8 — Rival dialogue/taunts (RivalDialogue.cs)
- [ ] 7.9 — Rival can go bankrupt (player wins) or dominate (player loses)

---

## PHASE 8: CRISIS & EVENT SYSTEM (Kriz & Olay)

- [ ] 8.1 — Sector-specific crises (GDD defines per venture)
- [ ] 8.2 — Global crises (economic downturn, pandemic, supply chain break)
- [ ] 8.3 — Positive events (food festival, celebrity visit, viral moment)
- [ ] 8.4 — Crisis requires reaction card or costs escalate
- [ ] 8.5 — Chain reaction: unresolved crisis → second crisis
- [ ] 8.6 — Seasonal events (Ramazan, yaz, kis)
- [ ] 8.7 — Legal events (denetim, dava, vergi kontrolu)

---

## PHASE 9: UI & PRESENTATION (Arayuz)

- [ ] 9.1 — TopBarUI: money, turn counter, market share, rating
- [ ] 9.2 — ActionBarUI: end turn, shop, analytics buttons
- [ ] 9.3 — AnalyticsPanelUI: financial statements, trends, graphs
- [ ] 9.4 — ShopPanel: buy new cards
- [ ] 9.5 — EventPopup: event cards with choices
- [ ] 9.6 — RivalPopup: rival status overview
- [ ] 9.7 — VentureSelectionUI: game start venture picker
- [ ] 9.8 — GameOverScreen: win/lose with stats
- [ ] 9.9 — ClarityPanelUI: board state summary for readability
- [ ] 9.10 — Indicators: customer, legal risk, rating, season
- [ ] 9.11 — Tutorial flow (TutorialManager + TutorialUI)

---

## PHASE 10: AUDIO, VFX, POLISH (Son Kat)

- [ ] 10.1 — Card play sound effects
- [ ] 10.2 — Turn phase transition sounds
- [ ] 10.3 — Crisis alarm sounds
- [ ] 10.4 — Money gain/loss sounds
- [ ] 10.5 — Card3D animations: draw, play, discard, destroy
- [ ] 10.6 — Screen shake on big events
- [ ] 10.7 — Particle effects for combos/chains
- [ ] 10.8 — Ambient neighborhood sounds

---

## PHASE 11: SAVE, BALANCE, SHIP (Son Kontrol)

- [ ] 11.1 — Save/load system (SaveManager.cs)
- [ ] 11.2 — Balance pass: all numbers feel right (costs, income, timing)
- [ ] 11.3 — Difficulty curve: early game teaches, mid game challenges, late game demands mastery
- [ ] 11.4 — 3 full playthroughs without crash or soft-lock
- [ ] 11.5 — Steam build configuration
- [ ] 11.6 — Achievements / milestones

---

## SUBAGENT ASSIGNMENTS (for multi-agent execution)

If running with multiple agents, assign by domain:

| Agent | Domain | Key Files |
|-------|--------|-----------|
| Agent 1: Gameplay | Turn system, cards, board, resolve | Core/, Gameplay/, Data/ |
| Agent 2: Economy | All financial systems, salary, tax | Gameplay/Economy/, Gameplay/Staff/ |
| Agent 3: Content | Card data, venture profiles, balance | Data/, Bootstrap/Data/ |
| Agent 4: Rival AI | Rival behavior, dialogue, economy | Gameplay/Rival/ |
| Agent 5: UI/UX | All panels, popups, indicators | UI/, World/ |
| Agent 6: Polish | Audio, VFX, save, optimization | Audio/, VFX/, Save/ |

**Merge rule:** All agents commit via EventBus. No agent directly calls another agent's managers. WiringService is the ONLY connection point.
