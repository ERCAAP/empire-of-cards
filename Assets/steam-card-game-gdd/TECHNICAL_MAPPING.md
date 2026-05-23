# Empire of Cards Technical Mapping

> Bu doküman GDD kararlarının Unity prototipine nasıl taşınacağını tanımlar. Kod yazımı için nihai API değil, karar seviyesinde teknik sözleşmedir.

## 1. Implementation Direction

Unity tarafında tasarım verileri ağırlıklı olarak ScriptableObject ile taşınmalıdır. Simülasyon state'i runtime C# modellerinde tutulur. Görsel dünya state'i bu runtime state'i dinler ve board üzerinde temsil eder.

Temel ayrım:

- `Definition`: editörde hazırlanan statik tasarım verisi.
- `State`: run sırasında değişen canlı değer.
- `System`: state'i çözen saf gameplay mantığı.
- `Presenter`: state değişimini sahada ve UI'da gösteren katman.

## 2. Core Data Definitions

### 2.1 VentureDefinition

Venture'ın kimliğini ve içerik havuzunu tutar.

Gerekli alanlar:

- `id`
- `displayName`
- `theme`
- `visualPalette`
- `starterDeck`
- `cardPool`
- `eventPool`
- `slotDefinitions`
- `derivedMetrics`
- `rivalBehaviorWeights`
- `startingBusinessState`
- `startingBoardLayout`
- `levelDefinition`
- `assetManifest`
- `uiFlowSet`

Slot definitions:

- `Operation`
- `Staff`
- `Marketing`
- `Supplier`
- `TempEffect`

Her slotun venture'a özel alt isimleri olur. Örnek Fast Food `Operation`: mutfak, servis, oturma, delivery.

### 2.2 CardDefinition

Kart, oyuncunun karar seçeneğidir.

Gerekli alanlar:

- `id`
- `displayName`
- `ventureTags`
- `cardFamily`
- `cost`
- `slotTarget`
- `duration`
- `statEffects`
- `riskTagsAdded`
- `eventHooks`
- `visualEffect`
- `worldPlacementRule`
- `persistenceType`
- `slotBehavior`
- `worldManifestation`
- `replacementRules`
- `expirationRule`
- `offerConditions`
- `rarityOrPhase`

Card families:

- `Install`
- `Burst`
- `Policy`
- `Risk`
- `Reaction`

Kart oynandığında sadece stat değiştirmez; `visualEffect` ve `worldPlacementRule` üzerinden sahada karşılık üretir.

Venture yüzeyi bu teknik tipleri kendi terimleriyle gösterir. Örneğin Fast Food için `Install` kartları ekipman/personel/tedarik olarak, Tech App için feature/infra/team olarak adlandırılabilir.

### 2.3 BusinessState

Oyuncu ve rakip için ayrı tutulur.

Gerekli alanlar:

- `cash`
- `demand`
- `capacity`
- `quality`
- `rating`
- `staffStability`
- `legalRisk`
- `marketShare`
- `activeCards`
- `activeTempEffects`
- `riskTags`
- `staffRoster`
- `customerSentiment`
- `scaleStage`

Rating 0-5 aralığında, diğer ana statlar prototipte 0-100 ölçeğinde tutulabilir. Cash ayrı para birimidir.

### 2.4 DistrictState

Pazar ve müşteri havuzunun canlı durumudur.

Gerekli alanlar:

- `neutralCustomers`
- `playerCustomers`
- `rivalCustomers`
- `trafficPressure`
- `localTrend`
- `playerPull`
- `rivalPull`
- `marketEvents`
- `flowModifiers`
- `customerMoodDistribution`

DistrictState fiziksel müşteri spawn ve renk dağılımı için ana kaynaktır.

### 2.5 EventDefinition

Event sisteminin veri tanımıdır.

Gerekli alanlar:

- `id`
- `category`
- `ventureTags`
- `triggerConditions`
- `priority`
- `cooldownTurns`
- `requiredNarrativeTags`
- `blockedByTags`
- `problemText`
- `cameraBeat`
- `npcBeat`
- `choices`
- `ambientFallback`

Event sadece text içermez. Kamera ve NPC beat verisi zorunludur.

### 2.6 EventChoiceDefinition

