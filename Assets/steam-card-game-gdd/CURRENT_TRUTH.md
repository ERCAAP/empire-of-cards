# CURRENT TRUTH
# Empire of Cards

> Son guncelleme: 2026-05-22
> Bu dosya repo icin aktif tasarim ve runtime truth index'idir.

---

## Authoritative Gameplay Docs

Bu dosyalar aktif gameplay source of truth'tur:

- `GDD.md`
- `TECHNICAL_MAPPING.md`
- `businesses/fast_food.md`
- `businesses/cafe.md`
- `businesses/market_bakkal.md`
- `businesses/tech_app.md`
- `businesses/giyim_magazasi.md`

## Active Implementation Audits

Bu dosyalar aktif repo/uygulama referansidir:

- `REVERSE_ENGINEERING_DOSSIER.md`
- `IMPLEMENTATION_GAP_AUDIT.md`

## Reference Docs

Bu dosyalar yardimci referanstir. Gameplay truth degildir:

- `UI_UX_3D_ART_BIBLE.md`
- `UI_UX_3D_PROMPT_PACK.md`
- `ART_DIRECTION_GUIDE.md`
- `ASSET_LIST.md`
- `COMBO_MATRIX.md`
- `MARKET_RESEARCH.md`
- `PAPER_PROTOTYPE.md`
- `PREMORTEM.md`
- `PREMORTEM_PROMPT.md`
- `NEXT_SESSION_PROMPTS.md`

## Legacy Docs

Asagidaki gameplay dokumanlari aktif kokten cikarildi. Bunlar tarihsel referanstir:

- `legacy/LEGACY_BALANCE.md`
- `legacy/LEGACY_CARD_LIST.md`
- `legacy/LEGACY_BALANCE_TABLE_V3.md`
- `legacy/LEGACY_CARD_LIST_V3.md`
- `legacy/LEGACY_TEST_PLAN_V3.md`
- `legacy/LEGACY_NARRATIVE_V3.md`

## Official Scene Flow

Aktif build zinciri:

1. `Assets/Scenes/Boot.unity`
2. `Assets/Scenes/MainMenu.unity`
3. `Assets/Scenes/Game.unity`

Legacy sahne:

- `Assets/Scenes/LegacyGameScene.unity`

## Runtime-Built Gameplay Sources

Gameplay sahnesinin ana runtime kaynaklari:

- `GameSceneBootstrap`
- `ManagerFactory`
- `SceneRuntimeFactory`
- `HUDBuilder`
- `WiringService`

## Naming Clarifications

- `MarketShareVisualizer` aktif market-share block gorsellestirme rolu olarak kabul edilir.
- `TerritoryManager` yalnizca compatibility katmanidir.

## Removed Dormant Runtime Scripts

2026-05-22 cleanup sprintinde su orphan/dormant scriptler repo'dan cikarildi:

- `CustomerLoyaltySystem`
- `InflationSystem`
- `DebtTracker`
- `MarketPool`
- `TaxCalculator`
- `BoardQueries`
- `ClosureManager`
- `EmployeeTenure`
- `RivalDecisionTree`
- `RivalEconomy`
- `RivalGrowth`
- `RivalDialogue`

Bu sistemler aktif runtime truth'un parcasi degildir. Gerekirse `Economy Core` veya `Rival Depth` milestone'unda v4 venture-first tasarimla yeniden kurulmalidir.

## Venture Priority

Phase 1 odagi:

- Fast Food
- Cafe
- Grocery Store

Phase 2 / ilk implementasyon hedefi degil:

- Tech App
- Clothing Store
