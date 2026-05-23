# Empire of Cards Code Architecture

> Amaç: GDD sistemlerini Unity'de sürdürülebilir, data-driven ve prototipten production'a taşınabilir şekilde kurmak.

## 1. Architecture Philosophy

Kod mimarisi dört katmandan oluşur:

1. `Definitions`: ScriptableObject tasarım verileri.
2. `Runtime State`: Run sırasında değişen saf C# state modelleri.
3. `Systems`: Gameplay hesaplayan, mümkün olduğunca sunumdan bağımsız servisler.
4. `Presenters`: State ve visual requestleri sahaya/UI'a çeviren Unity MonoBehaviour katmanı.

Kural: Gameplay sistemi doğrudan animasyon oynatmaz, presenter doğrudan ekonomi hesabı yapmaz.

## 2. Suggested Folder Structure

```text
Assets/EmpireOfCards/
  Scripts/
    Definitions/
    Runtime/
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
  Prefabs/
    Board/
    Business/
    Customers/
    Staff/
    Cards/
    UI/
    VFX/
  Scenes/
    Boot.unity
    MainMenu.unity
    Gameplay_Diorama.unity
    Prototype_FastFood.unity
```

## 3. Definition Models

### 3.1 VentureDefinition

Venture kimliği ve içerik giriş noktası.

Fields:

- id
- displayName
- ventureTheme
- visualTheme
- startingBusinessState
- slotDefinitions
- starterDeck
- cardPool
- eventPool
- levelModule
- rivalBehaviorWeights

### 3.2 CardDefinition

Kartın data-driven tanımı.

Fields:

- id
- displayName
- behaviorType: Install, Burst, Policy, Risk, Reaction
- ventureSurfaceType
- cost
- slotTarget
- persistenceType
- statEffects
- worldManifestation
- riskTagsAdded
- eventHooks
- replacementRules
- expirationRule

### 3.3 EventDefinition

Mikro-event tanımı.

Fields:

- id
- category
- triggerConditions
- priority
- cooldown
- requiredTags
- blockedTags
- cameraBeat
- npcBeat
- choices
- consequences

### 3.4 LevelDefinition

Venture sahne modülünü ve anchor noktalarını tanımlar.

Fields:

- id
- ventureId
- startupLayout
- midgameLayout
- lategameLayout
- playerBusinessAnchors
- districtSpawnAnchors
- rivalBusinessAnchors
- eventAnchors
- cameraProfiles

### 3.5 AssetManifestDefinition

Asset key'leri ile prefab/VFX/audio referanslarını bağlar.

Fields:

- actorPrefabKeys
- businessObjectKeys
- cardFrameKeys
- uiIconKeys
- vfxKeys
- audioKeys
- placeholderFallbacks

### 3.6 UIFlowDefinition

Ana UI flow ve panel davranışlarını tanımlar.

Fields:

- flowId
- screenType
- allowedInputs
- panelPrefabKey
- transitionRule
- blocksSimulation
- cameraHint

## 4. Runtime State Models

### 4.1 RunState

Aktif run'ın üst state'i.

Contains:

- selectedVentureId
- turnIndex
- playerBusiness
- rivalBusiness
- districtState
- deckState
- activeEvents
- metaProgress

### 4.2 BusinessState

Oyuncu veya rakip işletme state'i.

Contains:

- cash
- demand
- capacity
- quality
- rating
- staffStability
- legalRisk
- marketShare
- boardSlots
- activeTempEffects
- riskTags
- staffRoster
- scaleStage

### 4.3 BoardSlotState

Kalıcı kart-slot-world bağlantısı.

Contains:

- slotId
- slotType
- occupiedCardId
- linkedWorldEntityId
- upgradeLevel
- stressOrDurability
- activeModifiers

### 4.4 StaffState

Çalışanların event ve stress için takip edilen state'i.

Contains:

- staffId
- displayName
- role
- assignedSlotId
- stress
- morale
- skill
- wagePressure
- riskTags

### 4.5 CustomerFlowState

District müşteri akışının state'i.

Contains:

- neutralCount
- playerCount
- rivalCount
- playerPull
- rivalPull
- flowReason
- moodDistribution

## 5. Core Systems

### 5.1 Bootstrapping

`GameBootstrapper` gerekli definitions ve managers yükler. İlk prototipte doğrudan Resources veya inspector referansları kullanılabilir; production'da Addressables değerlendirilebilir.

### 5.2 TurnController

Tur sırasını yönetir.

```text
StartTurn
-> OfferCards
-> AwaitPlayerCard
-> CommitCard
-> ResolveSimulation
-> EvaluateEvents
-> ResolveRival
-> EndTurnSummary
```

### 5.3 CardLifecycleSystem

Kart teklif, inspect, slot validation, placement, replacement, persistence ve expiry işlerini yönetir.

### 5.4 SimulationResolveSystem

Demand, capacity, quality, rating, cash ve market share zincirini çözer.

### 5.5 EventDirector

Event trigger context'ini event pool ile eşleştirir, mikro-sinematik sequence'i başlatır, choice consequences uygular.

### 5.6 RivalAISystem

Rakip davranış ailesini seçer ve rival board/district üzerinde görsel hamle üretir.

### 5.7 WorldPresenter

Visual requestleri ilgili presenter'lara dağıtır.

Child presenters:

- CustomerFlowPresenter
- StaffPresenter
- BusinessObjectPresenter
- SlotBoardPresenter
- CardPresenter
- EventCinematicPresenter
- RivalPresenter
- UIPresenter

## 6. Implementation Order

1. Data definitions ve runtime state modelleri.
2. Fast Food data örnekleri.
3. TurnController ve kart offer flow.
4. CardLifecycleSystem + slot board.
5. Basit world manifestation: garson, masa, marketing flow.
6. SimulationResolveSystem: demand/capacity/rating/market share.
7. CustomerFlowPresenter: gray/blue/red akış.
8. EventDirector: staff quit event.
9. RivalAISystem: basit marketing/discount hamlesi.
10. UI polish ve venture genelleme.

## 7. Testing Strategy

EditMode tests:

- Card placement slot validation.
- Replacement rules.
- Simulation resolve math.
- Event trigger conditions.
- Rival behavior selection.

PlayMode tests:

- Kart oynanınca world manifestation oluşur.
- Staff slot kartı linked NPC ile bağlanır.
- Demand > capacity olunca queue/stress presenter tetiklenir.
- Event choice consequence state'e uygulanır.

## 8. Architecture Acceptance Criteria

- Yeni venture eklemek için core code değiştirmek gerekmemeli.
- Kart data'sı değiştirilince slot ve world manifestation davranışı editor data'dan akmalı.
- Presenter kapalıyken systems test edilebilmeli.
- UI ve world feedback aynı state değişiminden beslenmeli.
