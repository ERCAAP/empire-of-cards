# Empire of Cards - Business Simulation Card-Strategy Hybrid

## WHAT
Turn-based card game: build a business, hire staff, manage crises, fight a rival for market dominance.
- **Engine:** Unity 6 (C#) | **Platform:** PC (Steam) | **Solo dev**
- **GDD:** `Assets/steam-card-game-gdd/GDD_v5.md` (v5.0 - source of truth)
- **MVP:** Restaurant sector only. Other sectors unlock via meta-progression.
- **Architecture:** Bootstrap → WiringService → GameManager (Singleton) → TurnManager + EventBus
- **3 Layers:** Cards (decisions) + Board (business state) + Simulation (consequences)
- **Packages:** DOTween, Cinemachine 3.x, URP, Post Processing, Input System

## HOW (Working Rules)

### Commands
- No build system in terminal - Unity project. Test via Unity Editor play mode.

### Code Conventions
- Namespace: `EmpireOfCards.{Domain}` (Core, Gameplay, UI, Data, Bootstrap, World, VFX, Audio, Save)
- All inter-manager communication through EventBus - never direct calls
- Init() pattern: managers expose `public void Init(...)` called by WiringService
- Constants.cs for ALL magic numbers
- DOTween for ALL animations (DG.Tweening)
- Cinemachine 3.x (CinemachineCamera, NOT CinemachineVirtualCamera)

### GDD is Source of Truth
- `Assets/steam-card-game-gdd/GDD_v5.md` contains ALL game design specs
- When GDD and code conflict, ASK which is correct

### DO NOT
- Over-engineer: no unnecessary abstractions
- Use `GameObject.Find()` or `FindObjectOfType()`
- Change balance numbers without checking GDD
- Add managers without registering in WiringService
- Delete .meta files or modify ProjectSettings
