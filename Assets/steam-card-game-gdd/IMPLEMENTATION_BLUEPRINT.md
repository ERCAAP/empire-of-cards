# Empire of Cards Implementation Blueprint

> Bu dosya C# üretimine başlamadan önce teknik sözleşmeyi kilitler. Bu adımda kod yazılmayacak; ama geliştirici hangi enum, ScriptableObject, runtime state, system, presenter, sahne ve prefab yapısını kuracağını buradan çıkarabilmelidir.

## 1. C# Enum Contract

```csharp
public enum VentureType { FastFood, Cafe, TechApp, ClothingStore, MarketBakkal }
public enum CardBehaviorType { Install, Burst, Policy, Risk, Reaction }
public enum SlotType { Operation, Staff, Marketing, Supplier, TempEffect }
public enum CardPersistenceType { Instant, Persistent, Timed, EventOnly }
public enum SlotBehaviorType { None, OccupySlot, UpgradeSlot, ReplaceSlot, MergeWithSlot, AddTempEffect }
public enum WorldManifestationType { None, Actor, Object, Flow, Aura, Marker, UIOverlay, AudioOnly }
public enum TurnPhase { Planning, CardOffer, CardInspect, SlotCommit, Resolve, Event, RivalReaction, EndTurn }
public enum EventCategory { StaffCrisis, CustomerCrisis, CapacityCollapse, ReputationCrisis, SupplierCrisis, LegalRisk, RivalMove, Opportunity, GrowthPain, MetaHolding }
public enum CustomerOwnership { Neutral, Player, Rival, Loyal }
public enum CustomerMood { Calm, Happy, Impatient, Angry, Leaving }
public enum StaffStressLevel { Low, Medium, High, Breaking, Quit }
public enum BusinessStatType { Cash, Demand, Capacity, Quality, Rating, StaffStability, LegalRisk, MarketShare }
public enum RivalStrategyType { AggressiveMarketing, PremiumQuality, CheapExpansion, DefensiveStabilization, RiskyShortcut, StaffPoaching }
public enum CameraMode { BoardOverview, PlayerBusinessFocus, CardInspect, SlotPlacement, EventFocus, MarketFlow, RivalReaction, ScaleView }
public enum LightingFeedbackType { None, PlayerPositive, RivalPressure, StaffStress, LegalRisk, RatingDamage, RatingRecovery, ScaleUpgrade }
public enum ScaleStage { Startup, LocalFavorite, GrowthBusiness, ChainPlatform, HoldingCandidate }
```

Kural: Enum adları production code için başlangıç sözleşmesidir. İsim değişikliği gerekiyorsa önce bu doküman güncellenmelidir.

## 2. ScriptableObject Contract

### VentureDefinitionSO

Purpose: Bir venture'ın bütün tasarım giriş noktası.

Fields:

- `string id`
- `VentureType ventureType`
- `string displayName`
- `ScaleStage startingScaleStage`
- `BusinessStateDefinition startingBusinessState`
- `SlotDefinition[] slotDefinitions`
- `CardPoolSO starterDeck`
- `CardPoolSO cardPool`
- `EventDefinitionSO[] eventPool`
- `LevelDefinitionSO levelDefinition`
- `AssetManifestSO assetManifest`
- `RivalBehaviorProfileSO rivalProfile`
- `UIFlowDefinitionSO uiFlow`

### CardDefinitionSO

Purpose: Oyuncu karar kartı.

Fields:

- `string id`
- `string displayName`
- `VentureType[] allowedVentures`
- `CardBehaviorType behaviorType`
- `SlotType targetSlotType`
- `CardPersistenceType persistenceType`
- `SlotBehaviorType slotBehavior`
- `int cashCost`
- `int durationTurns`
- `StatModifier[] statModifiers`
- `string[] riskTagsAdded`
- `string[] eventHooks`
- `ReplacementRule[] replacementRules`
- `ExpirationRule expirationRule`
- `WorldManifestationDefinition worldManifestation`
- `CardOfferCondition[] offerConditions`
- `Sprite cardArt`
- `string promptRef`

### EventDefinitionSO

Purpose: Mikro-sinematik event.

Fields:

- `string id`
- `EventCategory category`
- `VentureType[] allowedVentures`
- `EventTriggerCondition[] triggerConditions`
- `int priority`
- `int cooldownTurns`
- `string[] requiredTags`
- `string[] blockedTags`
- `CameraBeatDefinition cameraBeat`
- `NPCBeatDefinition npcBeat`
- `EventChoiceDefinition[] choices`
- `ConsequenceDefinition ambientFallback`
- `string problemTitle`
- `string problemBody`

### LevelDefinitionSO

Purpose: Board layout, anchor ve camera data.

