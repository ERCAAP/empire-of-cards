# Empire of Cards Core Implementation Task List

> Amaç: Core mekanik için gerekli scriptleri, ScriptableObjectleri, runtime state modellerini, sistemleri, presenter katmanını, sahne hiyerarşisini ve testleri uygulanabilir sıraya çevirmek.
>
> Kapsam: İlk oynanabilir vertical slice Fast Food üzerinden kurulacak; mimari 5 venture'ı destekleyecek şekilde modüler kalacak.

## 1. Engineering Principles

### 1.1 Core Pattern

Oyun kodu dört ana katmana ayrılır:

```text
Definitions
-> Runtime State
-> Systems
-> Presenters
```

- `Definitions`: ScriptableObject data. Venture, kart, event, level, asset, UI flow burada tanımlanır.
- `Runtime State`: Run sırasında değişen saf C# state. MonoBehaviour değildir.
- `Systems`: Gameplay hesabı ve karar mantığı. Mümkün olduğunca Unity sahnesinden bağımsızdır.
- `Presenters`: Runtime state ve visual requestleri sahneye/UI'a çevirir.

Kural:

- Systems doğrudan prefab instantiate etmez.
- Presenters ekonomi hesabı yapmaz.
- ScriptableObject data runtime'da mutate edilmez.
- Gameplay sonucu önce state'e, sonra visual request'e, sonra presenter'a akar.

### 1.2 State Machine

Turn flow `TurnPhase` state machine ile yönetilir:

```text
Planning
-> CardOffer
-> CardInspect
-> SlotCommit
-> Resolve
-> Event
-> RivalReaction
-> EndTurn
```

Her phase şunları belirler:

- Oyuncu input alır mı?
- Simulation speed kaç?
- Hangi camera mode aktif?
- Hangi UI paneli açık?
- Sonraki phase'e geçiş şartı ne?

### 1.3 Visual Request Queue

Gameplay sistemleri sahneye direkt dokunmaz. Bunun yerine visual request üretir:

- Spawn staff NPC.
- Spawn operation object.
- Shift customer color.
- Move customer flow.
- Show review burst.
- Play staff stress animation.
- Focus camera.
- Pulse HUD.

`WorldPresenter` bu requestleri ilgili presenter'a dağıtır.

### 1.4 Optimization Rules

- Customer ve card UI prefabları pool ile yönetilir.
- Simulation resolve turn bazlı çalışır; her frame ağır ekonomi hesabı yapılmaz.
- Presenterlar state değişimi veya visual request gelince güncellenir.
- DOTween animasyonları kısa, interrupt-safe ve kill-safe olmalıdır.
- String ID kullanımı data bağlantısı için kabul edilir; sık çalışan runtime lookup'larda cache kullanılır.
- P0 prototipte Addressables gerekmez; inspector references veya Resources kabul edilir.

## 2. Milestone Overview

| Milestone | Goal | Exit Criteria |
|---|---|---|
| M0 Project Structure | Klasör, asmdef, namespace ve scene iskeleti | Unity compile temiz, boş yapı hazır |
| M1 Definitions | Enumlar ve ScriptableObject contract | Data assetleri oluşturulabilir |
| M2 Runtime State | Saf C# state modelleri | Run state memory'de kurulabilir |
| M3 Core Systems | Turn, card, sim, event, rival logic | Presenter olmadan EditMode testleri geçer |
| M4 Presenters | State'i world/UI'a çevirme | Kart, slot, customer, staff sahada görünür |
| M5 Scene Setup | Gameplay_Diorama hierarchy | Rootlar, camera, light, UI canvas hazır |
| M6 P0 Data | Fast Food vertical slice data | 3 kart, 1 event, 1 rival hamlesi data'da |
| M7 Playable Loop | İlk tur oynanabilir | Card offer -> event -> rival -> end turn çalışır |
| M8 Tests and Debug | Test, gizmo, validation | Regression yakalayacak test seti var |

## 3. M0 Project Structure

### T001 - Create Unity Folder Tree

Goal: Core oyun dosyaları için production klasör ağını kurmak.

Files/Folders:

