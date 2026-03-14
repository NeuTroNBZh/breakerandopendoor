# breakerandopendoor

CounterStrikeSharp plugin for CS2 retake servers.

At round start, the plugin can:
- open doors
- break windows
- break vents
- break other breakables

All behaviors are configurable with simple true/false options.

## Why This Plugin

Many retake servers need deterministic map cleanup at the start of each round. This plugin provides a robust, configurable pipeline with multiple timing passes to handle map/entity timing differences.

## Features

- Dedicated toggles for each category:
  - EnableOpenDoors
  - EnableBreakWindows
  - EnableBreakVents
  - EnableBreakOtherBreakables
- Multi-pass execution strategy (round_start, delayed, late, freeze_end)
- Strong break fallback for stubborn entities
- Detailed logging and diagnostics command
- Release-ready bundle generator for drag-and-drop deployment

## Requirements

- Counter-Strike 2 dedicated server
- Metamod:Source
- CounterStrikeSharp (with runtime recommended)

## Installation (Fast)

1. Download the latest release zip.
2. Extract it.
3. Copy the extracted addons folder into your server game/csgo folder.
4. Restart map/server.

Expected target paths:
- addons/counterstrikesharp/plugins/breakerandopendoor/
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json

## Configuration

Main config file:
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json

Core gameplay switches:
- EnableOpenDoors
- EnableBreakWindows
- EnableBreakVents
- EnableBreakOtherBreakables

Example:

```json
{
  "ConfigVersion": 1,
  "EnableOpenDoors": true,
  "EnableBreakWindows": true,
  "EnableBreakVents": true,
  "EnableBreakOtherBreakables": true
}
```

## Diagnostics

Console command:
- css_bod_dump_break_candidates

This logs candidate entities (index + classname) to help identify stubborn vents/windows on specific maps.

## Build Release Locally

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build-release.ps1
```

Outputs:
- artifacts/release/breakerandopendoor/
- artifacts/release/breakerandopendoor.zip

## Project Layout

- src/RetakePlugin: plugin source code
- addons/counterstrikesharp/configs/plugins/breakerandopendoor: default config
- scripts/build-release.ps1: build + package script
- docs/architecture-cs2-retake-plugin.md: architecture notes

## License

MIT License. See LICENSE.