Fields:

- `string id`
- `VentureType ventureType`
- `ScaleStageLayout[] scaleStageLayouts`
- `AnchorDefinition[] playerBusinessAnchors`
- `AnchorDefinition[] districtSpawnAnchors`
- `AnchorDefinition[] rivalBusinessAnchors`
- `AnchorDefinition[] eventAnchors`
- `AnchorDefinition slotBoardAnchor`
- `CameraProfileSO[] cameraProfiles`
- `FlowPathDefinition[] flowPaths`

### AssetManifestSO

Purpose: Asset key -> prefab/VFX/audio/UI referansları.

Fields:

- `PrefabEntry[] actorPrefabs`
- `PrefabEntry[] businessObjectPrefabs`
- `PrefabEntry[] boardPrefabs`
- `PrefabEntry[] cardPrefabs`
- `PrefabEntry[] uiPrefabs`
- `VfxEntry[] vfxPrefabs`
- `AudioEntry[] audioClips`
- `MaterialEntry[] materials`
- `PrefabEntry[] placeholderFallbacks`

### UIFlowDefinitionSO

Purpose: Turn phase ve UI panel davranışı.

Fields:

- `string id`
- `TurnPhase phase`
- `string panelPrefabKey`
- `bool blocksSimulation`
- `float simulationSpeed`
- `CameraMode cameraHint`
- `string[] allowedInputs`
- `UITransitionRule transitionRule`

### CameraProfileSO

Purpose: Cinemachine virtual camera profile.

Fields:

- `CameraMode cameraMode`
- `float orthographicSize`
- `Vector3 eulerAngles`
- `string targetAnchorId`
- `float blendInSeconds`
- `int priority`
- `SafeFrameRule safeFrameRule`

### LightingStateSO

Purpose: Toy Studio base light ve lokal feedback.

Fields:

- `LightingFeedbackType feedbackType`
- `Color color`
- `float intensity`
- `float duration`
- `AnimationCurve intensityCurve`
- `string targetAnchorHint`

### CardPoolSO

Purpose: Kart havuzu ve offer ağırlıkları.

Fields:

- `CardDefinitionSO[] cards`
- `WeightedCardTag[] weightedTags`
- `int minOfferCount`
- `int maxOfferCount`

### RivalBehaviorProfileSO

Purpose: Rakip karar ağırlıkları.

Fields:

- `RivalStrategyType defaultStrategy`
- `WeightedRivalStrategy[] strategyWeights`
- `CardDefinitionSO[] rivalCardPool`
- `EventDefinitionSO[] rivalEventPool`

## 3. Runtime State Contract

Runtime state saf C# class/struct olmalıdır; MonoBehaviour olmamalıdır.

```text
RunState
- selectedVenture
- turnIndex
- currentPhase
- playerBusiness
- rivalBusiness
- districtState
- deckState
- activeEvent
- metaProgress

BusinessState
- ownerId
- cash
- demand
- capacity
- quality
- rating
- staffStability
- legalRisk
- marketShare
- scaleStage
- boardSlots
- staffRoster
- activeTempEffects
- riskTags

DistrictState
- neutralCustomerCount
- playerCustomerCount
- rivalCustomerCount
- trafficPressure
- playerPull
- rivalPull
- localTrendTags
- customerFlows

BoardSlotState
- slotId
- slotType
- subSlotName
- occupiedCardId
- linkedWorldEntityId
- upgradeLevel
- stressOrDurability
- isLocked
- activeModifiers

StaffState
- staffId
- displayName
- role
- assignedSlotId
- stress
- morale
- skill
- wagePressure
- stressLevel
- riskTags

CustomerFlowState
- flowId
- fromAnchor
- toAnchor
- ownership
- mood
- count
- speed
- reasonTag

DeckState
- drawPile
- discardPile
- offeredCards
- exhaustedCards
- offerBiasTags

ActiveEventState
- eventId
- sourceActorId
- sourceAnchorId
- selectedChoiceId
- phase
- remainingSequenceTime

TempEffectState
- effectId
- sourceCardOrEventId
- remainingTurns
- statModifiers
- worldEntityId
```

## 4. System Contract

Systems pure gameplay logic'e yakın kalır.

- `TurnController`: phase sırası, turn index ve input gating.
- `CardOfferSystem`: 3-5 teklif kartı seçer, en az bir cevap kartı kuralını uygular.
- `CardLifecycleSystem`: offer, inspect, validate, replacement, commit, persist, expire.
- `CardResolutionSystem`: cost, stat modifier, tag, risk, temp effect uygular.
- `SimulationResolveSystem`: demand, capacity, stress, quality, rating, cash, market share çözer.
- `ConsequenceResolutionSystem`: event/card consequence'larını state'e yazar.
- `EventDirector`: trigger context, priority, cooldown, selected event ve choice flow yönetir.
- `RivalAISystem`: rakip strateji ve hamle seçer.
- `CustomerMigrationSystem`: neutral/player/rival müşteri geçişlerini hesaplar.

