# Central Journal - CS2 Retake Orchestration

## Project Objective
CS2 retake plugin that, at the start of each round:
- breaks breakable objects
- never damages doors
- opens doors instead of breaking them

## Stack and Technical Decisions
- Primary runtime: CounterStrikeSharp + .NET 8
- Architecture: scanner/classifier/executor/coordinator to separate detection, decision, and action
- Non-negotiable gameplay rule: every door entity must be opened, never broken

## Action Log

### 2026-03-14 17:34:10 +01:00
- Action: added a one-command GitHub publish script.
- Files changed:
  - scripts/publish-github.ps1
  - README.md
- Decision:
  - standardized publishing with `main` push + `v0.1.0` tag to trigger GitHub Actions release.
- Open points:
  - requires exact target repository in `owner/repo` format (or at least `owner`) to finalize remote push.

### 2026-03-14 17:47:00 +01:00
- Action: completed GitHub publication to https://github.com/NeuTroNBZh/breakerandopendoor.
- Result:
  - `v0.1.0` tag published to remote
  - `main` branch pushed and synchronized after clean merge of remote initial commit
- Note:
  - GitHub Actions release workflow triggered by `v0.1.0` tag.

### 2026-03-15 21:34:14 +01:00
- Action: added randomized door opening so not all doors open every round.
- Files changed:
  - src/RetakePlugin/Config/PluginConfig.cs
  - src/RetakePlugin/Core/RoundStartCoordinator.cs
  - src/RetakePlugin/RetakePlugin.cs
  - src/RetakePlugin/Host/RetakePluginHost.cs
  - addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json
  - README.md
- Decision:
  - new `DoorOpenChancePercent` option (0-100), with stable per-door decision for one full round (including multi-pass).

### 2026-03-15 22:00:56 +01:00
- Action: fixed release pipeline and published a clean replacement release.
- Files changed:
  - .github/workflows/release.yml
  - README.md
  - docs/MAINTAINER-HANDOVER.md
- Result:
  - identified root cause for missing `v1.0.1` release (packaging step path mismatch)
  - release workflow fixed to publish from `dotnet publish` output
  - new stable release `v1.0.2` published successfully with release asset
- Decision:
  - keep a professional release policy: failed release tags are superseded by the next patch release.

### 2026-03-15 22:16:49 +01:00
- Action: fixed vent-breaking reliability regression while preserving door safety rules.
- Files changed:
  - src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpEngineApi.cs
  - addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json
- Result:
  - tightened door hint detection to avoid accidental non-door classification
  - reinforced vent/window break fallback sequence (Break/Shatter + Kill/KillHierarchy + Remove)
  - expanded default breakable classnames with common window/vent/glass entities

### 2026-03-15 22:26:55 +01:00
- Action: removed obsolete legacy release artifacts and automated their cleanup.
- Files changed:
  - scripts/build-release.ps1
- Result:
  - deleted stale `artifacts/release/RetakePluginHost/` and `artifacts/release/RetakePluginHost.zip`
  - build script now always removes these legacy artifacts before generating a new release bundle
  - only `artifacts/release/breakerandopendoor/` and `artifacts/release/breakerandopendoor.zip` remain after build

### 2026-03-15 22:31:54 +01:00
- Action: applied a second reliability pass for vent/window breaking based on server logs.
- Files changed:
  - src/RetakePlugin/Core/RoundStartCoordinator.cs
  - src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpEngineApi.cs
  - addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json
- Result:
  - unknown probe now force-includes common dynamic props (`prop_dynamic*`, `prop_physics*`)
  - vent/window-like detection widened (`grate`, `breakable`, `prop_dynamic`) for stronger Kill/Remove fallback
  - default unknown probe tokens now include `prop_dynamic`