```text
Assets/EmpireOfCards/
  Scripts/
    Definitions/
      Enums/
      ScriptableObjects/
      Serializable/
    Runtime/
      State/
      Requests/
    Systems/
    Presenters/
    UI/
    Utilities/
  Data/
    Ventures/
    Cards/
    Events/
    Levels/
    Assets/
    UI/
  Prefabs/
    Board/
    Business/
    Customers/
    Staff/
    Cards/
    UI/
    VFX/
  Scenes/
  Tests/
    EditMode/
    PlayMode/
```

Depends On: None.

Acceptance Criteria:

- Unity project içinde klasörler görünür.
- `.meta` dosyaları Unity tarafından oluşturulur.
- GDD dokümanları `Assets/steam-card-game-gdd/` altında kalır.

Notes:

- Eski doküman klasörü taşınmaz.
- Gameplay kodu `Assets/EmpireOfCards/` altında başlar.

### T002 - Add Assembly Definitions

Goal: Compile süresini ve test sınırlarını kontrol etmek.

Files/Folders:

- `Assets/EmpireOfCards/Scripts/EmpireOfCards.Runtime.asmdef`
- `Assets/EmpireOfCards/Tests/EditMode/EmpireOfCards.EditModeTests.asmdef`
- `Assets/EmpireOfCards/Tests/PlayMode/EmpireOfCards.PlayModeTests.asmdef`

Depends On: T001.

Acceptance Criteria:

- Runtime asmdef UnityEngine referanslarıyla compile olur.
- Test asmdefleri runtime assembly'e referans verir.
- PlayMode test assembly scene/presenter testleri için ayrılır.

Notes:

- DOTween referansı runtime assembly'e ancak gerçek presenter animasyonunda gerekiyorsa eklenir.
- Pure systems DOTween bağımlılığı taşımamalıdır.

### T003 - Establish Namespace Rules

Goal: Kodun modüler ve bulunabilir olması.

Files/Folders:

- `EmpireOfCards.Definitions`
- `EmpireOfCards.Runtime`
- `EmpireOfCards.Systems`
- `EmpireOfCards.Presenters`
- `EmpireOfCards.UI`
- `EmpireOfCards.Utilities`

Depends On: T002.

Acceptance Criteria:

- Her yeni script doğru namespace kullanır.
- Runtime state ve systems Unity-specific presenter namespace'ine bağlı değildir.

Notes:

- Namespace ile klasör yapısı birebir uyumlu tutulur.

## 4. M1 Definitions

### T004 - Implement Core Enums

Goal: Blueprint'teki enum sözleşmesini C#'a taşımak.

Files/Folders:

- `Scripts/Definitions/Enums/VentureType.cs`
- `CardBehaviorType.cs`
- `SlotType.cs`
- `CardPersistenceType.cs`
- `SlotBehaviorType.cs`
- `WorldManifestationType.cs`
- `TurnPhase.cs`
- `EventCategory.cs`
- `CustomerOwnership.cs`
- `CustomerMood.cs`
- `StaffStressLevel.cs`
- `BusinessStatType.cs`
- `RivalStrategyType.cs`
- `CameraMode.cs`
- `LightingFeedbackType.cs`
- `ScaleStage.cs`

Depends On: T003.

Acceptance Criteria:

- Enum adları `IMPLEMENTATION_BLUEPRINT.md` ile aynı olur.
- Project compile eder.
- Enumlar tek dosya veya mantıklı gruplar halinde olabilir; public API isimleri değişmez.

Notes:

- `CameraMode` UnityEngine.Camera ile çakışmasın diye namespace doğru kullanılmalı.

### T005 - Implement Serializable Helper Types

Goal: ScriptableObjectlerin tekrar eden field yapılarını ortak serializable tiplerle taşımak.

Files/Folders:

- `StatModifier.cs`
- `SlotDefinition.cs`
- `ReplacementRule.cs`
- `ExpirationRule.cs`
- `WorldManifestationDefinition.cs`
- `CardOfferCondition.cs`
- `EventTriggerCondition.cs`
- `CameraBeatDefinition.cs`
- `NPCBeatDefinition.cs`
- `EventChoiceDefinition.cs`
- `ConsequenceDefinition.cs`
- `AnchorDefinition.cs`
- `FlowPathDefinition.cs`
- `ScaleStageLayout.cs`
- `PrefabEntry.cs`
- `VfxEntry.cs`
- `AudioEntry.cs`
- `MaterialEntry.cs`
- `WeightedCardTag.cs`
- `WeightedRivalStrategy.cs`
- `UITransitionRule.cs`
- `SafeFrameRule.cs`
- `BusinessStateDefinition.cs`

