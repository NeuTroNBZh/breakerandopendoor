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

Retake servers need a deterministic environment every round. This plugin provides a robust and configurable pipeline with multiple timing passes to handle entity behavior differences across maps.

## ✨ Features

| Feature | Description | Configurable |
|---------|-------------|--------------|
| 🚪 **Door Opening** | Automatically opens all doors | ✅ |
| 🪟 **Window Breaking** | Destroys all windows and glass | ✅ |
| 🕳️ **Vent Breaking** | Destroys air vents and ducts | ✅ |
| 📦 **Other Objects** | Breaks remaining destructible elements | ✅ |

### 🔧 Technical Features

- **Multi-pass execution**: Multi-phase strategy (round_start, delayed, late, freeze_end)
- **Robust fallback**: Handles stubborn entities
- **Detailed logging**: Diagnostic command for debugging
- **Simple configuration**: Intuitive true/false options
- **Optimized performance**: Fast execution with no server impact

## 📦 Quick Installation

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

### Available Options

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

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `EnableOpenDoors` | bool | `true` | Enable automatic door opening |
| `DoorOpenChancePercent` | int | `100` | Percentage chance to open each door per round (0-100) |
| `EnableBreakWindows` | bool | `true` | Enable window/glass destruction |
| `EnableBreakVents` | bool | `true` | Enable vent/duct destruction |
| `EnableBreakOtherBreakables` | bool | `true` | Enable other breakable objects destruction |

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
dotnet build src/RetakePlugin/RetakePluginHost.csproj -c Release
```

Output:
- `artifacts/release/breakerandopendoor/`
- `artifacts/release/breakerandopendoor.zip`

## 📁 Project Structure

```
breakerandopendoor/
├── src/
│   └── RetakePlugin/           # Plugin source code
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
