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