Depends On: T004.

Acceptance Criteria:

- Tüm helper tipler `[System.Serializable]` olur.
- SO inspector'larında field olarak görünür.
- Gameplay hesapları için stat delta, tag, cost, duration ve prefab key bilgileri taşınabilir.

Notes:

- Fazla inheritance kullanılmaz; ilk sürümde düz serializable class/struct tercih edilir.

### T006 - Implement ScriptableObject Definitions

Goal: Data-driven gameplay için ana SO sınıflarını oluşturmak.

Files/Folders:

- `VentureDefinitionSO.cs`
- `CardDefinitionSO.cs`
- `EventDefinitionSO.cs`
- `LevelDefinitionSO.cs`
- `AssetManifestSO.cs`
- `UIFlowDefinitionSO.cs`
- `CameraProfileSO.cs`
- `LightingStateSO.cs`
- `CardPoolSO.cs`
- `RivalBehaviorProfileSO.cs`

Depends On: T005.

Acceptance Criteria:

- Her SO `[CreateAssetMenu]` ile editor'da oluşturulabilir.
- Field listesi `IMPLEMENTATION_BLUEPRINT.md` ile uyumludur.
- SO data runtime state'ten ayrıdır.

Notes:

- Runtime sırasında SO üzerinde değer değiştirilmez.
- Runtime kopyası gerekiyorsa state modellerine aktarılır.

### T007 - Add Data Validation Hooks

Goal: Eksik data'yı prototip sırasında hızlı yakalamak.

Files/Folders:

- `DefinitionValidationUtility.cs`
- SO sınıflarında `OnValidate()` veya editor-safe validation helper.

Depends On: T006.

Acceptance Criteria:

- Eksik `id`, negatif cash cost, boş event choice, boş prefab key gibi basit hatalar warning üretir.
- Validation runtime build'i bozacak editor-only bağımlılık taşımaz.

Notes:

- Editor scriptleri ayrı asmdef gerektirirse sonraki milestone'a bırakılabilir.

## 5. M2 Runtime State

### T008 - Implement RunState

Goal: Aktif run'ın üst state modelini kurmak.

Files/Folders:

- `Scripts/Runtime/State/RunState.cs`

Depends On: T004.

Acceptance Criteria:

- Selected venture, turn index, current phase, player/rival business, district, deck, active event taşınır.
- MonoBehaviour değildir.
- Testte new ile oluşturulabilir.

### T009 - Implement BusinessState And BoardSlotState

Goal: Oyuncu ve rakip işletmesinin hesaplanabilir state'ini kurmak.

Files/Folders:

- `BusinessState.cs`
- `BoardSlotState.cs`
- `TempEffectState.cs`

Depends On: T008.

Acceptance Criteria:

- Cash, demand, capacity, quality, rating, staff stability, legal risk, market share tutulur.
- Slotlarda occupied card ID ve linked world entity ID tutulur.
- Temp effect duration turn bazlı azalabilir.

### T010 - Implement StaffState And DistrictState

Goal: Çalışan krizi ve müşteri akışını state seviyesinde takip etmek.

Files/Folders:

- `StaffState.cs`
- `DistrictState.cs`
- `CustomerFlowState.cs`

Depends On: T009.

Acceptance Criteria:

- Staff stress, morale, skill, wage pressure ve stress level hesaplanabilir.
- District neutral/player/rival müşteri sayısını ve pull değerlerini taşır.
- Customer flow from/to anchor ve ownership taşır.

### T011 - Implement DeckState And ActiveEventState

Goal: Kart teklifleri ve aktif event sequence state'ini takip etmek.

Files/Folders:

- `DeckState.cs`
- `ActiveEventState.cs`

Depends On: T010.

Acceptance Criteria:

- Offered cards listesi tutulur.
- Draw/discard/exhausted listeleri veya ID koleksiyonları tutulur.
- Active event ID, source anchor, selected choice, remaining sequence time taşınır.