Oyuncunun event sırasında seçtiği karar.

Gerekli alanlar:

- `id`
- `displayText`
- `tradeoffLabel`
- `requirements`
- `consequences`
- `visualConsequence`
- `futureTags`

### 2.7 ConsequenceDefinition

Seçim veya kart sonucunu işler.

Gerekli alanlar:

- `statDeltas`
- `cashDelta`
- `customerMigration`
- `actorBehavior`
- `tempEffectAdded`
- `riskTagsAdded`
- `riskTagsRemoved`
- `deckBias`
- `eventCooldownChanges`
- `worldLabel`

### 2.8 BoardSlotState

Sert slot limitlerini ve kalıcı kartların dünya bağlantısını tutar.

Gerekli alanlar:

- `slotId`
- `slotType`
- `subSlotName`
- `occupiedCardId`
- `linkedWorldEntityId`
- `upgradeLevel`
- `stressOrDurability`
- `isLocked`
- `allowedReplacementRules`
- `activeModifiers`

BoardSlotState bir kart yuvası değildir; işletme kararının sahadaki karşılığını takip eden bağ noktasıdır. Staff slotundaki kart çalışan NPC'ye, Operation slotundaki kart objeye, Marketing slotundaki kart district flow modifier'a bağlanır.

### 2.9 WorldManifestationRequest

Kart veya event'in sahada nasıl görüneceğini presenter katmanına taşır.

Gerekli alanlar:

- `requestType`
- `sourceCardOrEventId`
- `targetBoardZone`
- `targetSlotId`
- `linkedStateId`
- `prefabKey`
- `animationKey`
- `colorShift`
- `worldLabel`
- `duration`
- `cameraHint`

Request tipleri:

- `Actor`
- `Object`
- `Flow`
- `Aura`
- `Marker`
- `Temp`

### 2.10 LevelDefinition

Level layout ve kamera anchor verisini taşır.

Gerekli alanlar:

- `id`
- `ventureId`
- `scaleStageLayouts`
- `playerBusinessAnchors`
- `districtSpawnAnchors`
- `rivalBusinessAnchors`
- `slotBoardAnchor`
- `eventAnchors`
- `cameraProfiles`
- `flowPaths`

Kaynak doküman: `LEVEL_DESIGN.md`.

### 2.11 AssetManifestDefinition

Toy Diorama asset key'lerini prefab/VFX/audio referanslarına bağlar.

Gerekli alanlar:

- `actorPrefabKeys`
- `businessObjectKeys`
- `cardFrameKeys`
- `uiIconKeys`
- `vfxKeys`
- `audioKeys`
- `placeholderFallbacks`

Kaynak doküman: `ASSET_MANIFEST.md`.

### 2.12 UIFlowDefinition

Ana UI akışlarını data seviyesinde tanımlar.

Gerekli alanlar:

- `id`
- `screenType`
- `panelPrefabKey`
- `allowedInputs`
- `transitionRule`
- `blocksSimulation`
- `cameraHint`
- `worldOverlayMode`

Kaynak doküman: `UI_UX_DESIGN.md`.

### 2.13 CameraProfileDefinition

Cinemachine virtual camera profillerini tanımlar.

Gerekli alanlar:

- `id`
- `cameraMode`
- `orthographicSize`
- `pitch`
- `yaw`
- `targetAnchor`
- `blendInSeconds`
- `priority`
- `safeFrameRule`

Kamera profilleri `BoardOverview`, `PlayerBusinessFocus`, `CardInspect`, `SlotPlacement`, `EventFocus`, `MarketFlow`, `RivalReaction`, `ScaleView` modlarını kapsar. Kaynak doküman: `CAMERA_LIGHTING_TURN_FLOW.md`.

### 2.14 LightingStateDefinition

Toy Studio ışık ve lokal vurgu state'lerini tanımlar.

Gerekli alanlar:

- `id`
- `baseLightRig`
- `playerAccent`
- `rivalAccent`
- `neutralDistrictLight`
- `localHighlightType`
- `duration`
- `intensityCurve`
- `color`

MVP'de gün/gece döngüsü yoktur; lighting state'ler kriz ve feedback vurgusu için kullanılır.