## 5. Presenter Contract

Presenters state'i ve visual request'i sahaya çevirir.

- `WorldPresenter`: visual request router.
- `CustomerFlowPresenter`: müşteri spawn, color shift, path movement.
- `BusinessSlotPresenter`: slot board, occupied card, linked highlight.
- `StaffPresenter`: staff spawn, stress animation, quit/walkout.
- `CardPresenter`: card hand, hover, drag, snap, invalid feedback.
- `EventCinematicPresenter`: camera beat, NPC beat, event inset, consequence beat.
- `HUDPresenter`: left stat plaques, pulses, warnings.
- `CameraDirector`: Cinemachine camera mode geçişleri.
- `LightingDirector`: Toy Studio rig ve lokal feedback ışıkları.

Presenter kuralı: Presenter gameplay sonucu hesaplamaz; sadece gelen state/request'i oynatır.

## 6. Scene Hierarchy Contract

Ana sahne: `Assets/EmpireOfCards/Scenes/Gameplay_Diorama.unity`

```text
Gameplay_Diorama
  Systems
    GameBootstrapper
    TurnController
    CardLifecycleSystem
    SimulationResolveSystem
    EventDirector
    RivalAISystem
  Cameras
    CM_BoardOverview
    CM_PlayerBusinessFocus
    CM_CardInspect
    CM_SlotPlacement
    CM_EventFocus
    CM_MarketFlow
    CM_RivalReaction
    CM_ScaleView
  Lighting
    ToyStudio_BaseRig
    PlayerAccentLight
    RivalAccentLight
    LocalFeedbackLightPool
  BoardRoot
    PF_Board_TabletopFrame
    BoardSurface
    OffBoardProps
  DistrictRoot
    PF_District_RoadIntersection
    SpawnAnchors
    FlowPaths
    TrendEventAnchors
  PlayerBusinessRoot
    PF_PlayerBusiness_FastFood_Startup
    PlayerStaffAnchors
    PlayerQueueAnchors
    PlayerObjectAnchors
  RivalBusinessRoot
    PF_RivalBusiness_FastFood_Startup
    RivalQueueAnchors
    RivalActionAnchors
  BusinessControlBoardRoot
    PF_SlotBoard
    SlotAnchors
    CardLinkAnchors
  CustomerFlowRoot
    ActiveCustomers
    FlowVFX
  EventAnchorsRoot
    StaffEventAnchors
    CustomerEventAnchors
    LegalEventAnchors
    RivalEventAnchors
  UIRoot
    Canvas_HUD_LeftStats
    Canvas_CardHand_Bottom
    Canvas_RightPanel
    Canvas_EventChoice
    Canvas_Debug
  DebugRoot
    CameraAnchorGizmos
    FlowPathGizmos
    SlotLinkGizmos
```

## 7. P0 Prefab Contract

P0 prefablar ilk Fast Food vertical slice için zorunludur:

- `PF_Board_TabletopFrame`
- `PF_District_RoadIntersection`
- `PF_PlayerBusiness_FastFood_Startup`
- `PF_RivalBusiness_FastFood_Startup`
- `PF_Customer_Tintable`
- `PF_Staff_FastFood_Waiter`
- `PF_Staff_FastFood_Cashier`
- `PF_BusinessObject_FastFood_Grill`
- `PF_BusinessObject_FastFood_Counter`
- `PF_SlotBoard`
- `PF_Card_Install`
- `PF_Card_Risk`
- `PF_HUD_LeftStats`
- `PF_UI_RightSimulationPanel`
- `PF_UI_EventChoicePanel`
- `PF_VFX_CustomerColorShift`
- `PF_VFX_StressAura`
- `PF_VFX_MarketingPull`
- `PF_VFX_ReviewBurst`

## 8. First Vertical Slice Acceptance

İlk kodlanabilir loop:

```text
Start Turn
-> Card Offer shows 3 cards
-> Player inspects Staff card
-> Staff slot highlights and waiter ghost appears
-> Player commits card
-> Waiter card occupies Staff slot
-> Waiter NPC spawns in PlayerBusinessRoot
-> Simulation resolves demand/capacity
-> If demand > capacity, queue grows and stress rises
-> Staff stress event can trigger
-> Event choice applies consequence
-> Customer migration changes gray/blue/red distribution
-> Rival performs one visible red pull action
```

Bu loop çalışmadan venture expansion veya meta progression üretimine geçilmez.
