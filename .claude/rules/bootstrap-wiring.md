---
paths: ["Assets/_EmpireOfCards/Scripts/Bootstrap/**/*.cs"]
---

# Bootstrap & Wiring Rules

- Bootstrap classes create and wire ALL managers. No lazy initialization elsewhere.
- WiringService.WireAll() is the SINGLE entry point for all cross-references.
- New manager? Add to: ManagerBundle, WiringService.WireManagerReferences(), GameManager.Init()
- CardDataFactory and VentureDataFactory create ScriptableObjects at startup - data is immutable after.
- HUDBuilder creates UI hierarchy. UI references go through HUDBundle.
- Order matters: Data factories → Manager creation → WiringService → GameManager.StartNewRun()