## 6. M3 Core Systems

### T012 - Implement TurnController State Machine

Goal: Turn phase geçişlerinin tek merkezden yönetilmesi.

Files/Folders:

- `Scripts/Systems/TurnController.cs`

Depends On: T008-T011.

Acceptance Criteria:

- Current phase `TurnPhase` olarak tutulur.
- Phase transition metotları kontrollüdür.
- Invalid transition warning üretir.
- İlk flow şu sırayı destekler: Planning -> CardOffer -> CardInspect -> SlotCommit -> Resolve -> Event/RivalReaction -> EndTurn.

Notes:

- TurnController presenter çağırmaz; phase değişim event'i yayınlar.

### T013 - Implement CardOfferSystem

Goal: Board state'e göre 3 kartlık teklif üretmek.

Files/Folders:

- `CardOfferSystem.cs`

Depends On: T006, T011.

Acceptance Criteria:

- CardPoolSO içinden minimum 3 kart döner.
- En az bir kart mevcut pressure'a cevap olacak şekilde bias desteklenir.
- Yetersiz kart varsa fallback kartlar döner.

Notes:

- İlk prototipte random seed basit olabilir; test için deterministic seed eklenmeli.

### T014 - Implement CardLifecycleSystem

Goal: Kart inspect, slot validation, replacement ve commit akışını yönetmek.

Files/Folders:

- `CardLifecycleSystem.cs`
- `CardPlayResult.cs`
- `SlotValidationResult.cs`

Depends On: T009, T013.

Acceptance Criteria:

- Empty slotta persistent kart commit olur.
- Full slotta replacement required sonucu döner.
- Instant/Burst kart slot doldurmadan resolve olabilir.
- Risk tag ve event hook bilgisi result içinde taşınır.
- World manifestation request üretmek için gerekli data döner.

### T015 - Implement CardResolutionSystem

Goal: Kartın stat, cash, tag ve temp effect etkilerini state'e uygulamak.

Files/Folders:

- `CardResolutionSystem.cs`

Depends On: T014.

Acceptance Criteria:

- Cash cost düşer.
- Stat modifiers uygulanır.
- Risk tags eklenir.
- Timed kartlar temp effect olarak kaydedilir.
- SO data mutate edilmez.

### T016 - Implement SimulationResolveSystem

Goal: Demand, capacity, staff stress, quality, rating, cash ve market share zincirini çözmek.

Files/Folders:

- `SimulationResolveSystem.cs`
- `SimulationResult.cs`

Depends On: T009, T010, T015.

Acceptance Criteria:

- Demand > capacity ise queue pressure ve staff stress artar.
- Quality düşükse rating delta negatif olur.
- Rating düşüşü organik demand ve market share'e etki eder.
- Cash revenue/cost hesaplanır.
- Event trigger context üretilir.

Notes:

- Formüller basit ve test edilebilir başlamalı; balance sonra yapılır.

### T017 - Implement CustomerMigrationSystem

Goal: Market share'i fiziksel müşteri akışına çevirmek.

Files/Folders:

- `CustomerMigrationSystem.cs`
- `CustomerMigrationResult.cs`

Depends On: T016.

Acceptance Criteria:

- Neutral -> player/rival geçişi hesaplanır.
- Player -> rival veya player -> neutral kaybı hesaplanır.
- Migration reason tag üretir: `BetterRating`, `LongQueue`, `RivalDiscount`, `BadService`.

### T018 - Implement EventDirector

Goal: Trigger context'ten event seçmek ve consequence uygulama akışını başlatmak.

Files/Folders:

- `EventDirector.cs`
- `EventTriggerContext.cs`
- `EventSelectionResult.cs`

Depends On: T006, T016.

Acceptance Criteria:

- Trigger conditions event pool ile eşleşir.
- Priority ve cooldown uygulanır.
- Staff crisis event seçilebilir.
- Event choice consequence sonucu state'e uygulanmak üzere döner.

### T019 - Implement ConsequenceResolutionSystem

Goal: Event choice sonuçlarını state'e yazmak.

Files/Folders:

- `ConsequenceResolutionSystem.cs`

Depends On: T018.

Acceptance Criteria:

- Stat delta, cash delta, customer migration, temp effect, risk tag değişimleri uygulanır.
- New pressure tags state'e eklenir.
- Visual consequence request üretmek için result döner.

### T020 - Implement RivalAISystem

Goal: Rakip için basit ama görünür hamle seçmek.

Files/Folders:

- `RivalAISystem.cs`
- `RivalActionResult.cs`

Depends On: T016, T017.

Acceptance Criteria:

- Rival behavior profile ağırlıklarına göre strateji seçer.
- İlk prototipte `AggressiveMarketing` ve `DefensiveStabilization` desteklenir.
- Rival action red customer pull veya rival signage visual request üretir.

## 7. M4 Presenters

### T021 - Implement Visual Request Types

Goal: Systems ve presenters arasındaki veri kontratını oluşturmak.

Files/Folders:

- `Scripts/Runtime/Requests/VisualRequest.cs`
- `WorldManifestationRequest.cs`
- `CameraRequest.cs`
- `LightingRequest.cs`
- `HudPulseRequest.cs`

Depends On: T004.

Acceptance Criteria:

- Request type, source ID, target anchor, prefab key, color, duration, camera hint taşınır.
- Requestler plain C# data olur.

### T022 - Implement WorldPresenter

Goal: Visual requestleri ilgili presenter'a dağıtmak.

Files/Folders:

- `Presenters/WorldPresenter.cs`

Depends On: T021.

Acceptance Criteria:

- Actor/Object/Flow/Aura/Marker/UIOverlay requestleri route edilir.
- Unknown prefab key warning üretir.
- Gameplay system referansı minimum tutulur.

### T023 - Implement CardPresenter And Card UI Pool

Goal: Card offer, hover, drag, snap ve invalid feedback göstermek.

Files/Folders:

- `Presenters/CardPresenter.cs`
- `UI/CardView.cs`
- `Utilities/ObjectPool.cs`

Depends On: T013, T021.

Acceptance Criteria:

- 3 kartlık offer bandı gösterilir.
- Hover card büyür ve target slot highlight request'i üretir.
- Drag/snap animasyonu çalışır.
- Card UI reuse/pool kullanır.

### T024 - Implement BusinessSlotPresenter

Goal: Slot board ve kart-slot-world linkini göstermek.

Files/Folders:

- `Presenters/BusinessSlotPresenter.cs`
- `UI/SlotView.cs`

Depends On: T014, T021.

Acceptance Criteria:

- Operation/Staff/Marketing/Supplier/TempEffect slotları görünür.
- Filled slot kartı gösterir.
- Hover linked world entity highlight request üretir.
- Full slot replacement panelini tetikler.

### T025 - Implement CustomerFlowPresenter

Goal: Gray/blue/red müşteri akışını sahada temsil etmek.

Files/Folders:

- `Presenters/CustomerFlowPresenter.cs`
- `Presenters/CustomerActorView.cs`

Depends On: T017, T021.

Acceptance Criteria:

- Tintable customer prefab pool kullanır.
- Neutral/player/rival ownership renkleri gösterilir.
- Flow path boyunca hareket eder.
- Migration result sonrası renk ve yön değişimi oynar.

### T026 - Implement StaffPresenter

Goal: Staff kartının sahada NPC'ye dönüşmesi ve stress/quit animasyonlarını oynatması.

Files/Folders:

- `Presenters/StaffPresenter.cs`
- `Presenters/StaffActorView.cs`

Depends On: T010, T021.

Acceptance Criteria:

- Garson kartı commit sonrası waiter NPC spawn olur.
- Stress level medium/high/breaking görsel state'e yansır.
- Quit event sonrası NPC walkout animasyonu oynar veya placeholder hareketi yapar.

### T027 - Implement HUDPresenter

Goal: Sol stat plakalarını runtime state ile güncellemek.

Files/Folders:

- `Presenters/HUDPresenter.cs`
- `UI/StatPlaqueView.cs`

Depends On: T016.

Acceptance Criteria:

- Cash, rating, demand, staff stability, legal risk, market share gösterilir.
- Stat delta pulse yapar.
- Critical threshold warning verir.

### T028 - Implement EventCinematicPresenter

Goal: Event focus, NPC beat, choice UI ve consequence feedback'i göstermek.