### 2.15 TurnPhaseDefinition

Tur fazlarının kamera, UI ve simulation speed davranışlarını tanımlar.

Gerekli alanlar:

- `phaseId`
- `cameraProfileId`
- `simulationSpeed`
- `uiMode`
- `allowsPlayerInput`
- `maxDuration`
- `nextPhaseRule`

Fazlar: `Planning`, `CardOffer`, `CardInspect`, `SlotCommit`, `Resolve`, `Event`, `RivalReaction`, `EndTurn`.

## 3. Runtime Systems

### 3.1 TurnController

Tur sırasını yönetir:

```text
StartTurn
-> OfferCards
-> AwaitPlayerDecision
-> ApplyCard
-> ResolveSimulation
-> EvaluateEvents
-> ResolveEvent
-> ResolveRival
-> UpdateMarketShare
-> EndTurn
```

### 3.2 CardOfferSystem

Kart tekliflerini board state'e göre seçer.

Girdi:

- venture definition
- player business state
- district state
- active pressure tags
- run phase

Çıktı:

- 3-5 kartlık teklif listesi.

Kural: En az bir kart mevcut ana baskıya cevap verebilmelidir. Kalan kartlar stratejik alternatif veya riskli kısa yol olabilir.

### 3.3 CardResolutionSystem

Kart etkisini uygular:

- cost kontrolü
- slot uygunluğu
- stat effect
- tag ekleme/çıkarma
- temp effect ekleme
- visual event request
- board placement request

### 3.4 CardLifecycleSystem

Kartın tekliften expire olana kadar tüm yaşam döngüsünü yönetir.

Sorumluluklar:

- offer edilen kartın inspect preview verisini üretmek
- slot uygunluğunu kontrol etmek
- slot boşsa commit etmek
- slot doluysa replace/upgrade/merge/discard kararını istemek
- kalıcı kartı BoardSlotState'e bağlamak
- world manifestation request üretmek
- duration/expiration rule süresi dolan kartları temizlemek
- kartın event hook ve risk tag'lerini run state'e yazmak

Lifecycle sırası:

```text
Offer
-> Inspect
-> PlayIntent
-> ValidateSlot
-> ResolveReplacementIfNeeded
-> CommitToSlot
-> CreateWorldManifestationRequest
-> ApplyGameplayEffects
-> TrackPersistenceOrExpiry
```

### 3.5 SimulationResolveSystem

Ana ekonomi zincirini çözer:

1. Demand pull hesapla.
2. Customer distribution hesapla.
3. Capacity ve overload hesapla.
4. Staff stress delta hesapla.
5. Quality outcome hesapla.
6. Rating delta hesapla.
7. Cash revenue/cost hesapla.
8. Market share delta hesapla.
9. Event trigger context üret.

### 3.6 EventDirector

Event seçimi ve sequence yönetimi yapar.

Sorumluluklar:

- Trigger context'i event pool ile eşleştirir.
- Priority ve cooldown uygular.
- Aynı anda çok event varsa en kritik olanı seçer.
- Küçük event'leri ambient fallback olarak çözer.
- Micro-cinematic sequence'i başlatır.
- Oyuncu seçimini consequence olarak sisteme döker.

### 3.7 RivalAISystem

Rakip kararını seçer.

Inputs:

- venture behavior weights
- player market share
- rival business state
- district trend
- recent player choices

Behavior families:

- aggressive marketing
- premium quality
- cheap expansion
- defensive stabilization
- risky shortcut
- staff poaching

Rakip hamlesi mutlaka district veya rival business üzerinde görsel sinyal üretmelidir.

### 3.8 WorldPresenter

Runtime state'i sahaya çevirir.

Alt presenter'lar:

- `CustomerFlowPresenter`
- `BusinessSlotPresenter`
- `StaffStressPresenter`
- `RatingPresenter`
- `MarketSharePresenter`
- `EventCinematicPresenter`
- `RivalPresenter`

Presenter katmanı gameplay kararlarını hesaplamaz; sadece gösterir.

### 3.9 CameraDirector

Cinemachine virtual camera geçişlerini yönetir.

Sorumluluklar:

- TurnPhaseDefinition'a göre aktif kamerayı seçmek.
- EventDirector ve CardLifecycleSystem'den gelen focus requestleri sıraya almak.
- Aynı turdaki güçlü kamera geçişi limitini uygulamak.
- BoardOverview'a dönüşleri yumuşak yapmak.

### 3.10 LightingDirector

Toy Studio ışık rig'ini ve lokal feedback vurgularını yönetir.

Sorumluluklar:

- Base light rig'i sabit tutmak.
- Player/rival/neutral accent ışıklarını yönetmek.
- Staff stress, legal risk, rating damage/recovery gibi lokal highlight requestlerini oynatmak.
- Krizlerde global sahneyi karartmamak.

### 3.11 TurnPhaseDirector

Tur fazlarının simulation speed, UI mode ve camera profile davranışlarını yönetir.

Sorumluluklar:

- Planning'de canlı simülasyonu normal/0.75 hızda tutmak.
- Card Offer ve Inspect sırasında slow tactical time kullanmak.
- Resolve penceresini 6-12 sn aralığında oynatmak.
- Event sırasında pause veya 0.1 hız uygulamak.
- EndTurn sonrası CardOffer hazırlığına dönmek.

## 4. Visual Request Model

Gameplay sistemleri direkt animasyon oynatmamalıdır. Bunun yerine visual request üretir.

Örnek request tipleri:

- `SpawnCardObject`
- `MoveCustomerFlow`
- `ShiftCustomerColor`
- `ShowWorldLabel`
- `FocusCamera`
- `PlayStaffStressAnimation`
- `ShowReviewBurst`
- `ShowRivalCampaign`
- `ApplyBusinessScar`

Kalıcı kartlar için visual request sadece spawn anında değil, slot-card-world bağlantısı boyunca yaşar. Oyuncu slot kartına hover yaptığında linked world entity highlight edilmelidir.

Bu ayrım test edilebilir gameplay mantığı ile esnek sunum katmanını ayırır.

## 5. Event Pipeline

```text
SimulationResolveSystem
-> EventTriggerContext
-> EventDirector.SelectEvent()
-> EventCinematicPresenter.PlayIntro()
-> EventChoiceUI.Show()
-> PlayerChoice
-> ConsequenceResolutionSystem.Apply()
-> WorldPresenter.PlayConsequence()
-> NewPressureTags stored
```

Event sırasında zaman tamamen durmak zorunda değildir. Prototipte karar paneli açıldığında simulation pause, ambient animasyonlar slow motion çalışabilir.

## 6. Customer Migration Rules

Customer migration market share'in fiziksel karşılığıdır.

Minimum model:

- `neutralToPlayer`
- `neutralToRival`
- `playerToRival`
- `rivalToPlayer`
- `playerToNeutral`
- `rivalToNeutral`

Migration sebebi world label ile gösterilmelidir:

- `Better Rating`
- `Long Queue`
- `Rival Discount`
- `Viral Review`
- `Bad Service`

## 7. Derived Metrics

Venture'a özel metrikler ana statlardan türetilir. Bunlar ikinci ekonomi sistemi değil, okunabilir lens olmalıdır.

Örnek Fast Food:

- `Service Speed = capacity / demand`
- `Google Score = rating`
- `Ingredient Trust = supplier quality + legal cleanliness`
- `Kitchen Pressure = demand - operation capacity`

Örnek Tech App:

- `App Stability = operation + staff - overload`
- `Store Rating = rating`
- `Churn = low quality + bad support + low rating`
- `Infra Cost = backend scale + cloud supplier`

## 8. Suggested Unity Assets

Klasör önerisi:

```text
Assets/EmpireOfCards/
  Scripts/
    Definitions/
    Runtime/
    Systems/
    Presenters/
    UI/
  Data/
    Ventures/
    Cards/
    Events/
    Levels/
    Assets/
  Prefabs/
    Customers/
    Staff/
    Board/
    Cards/
    UI/
    VFX/
  Scenes/
```

GDD dokümanları `Assets/steam-card-game-gdd/` altında kalır.

Production dokümanları:

