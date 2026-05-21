# Empire of Cards - Unity Card Game

## WHAT
Turn-based card game: build businesses, hire employees, fight a rival for market dominance.
- **Engine:** Unity 6 (C#) | **Platform:** PC (Steam) | **Solo dev**
- **Architecture:** Bootstrap → WiringService → GameManager (Singleton) → StateMachine + TurnManager
- **Event-driven:** Central `EventBus` (static events). Managers communicate via events, not direct calls.
- **Turn flow:** 5 phases per turn: Event → Draw → Play → Resolve → Rival
- **Win condition:** Capture 6/10 territories. Rival gets 7 = lose. Hard cap at turn 30.

## WHY (Design Decisions)
- **WiringService pattern:** All cross-references set in one place via typed Init() calls. No reflection, no Find(). This prevents race conditions on scene load.
- **EventBus (static):** Decouples systems. UI listens to events, never polls managers. ClearAll() on scene unload prevents stale subscriptions.
- **PlayerResources extracted object:** Money/Actions/Slots grouped to avoid GameManager bloat.
- **3D world (Card3D, Hand3D, Board3D, SlotZone3D):** Cards are 3D objects, not UI. Drag via raycasts (InputManager3D), not UI EventSystem.
- **Bootstrap pattern:** GameSceneBootstrap creates managers → CardDataFactory builds cards → WiringService connects everything → GameManager.StartNewRun().

## HOW (Working Rules)

### Commands
- No build system in terminal - this is a Unity project. Test via Unity Editor play mode.
- Use `git status`, `git diff`, `git add`, `git commit` for version control.

### Code Conventions
- Namespace: `EmpireOfCards.{Domain}` (Core, Gameplay, UI, Data, Bootstrap, World, VFX, Audio, Save)
- MonoBehaviour singletons use `Singleton<T>` base or manual Instance pattern
- All inter-manager communication through EventBus - never direct method calls between managers
- Init() pattern: managers expose `public void Init(...)` called by WiringService
- CardData is ScriptableObject - never modify at runtime, use runtime copies

### GDD is Source of Truth
- `Assets/steam-card-game-gdd/GDD.md` contains ALL game design specs
- Balance numbers, card stats, turn flow, combo rules - always check GDD before changing gameplay code
- When GDD and code conflict, ASK which is correct before changing either

### DO NOT
- Over-engineer: no extra abstractions, no unnecessary interfaces
- Create stub files with TODO/placeholder content
- Add docstrings or comments to code you didn't change
- Use `GameObject.Find()` or `FindObjectOfType()` - use WiringService references
- Modify EventBus event signatures without updating ALL subscribers
- Change balance numbers without checking GDD first
- Add new managers without registering them in WiringService + GameManager.Init()

### Boundaries
- **Free to do:** Read/edit existing scripts, check GDD, run git commands
- **Ask first:** New manager classes, new card types, balance changes, architecture changes
- **Never:** Delete .meta files, modify ProjectSettings, commit secrets