Files/Folders:

- `Presenters/EventCinematicPresenter.cs`
- `UI/EventChoicePanel.cs`
- `UI/EventConsequencePanel.cs`

Depends On: T018, T019, T021.

Acceptance Criteria:

- Event problem title ve choices sağ panelde görünür.
- Choice seçimi ConsequenceResolutionSystem'e bağlanır.
- Seçim sonrası visible consequence requestleri oynar.

### T029 - Implement CameraDirector And LightingDirector

Goal: Sabit izometrik camera mode ve Toy Studio feedback lights.

Files/Folders:

- `Presenters/CameraDirector.cs`
- `Presenters/LightingDirector.cs`

Depends On: T012, T021.

Acceptance Criteria:

- CameraMode değişimi desteklenir.
- BoardOverview, CardInspect, SlotPlacement, EventFocus placeholder camera anchorları çalışır.
- Lighting feedback local accent olarak oynar, global sahneyi karartmaz.

## 8. M5 Scene Setup

### T030 - Create Gameplay_Diorama Scene

Goal: Core sahne root hierarchy'sini kurmak.

Files/Folders:

- `Assets/EmpireOfCards/Scenes/Gameplay_Diorama.unity`

Depends On: T001.

Acceptance Criteria:

Scene hierarchy:

```text
Gameplay_Diorama
  Systems
  Cameras
  Lighting
  BoardRoot
  DistrictRoot
  PlayerBusinessRoot
  RivalBusinessRoot
  BusinessControlBoardRoot
  CustomerFlowRoot
  EventAnchorsRoot
  UIRoot
  DebugRoot
```

Notes:

- Root isimleri `IMPLEMENTATION_BLUEPRINT.md` ile aynı kalır.

### T031 - Setup Cameras

Goal: Kamera anchor ve virtual camera iskeletini kurmak.

Files/Folders:

- `Cameras/CM_BoardOverview`
- `CM_PlayerBusinessFocus`
- `CM_CardInspect`
- `CM_SlotPlacement`
- `CM_EventFocus`
- `CM_MarketFlow`
- `CM_RivalReaction`

Depends On: T030.

Acceptance Criteria:

- Orthographic isometric camera default görünümü verir.
- Serbest orbit yoktur.
- CameraDirector bu camera mode'ları seçebilir.

### T032 - Setup Lighting

Goal: Toy Studio base light rig kurmak.

Files/Folders:

- `Lighting/ToyStudio_BaseRig`
- `PlayerAccentLight`
- `RivalAccentLight`
- `LocalFeedbackLightPool`

Depends On: T030.

Acceptance Criteria:

- Gray/blue/red customer renkleri net ayrılır.
- Kriz feedback'i local light/VFX ile yapılır.

### T033 - Setup Board And District Placeholders

Goal: Görsel referans bible'a uygun placeholder board.

Files/Folders:

- `PF_Board_TabletopFrame`
- `PF_District_RoadIntersection`
- `PF_PlayerBusiness_FastFood_Startup`
- `PF_RivalBusiness_FastFood_Startup`

Depends On: T030.

Acceptance Criteria:

- Ahşap tabletop frame placeholder görünür.
- District ortada, player altta, rival üstte konumlanır.
- Flow path anchorları sahnede bulunur.

### T034 - Setup UI Root

Goal: Sol HUD, alt card hand, sağ panel canvas iskeletini kurmak.

Files/Folders:

- `Canvas_HUD_LeftStats`
- `Canvas_CardHand_Bottom`
- `Canvas_RightPanel`
- `Canvas_EventChoice`
- `Canvas_Debug`

Depends On: T030.

Acceptance Criteria:

- UI sahayı tamamen kapatmaz.
- Card hand alt bantta görünür.
- Right panel event/simulation modlarına hazırdır.

## 9. M6 P0 Data

### T035 - Create Fast Food Venture Data

Goal: İlk vertical slice için venture data.

Files/Folders:

- `Data/Ventures/FastFood.asset`
- `Data/Levels/FastFood_Level.asset`
- `Data/Assets/FastFood_AssetManifest.asset`

Depends On: T006, T030.

Acceptance Criteria:

