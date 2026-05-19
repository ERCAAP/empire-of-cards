---
paths: ["Assets/_EmpireOfCards/Scripts/**/*.cs"]
---

# Unity C# Script Rules

- Namespace must be `EmpireOfCards.{Domain}` matching folder structure
- Inter-manager communication ONLY through EventBus static events
- New managers must have `public void Init(...)` and be registered in WiringService
- Never use `GameObject.Find()`, `FindObjectOfType()`, or `GetComponent()` in Update loops
- EventBus subscriptions: subscribe in OnEnable, unsubscribe in OnDisable
- CardData (ScriptableObject) is read-only at runtime
- 3D card system: Card3D for visuals, CardData for data. Never mix UI canvas with 3D cards
- Balance values must reference Constants.cs or GameBalanceData, not magic numbers
