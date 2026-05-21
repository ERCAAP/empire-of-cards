# Empire of Cards - Business Simulation Card-Strategy Hybrid

## WHAT
Turn-based card game: build a business, hire staff, manage crises, fight a rival for market dominance.
- **Engine:** Unity 6 (C#) | **Platform:** PC (Steam) | **Solo dev**
- **GDD:** `Assets/steam-card-game-gdd/GDD_v5.md` (v5.0 - source of truth)
- **MVP:** Restaurant sector only. Other sectors unlock via meta-progression.
- **Architecture:** Bootstrap → WiringService → GameManager (Singleton) → TurnManager + EventBus
- **3 Layers:** Cards (decisions) + Board (business state) + Simulation (consequences)

## WHY (Design Decisions)
- **WiringService pattern:** All cross-references set in one place via Init() calls. No Find().
- **EventBus (static):** Decouples systems. UI listens to events, never polls.
- **Board = Physical business:** Not a card game table. Mutfak, Salon, Depo - cards sit in physical spaces.
- **Customers are simulation, not cards:** Player controls the business, customers come based on stats.
- **Staff have drama:** They grow, get promoted, burn out, quit. Not just stat modifiers.

## HOW (Working Rules)

### Commands
- No build system in terminal - Unity project. Test via Unity Editor play mode.
- Use `git status`, `git diff`, `git add`, `git commit` for version control.

### Code Conventions
- Namespace: `EmpireOfCards.{Domain}` (Core, Gameplay, UI, Data, Bootstrap, World, VFX, Audio, Save)
- MonoBehaviour singletons use Instance pattern
- All inter-manager communication through EventBus
- Init() pattern: managers expose `public void Init(...)` called by WiringService
- CardData is ScriptableObject - never modify at runtime, use runtime copies
- Constants.cs for ALL magic numbers
- One responsibility per class. No God classes.

### GDD is Source of Truth
- `Assets/steam-card-game-gdd/GDD_v5.md` contains ALL game design specs
- When GDD and code conflict, ASK which is correct

### DO NOT
- Over-engineer: no unnecessary abstractions
- Create stub files with TODO/placeholder content
- Use `GameObject.Find()` or `FindObjectOfType()`
- Modify EventBus event signatures without updating ALL subscribers
- Change balance numbers without checking GDD first
- Add new managers without registering them in WiringService

### Boundaries
- **Free to do:** Read/edit existing scripts, check GDD, run git commands
- **Ask first:** New manager classes, new card types, balance changes, architecture changes
- **Never:** Delete .meta files, modify ProjectSettings, commit secrets