- VentureDefinitionSO Fast Food için oluşturulur.
- Starting business state ve slot definitions doldurulur.
- LevelDefinitionSO scene anchorlarıyla eşleşir.

### T036 - Create P0 Fast Food Cards

Goal: İlk kart offer loop'u için minimum kart seti.

Files/Folders:

- `Card_NewWaiter.asset`
- `Card_NewGrill.asset`
- `Card_BrochureCampaign.asset`
- `Card_CheapSupplier.asset`
- `Card_ApologyCampaign.asset`
- `Card_LowWagePolicy.asset`

Depends On: T006, T035.

Acceptance Criteria:

- En az 3 offer kartı üretilebilir.
- Staff kartı persistent slot doldurur.
- Marketing kartı customer flow etkisi üretir.
- Risk/policy kartı future event tag bırakır.

### T037 - Create Staff Quit Event Data

Goal: İlk mikro-sinematik event.

Files/Folders:

- `Event_StaffQuit_FastFood.asset`

Depends On: T006, T036.

Acceptance Criteria:

- Trigger: staff stress high veya staff stability low.
- Choices: bonus ver, kabul et, krizi bastır/kov.
- Consequences: cash delta, staff count, stress, rating pressure, customer queue etkisi.

### T038 - Create Rival Action Data

Goal: İlk görünür rakip hamlesi.

Files/Folders:

- `RivalProfile_FastFood.asset`
- `Card_RivalDiscount.asset`

Depends On: T035.

Acceptance Criteria:

- Rival aggressive marketing/discount hamlesi yapabilir.
- District red pull visual request üretir.

## 10. M7 Playable Loop

### T039 - Wire GameBootstrapper

Goal: Scene açılınca Fast Food run state kurmak.

Files/Folders:

- `Systems/GameBootstrapper.cs`

Depends On: T035-T038.

Acceptance Criteria:

- VentureDefinitionSO inspector'dan atanır.
- RunState oluşturulur.
- TurnController initial phase'e alınır.

### T040 - Wire Card Offer To UI

Goal: İlk turda 3 kart göstermek.

Depends On: T013, T023, T039.

Acceptance Criteria:

- CardOffer phase'de bottom card hand açılır.
- 3 kart gösterilir.
- Hover/inspect target preview tetikler.

### T041 - Wire Slot Commit To World Manifestation

Goal: Kart oynanınca slot ve sahada sonuç görünsün.

Depends On: T014, T015, T022, T024, T026.

Acceptance Criteria:

- Garson kartı Staff slotuna oturur.
- Waiter NPC player business'ta belirir.
- Slot card hover linked waiter'ı highlight eder.

### T042 - Wire Simulation Resolve

Goal: Kart commit sonrası 6-12 sn resolve penceresi.

Depends On: T016, T017, T025, T027.

Acceptance Criteria:

- Demand/capacity hesaplanır.
- Queue ve customer flow güncellenir.
- HUD statları pulse eder.

### T043 - Wire Staff Quit Event

Goal: Staff stress event'i oynasın.

Depends On: T018, T019, T028, T029.

Acceptance Criteria:

- Staff stability düşükse event paneli açılır.
- EventFocus kamera aktif olur.
- Choice sonucu staff/cash/rating/customer pressure'a uygulanır.

### T044 - Wire Rival Reaction

Goal: Rakip hamlesi fiziksel okunur olsun.

Depends On: T020, T025, T029.

Acceptance Criteria:

- RivalReaction phase'de rakip kısa hamle yapar.
- Red customer flow güçlenir.
- BoardOverview'a geri dönülür.

### T045 - First Playable Acceptance Pass

Goal: İlk tam loop'u oynanabilir hale getirmek.

Depends On: T039-T044.

Acceptance Criteria:

```text
Start turn
-> 3 card offer
-> inspect card
-> commit staff card
-> waiter appears
-> resolve demand/capacity
-> event may trigger
-> player choice applies consequence
-> rival red pull
-> end turn summary
```

## 11. M8 Tests And Debug

### T046 - EditMode Tests For Card Systems

Goal: Card logic regression yakalamak.

Depends On: T013-T015.

Acceptance Criteria:

- Empty slot commit test.
- Full slot replacement required test.
- Persistent card occupies slot test.
- Burst card does not occupy slot test.

