# 💥 BreakerAndOpenDoor

> CounterStrikeSharp plugin for CS2 Retake servers - Automatic map cleanup

[![CS2](https://img.shields.io/badge/Counter--Strike%202-000000?logo=counter-strike&logoColor=white)](https://store.steampowered.com/app/730/CounterStrike_2/)
[![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Release](https://img.shields.io/github/v/release/NeuTroNBZh/breakerandopendoor)](https://github.com/NeuTroNBZh/breakerandopendoor/releases)

---

## 📋 Overview

**BreakerAndOpenDoor** is a [CounterStrikeSharp](https://docs.cssharp.dev/) plugin for Counter-Strike 2 Retake servers. It automates map cleanup at the start of each round by opening doors and breaking destructible elements (windows, vents, objects).

### 🎯 Why This Plugin?

Retake servers need predictable map state every round. This plugin enforces a clear policy:
- open doors (never break them)
- break windows, vents, and other breakables
- support random door opening with stable per-round decisions

## ✨ Features

| Feature | Description | Configurable |
|---------|-------------|--------------|
| 🚪 **Door Opening** | Automatically opens all doors | ✅ |
| 🪟 **Window Breaking** | Destroys all windows and glass | ✅ |
| 🕳️ **Vent Breaking** | Destroys air vents and ducts | ✅ |
| 📦 **Other Objects** | Breaks remaining destructible elements | ✅ |

### 🔧 Technical Features

- **Door safety rule**: door entities are always open-only targets
- **Multi-pass execution**: immediate + delayed + extra + late + freeze_end
- **Robust fallback**: multiple break inputs and vent/window fallback path
- **Unknown probing**: optional probe mode for map-specific dynamic entities
- **Diagnostics**: candidate dump command for fast map tuning

## 📦 Quick Installation

## 🚀 Release Policy

- Active stable line: `v1.0.1`
- If a tag fails in CI, it is replaced by a clean patch release and documented.

### Method 1: Direct Download (Recommended)

1. **Download** the latest release: [breakerandopendoor.zip](https://github.com/NeuTroNBZh/breakerandopendoor/releases/latest/download/breakerandopendoor.zip)
2. **Extract** the archive
3. **Copy** the `addons` folder to your server's `game/csgo/`
4. **Restart** the map or server

### Method 2: GitHub Actions (Automatic)

Click the button below to deploy automatically:

[![Deploy](https://img.shields.io/badge/🚀_Deploy-Actions-blue?logo=github-actions)](https://github.com/NeuTroNBZh/breakerandopendoor/actions/workflows/release.yml)

## ⚙️ Configuration

Configuration file:
```
addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json
```

### Modes

The plugin supports two practical modes:

1. **Minimal Production Mode** (recommended)
- small config, lower maintenance
- relies on safe built-in defaults
- best for most servers

Default shipped config uses this mode:

```json
{
  "ConfigVersion": 1,
  "EnableOpenDoors": true,
  "DoorOpenChancePercent": 70,
  "EnableBreakWindows": true,
  "EnableBreakVents": true,
  "EnableBreakOtherBreakables": true
}
```

2. **Debug / Map Tuning Mode**
- full control over timing, probing and classname lists
- useful when a custom map has stubborn vents/windows

Reference preset:
- docs/config-debug-map-tuning.json

### Core Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `EnableOpenDoors` | bool | `true` | Enables door opening at round start |
| `DoorOpenChancePercent` | int | `100` | Per-door open chance each round (0-100) |
| `EnableBreakWindows` | bool | `true` | Enables window/glass breaking |
| `EnableBreakVents` | bool | `true` | Enables vent/grate breaking |
| `EnableBreakOtherBreakables` | bool | `true` | Enables other breakable objects |

### Advanced Options (optional)

- Pass timing: `EnableSecondPassAfterDelay`, `SecondPassDelaySeconds`, `AdditionalPassCount`, `AdditionalPassIntervalSeconds`, `EnableLateRoundStartPass`, `LateRoundStartDelaySeconds`, `EnableFreezeEndPass`, `FreezeEndPassDelaySeconds`
- Fallback control: `EnableKillFallbackForVentWindow`, `EnableRemoveFallbackForVentWindow`
- Probe control: `ProbeUnknownEntitiesForBreakInput`, `UnknownProbeMaxPerPass`, `UnknownProbeClassNameTokens`
- Diagnostics: `VerboseLogging`, `DebugDumpMaxLines`
- Entity lists: `DoorClassNames`, `BreakableClassNames`, `ExcludedClassNames`

If your server already works, keep only Minimal Production Mode.

## 🗺️ Supported Maps

| Map | Doors | Windows | Vents | Status |
|-----|-------|---------|-------|--------|
| de_mirage | ✅ | ✅ | ✅ | ✅ Tested |
| de_inferno | ✅ | ✅ | ✅ | ✅ Tested |
| de_nuke | ✅ | ✅ | ✅ | ✅ Tested |
| de_vertigo | ✅ | ✅ | ✅ | ✅ Tested |
| de_anubis | ✅ | ✅ | ✅ | ✅ Tested |
| de_ancient | ✅ | ✅ | ✅ | ✅ Tested |

## 🛠️ Requirements

- [Counter-Strike 2 Dedicated Server](https://developer.valvesoftware.com/wiki/Counter-Strike_2/Dedicated_Servers)
- [Metamod:Source](https://www.sourcemm.net/downloads.php?branch=master)
- [CounterStrikeSharp](https://docs.cssharp.dev/) (runtime recommended)

## 🔍 Diagnostics

Server console command:
```
css_bod_dump_break_candidates
```

This command displays candidate entities (index + classname) to help identify stubborn vents/windows on specific maps.

## 🏗️ Building from Source

### Windows
```powershell
powershell -ExecutionPolicy Bypass -File scripts/build-release.ps1
```

### Linux
```bash
dotnet build src/BreakerAndOpenDoor/BreakerAndOpenDoor.csproj -c Release
```

Output:
- `artifacts/release/breakerandopendoor/`
- `artifacts/release/breakerandopendoor.zip`

## 📁 Project Structure

```
breakerandopendoor/
├── src/
│   └── BreakerAndOpenDoor/      # Plugin source code
│       ├── Core/               # Business logic
│       ├── Config/             # Configuration
│       ├── Adapters/           # Adapters
│       └── Host/               # Entry point
├── addons/
│   └── counterstrikesharp/
│       └── configs/plugins/breakerandopendoor/  # Default config
├── scripts/
│   ├── build-release.ps1       # Build script
│   └── publish-github.ps1      # Publish script
└── .github/workflows/
    ├── ci.yml                  # CI/CD
    └── release.yml             # Automatic release
```

## 🧭 Maintainer Handover

If someone needs to continue this plugin professionally, use:
- docs/MAINTAINER-HANDOVER.md

Release process summary:
1. Merge tested changes to `main`
2. Ensure CI passes
3. Create semantic tag (`vMAJOR.MINOR.PATCH`)
4. Push tag to trigger release workflow

```bash
git tag -a v1.0.2 -m "Release v1.0.2"
git push origin v1.0.2
```

## 🤝 Contributing

Contributions are welcome! Check out [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

1. Fork the project
2. Create a branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📜 Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history.

## 🔒 Security

To report a vulnerability, see [SECURITY.md](SECURITY.md).

## 📝 License

Distributed under MIT License. See [LICENSE](LICENSE) for more information.

---

**⭐ Star this repo if you use it on your server!**

## Changelog
- v1.0.0: Initial release

## Acknowledgments
Thanks to all contributors!

### Installation Tips
- Always backup your config before updating
- Test on a local server first

## Troubleshooting
### Plugin not loading
Check that CounterStrikeSharp is properly installed.

### Doors not opening
Verify the map is in the supported list.

## Performance
This plugin has minimal performance impact on your server.

## Credits
Made with ❤️ for the CS2 community

## Examples
### Example 1: Basic Setup
Use the default config for most servers.

### Example 2: Custom Map
Enable probe mode for unsupported maps.

## Roadmap
- [ ] Support for more maps
- [ ] Config GUI
- [ ] Auto-update feature

## Security Best Practices
- Keep your server updated
- Use strong RCON passwords
- Monitor server logs

## Testing
Before deploying to production:
1. Test on a local server
2. Verify all doors open correctly
3. Check for console errors

## Community
Join our Discord for support and feature requests!

## Related Projects
- [CS2Retake](https://github.com/NeuTroNBZh/CS2Retake-main) - Full retake plugin
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) - The framework

## Stats
![GitHub stars](https://img.shields.io/github/stars/NeuTroNBZh/breakerandopendoor)
![GitHub forks](https://img.shields.io/github/forks/NeuTroNBZh/breakerandopendoor)

## Version History
- v1.0.0 - Initial release
- v1.0.1 - Bug fixes and improvements

## Architecture
The plugin uses a modular architecture with separate handlers for doors, windows, vents, and other breakables.

## Configuration Guide
See our wiki for detailed configuration options.

## API Documentation
The plugin exposes console commands for server administrators.
\n## Update 11\nDocumentation update 11