- `LEVEL_DESIGN.md`: LevelDefinition ve kamera/anchor kararları için kaynak.
- `CODE_ARCHITECTURE.md`: klasör, state, system ve presenter mimarisi için kaynak.
- `ASSET_MANIFEST.md`: AssetManifestDefinition ve prefab üretimi için kaynak.
- `ASSET_PROMPTS.md`: concept prompt ve 3D production brief referansları için kaynak.
- `ASSET_PRODUCTION_PIPELINE.md`: naming, import, placeholder/final asset ve QA süreci için kaynak.
- `UI_UX_DESIGN.md`: UIFlowDefinition ve panel/flow davranışları için kaynak.

## 9. Package Use

Mevcut paketlerle uyumlu öneri:

- Cinemachine: event focus, board overview, market flow view.
- DOTween: kart placement, UI pulse, müşteri renk geçişleri, kamera yardımcı animasyonları.
- URP: Toy Diorama lighting ve stylized material dili.
- UGUI veya UI Toolkit: prototip event paneli ve kart seçimi.
- Unity AI Navigation: müşteri ve çalışan pathing prototipi.
- Input System: kamera ve kart seçimi inputları.

Kamera üretim kararı:

- Cinemachine virtual cameras kullanılacak.
- Ana kamera orthographic ve sabit izometrik olacak.
- Serbest orbit kamera MVP kapsamında yok.

Işık üretim kararı:

- URP üzerinde sıcak Toy Studio base light rig kurulacak.
- Kriz ve feedback için lokal VFX/point/accent light kullanılacak.
- Gün/gece döngüsü MVP kapsamında yok.

## 10. Prototype Acceptance Criteria

İlk teknik prototip şu davranışları kanıtlamalıdır:

- Oyuncu kart seçer ve kart sahada görsel etki üretir.
- District'te gri müşteriler oyuncu/rakip yönüne mavi/kırmızı akar.
- Demand capacity'yi aşınca kuyruk ve stress artar.
- Staff stability düşükse çalışan krizi event'i tetiklenir.
- Event kamera odağı, NPC beat, seçim ve görünür consequence üretir.
- Rating düşünce müşteri akışı rakibe kayar.
- Rakip en az bir görünür hamle yapar.

## 11. Blueprint Reference

Kod üretimine başlamadan önce `IMPLEMENTATION_BLUEPRINT.md` kaynak kabul edilir. Bu dosyadaki karar seviyesi modeller production C# sözleşmesine şu şekilde bağlanır:

- Enum contract: `VentureType`, `CardBehaviorType`, `SlotType`, `TurnPhase`, `EventCategory`, `CustomerOwnership`, `CameraMode` ve diğer enumlar doğrudan C# dosyalarına taşınır.
- ScriptableObject contract: `VentureDefinitionSO`, `CardDefinitionSO`, `EventDefinitionSO`, `LevelDefinitionSO`, `AssetManifestSO`, `UIFlowDefinitionSO`, `CameraProfileSO`, `LightingStateSO`, `CardPoolSO`, `RivalBehaviorProfileSO`.
- Runtime state contract: `RunState`, `BusinessState`, `DistrictState`, `BoardSlotState`, `StaffState`, `CustomerFlowState`, `DeckState`, `ActiveEventState`, `TempEffectState`.
- Scene contract: `Gameplay_Diorama.unity` root hierarchy ve P0 prefab listesi blueprint'teki isimlerle kurulmalıdır.

Bu mapping dosyası sistem davranışını açıklar; blueprint ise geliştiricinin dosya/sınıf üretirken karar vermesini engelleyen kesin listedir.

## 12. Visual Reference Data Binding

`VISUAL_REFERENCE_BIBLE.md` production screen composition için kaynak kabul edilir.

Data bağlantıları:

- `LevelDefinitionSO` tabletop frame, road intersection, player/rival anchors ve slot board anchor'larını referanslar.
- `AssetManifestSO` tabletop, left HUD, right panel, bottom card rail ve slot board pedestal prefab key'lerini taşır.
- `UIFlowDefinitionSO` left HUD, bottom card hand, right simulation/event panel modlarını phase'e göre açar.
- `CameraProfileSO` BoardOverview'da sol HUD, sağ panel ve alt card bandı için safe frame kuralı taşır.
- `WorldManifestationDefinition` kart commit sonrası slot ve world entity arasındaki link feedback'ini tetikler.