### T047 - EditMode Tests For Simulation

Goal: Core economy zinciri test edilsin.

Depends On: T016-T017.

Acceptance Criteria:

- Demand > capacity stress artırır.
- Low quality rating düşürür.
- Bad service customer migration'ı rival lehine bias eder.

### T048 - EditMode Tests For Event Selection

Goal: Event trigger doğru çalışsın.

Depends On: T018-T019.

Acceptance Criteria:

- Staff stability low staff crisis seçer.
- Cooldown event tekrarını engeller.
- Choice consequence state'e uygulanır.

### T049 - PlayMode Tests For World Manifestation

Goal: Sistemden gelen visual request sahada görünür.

Depends On: T022-T026.

Acceptance Criteria:

- Staff card sonrası NPC spawn olur.
- Customer migration sonrası tint değişimi görülür.
- Slot hover linked world entity highlight eder.

### T050 - Debug Tools And Gizmos

Goal: Level setup ve flow hatalarını hızlı görmek.

Files/Folders:

- `Debug/AnchorGizmo.cs`
- `Debug/FlowPathGizmo.cs`
- `Debug/RuntimeStateDebugPanel.cs`

Depends On: T030-T034.

Acceptance Criteria:

- Anchorlar editor'da görünür.
- Flow path yönleri renkli çizilir.
- Runtime state debug panelde temel statlar okunur.

## 12. Scene Checklist

Gameplay scene tamam sayılması için:

- [ ] `Systems` altında bootstrapper ve core systems var.
- [ ] `Cameras` altında BoardOverview, CardInspect, SlotPlacement, EventFocus, RivalReaction var.
- [ ] `Lighting` altında ToyStudio base ve local feedback pool var.
- [ ] `BoardRoot` altında tabletop frame ve board surface var.
- [ ] `DistrictRoot` altında road intersection, spawn anchors, flow paths var.
- [ ] `PlayerBusinessRoot` altında Fast Food startup placeholder var.
- [ ] `RivalBusinessRoot` altında rival placeholder var.
- [ ] `BusinessControlBoardRoot` altında slot board ve slot anchors var.
- [ ] `CustomerFlowRoot` altında active customer pool root var.
- [ ] `EventAnchorsRoot` altında staff/customer/legal/rival anchors var.
- [ ] `UIRoot` altında left HUD, bottom card hand, right panel, event panel var.
- [ ] `DebugRoot` altında gizmo/debug helpers var.

## 13. Definition Asset Checklist

P0 data tamam sayılması için:

- [ ] `VentureDefinitionSO/FastFood`
- [ ] `LevelDefinitionSO/FastFood_Level`
- [ ] `AssetManifestSO/FastFood_AssetManifest`
- [ ] `CardPoolSO/FastFood_StarterDeck`
- [ ] `RivalBehaviorProfileSO/FastFood_Rival`
- [ ] `CardDefinitionSO/NewWaiter`
- [ ] `CardDefinitionSO/NewGrill`
- [ ] `CardDefinitionSO/BrochureCampaign`
- [ ] `CardDefinitionSO/CheapSupplier`
- [ ] `CardDefinitionSO/ApologyCampaign`
- [ ] `CardDefinitionSO/LowWagePolicy`
- [ ] `EventDefinitionSO/StaffQuit_FastFood`
- [ ] `UIFlowDefinitionSO/Gameplay_Default`
- [ ] `CameraProfileSO/BoardOverview`
- [ ] `CameraProfileSO/EventFocus`
- [ ] `LightingStateSO/ToyStudio_Default`

## 14. Done Definition

Core mekanik foundation tamam sayılması için:

- Unity compile hatasız.
- Fast Food vertical slice scene açılır.
- İlk turda 3 kart çıkar.
- En az bir kart slotta kalıcı görünür.
- En az bir kart sahada NPC veya object oluşturur.
- Demand/capacity/rating/staff stress hesaplanır.
- Staff quit event tetiklenebilir ve seçim sonucu state değiştirir.
- District'te gray/blue/red müşteri akışı görünür.
- Rakip en az bir red pull hamlesi yapar.
- EditMode testleri card, simulation ve event sistemleri için geçer.
- PlayMode testleri basic world manifestation için geçer.
