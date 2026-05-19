---
paths: ["Assets/steam-card-game-gdd/**/*"]
---

# GDD Rules

- GDD is the source of truth for ALL gameplay design
- When editing GDD: update version number and date at the top
- Balance tables in GDD must stay in sync with code (BalanceDefs.cs, Constants.cs)
- If a code change contradicts GDD, flag it - don't silently change GDD to match
- Turkish comments in GDD are fine - it's the designer's native language
