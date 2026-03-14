# 💥 BreakerAndOpenDoor

> Plugin CounterStrikeSharp pour serveurs CS2 Retake - Nettoyage automatique des maps

[![CS2](https://img.shields.io/badge/Counter--Strike%202-000000?logo=counter-strike&logoColor=white)](https://store.steampowered.com/app/730/CounterStrike_2/)
[![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Release](https://img.shields.io/github/v/release/NeuTroNBZh/breakerandopendoor?include_prereleases)](https://github.com/NeuTroNBZh/breakerandopendoor/releases)

## 📋 Description

**BreakerAndOpenDoor** est un plugin [CounterStrikeSharp](https://docs.cssharp.dev/) pour les serveurs Counter-Strike 2 en mode Retake. Il automatise le nettoyage des maps au début de chaque round en ouvrant les portes et en cassant les éléments destructibles (vitres, conduits, objets).

### 🎯 Pourquoi ce plugin ?

Les serveurs retake ont besoin d'un environnement déterministe à chaque round. Ce plugin fournit une pipeline robuste et configurable avec plusieurs passes de timing pour gérer les différences de comportement des entités selon les maps.

## ✨ Fonctionnalités

| Fonction | Description | Configurable |
|----------|-------------|--------------|
| 🚪 **Ouverture des portes** | Ouvre automatiquement toutes les portes | ✅ |
| 🪟 **Casse les vitres** | Détruit toutes les fenêtres et vitres | ✅ |
| 🕳️ **Casse les conduits** | Détruit les vents et conduits d'aération | ✅ |
| 📦 **Autres objets** | Casse les éléments destructibles restants | ✅ |

### 🔧 Caractéristiques techniques

- **Multi-pass execution** : Stratégie en plusieurs phases (round_start, delayed, late, freeze_end)
- **Fallback robuste** : Gestion des entités récalcitrantes
- **Logging détaillé** : Commande de diagnostic pour le débogage
- **Configuration simple** : Options true/false intuitives
- **Performance optimisée** : Exécution rapide sans impact sur le serveur

## 📦 Installation Rapide

### Méthode 1 : Téléchargement direct (Recommandé)

1. **Télécharger** la dernière release : [breakerandopendoor.zip](https://github.com/NeuTroNBZh/breakerandopendoor/releases/latest/download/breakerandopendoor.zip)
2. **Extraire** l'archive
3. **Copier** le dossier `addons` dans `game/csgo/` de votre serveur
4. **Redémarrer** la map ou le serveur

### Méthode 2 : GitHub Actions (Automatique)

Cliquez sur le bouton ci-dessous pour déployer automatiquement :

[![Deploy](https://img.shields.io/badge/🚀_Deploy-Actions-blue?logo=github-actions)](https://github.com/NeuTroNBZh/breakerandopendoor/actions/workflows/release.yml)

## ⚙️ Configuration

Fichier de configuration :
```
addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json
```

### Options disponibles

```json
{
  "ConfigVersion": 1,
  "EnableOpenDoors": true,
  "EnableBreakWindows": true,
  "EnableBreakVents": true,
  "EnableBreakOtherBreakables": true
}
```

| Option | Type | Défaut | Description |
|--------|------|--------|-------------|
| `EnableOpenDoors` | bool | `true` | Active l'ouverture automatique des portes |
| `EnableBreakWindows` | bool | `true` | Active la destruction des vitres |
| `EnableBreakVents` | bool | `true` | Active la destruction des conduits/vents |
| `EnableBreakOtherBreakables` | bool | `true` | Active la destruction des autres objets cassables |

## 🗺️ Maps supportées

| Map | Portes | Vitres | Vents | Statut |
|-----|--------|--------|-------|--------|
| de_mirage | ✅ | ✅ | ✅ | ✅ Testé |
| de_inferno | ✅ | ✅ | ✅ | ✅ Testé |
| de_nuke | ✅ | ✅ | ✅ | ✅ Testé |
| de_vertigo | ✅ | ✅ | ✅ | ✅ Testé |
| de_anubis | ✅ | ✅ | ✅ | ✅ Testé |
| de_ancient | ✅ | ✅ | ✅ | ✅ Testé |

## 🛠️ Prérequis

- [Counter-Strike 2 Dedicated Server](https://developer.valvesoftware.com/wiki/Counter-Strike_2/Dedicated_Servers)
- [Metamod:Source](https://www.sourcemm.net/downloads.php?branch=master)
- [CounterStrikeSharp](https://docs.cssharp.dev/) (avec runtime recommandé)

## 🔍 Diagnostics

Commande console serveur :
```
css_bod_dump_break_candidates
```

Cette commande affiche les entités candidates (index + classname) pour aider à identifier les vents/vitres récalcitrants sur des maps spécifiques.

## 🏗️ Compilation

### Windows
```powershell
powershell -ExecutionPolicy Bypass -File scripts/build-release.ps1
```

### Linux
```bash
dotnet build src/RetakePlugin/RetakePluginHost.csproj -c Release
```

Sortie :
- `artifacts/release/breakerandopendoor/`
- `artifacts/release/breakerandopendoor.zip`

## 📁 Structure du projet

```
breakerandopendoor/
├── src/
│   └── RetakePlugin/           # Code source du plugin
│       ├── Core/               # Logique métier
│       ├── Config/             # Configuration
│       ├── Adapters/           # Adaptateurs
│       └── Host/               # Point d'entrée
├── addons/
│   └── counterstrikesharp/
│       └── configs/plugins/breakerandopendoor/  # Config par défaut
├── scripts/
│   ├── build-release.ps1       # Script de build
│   └── publish-github.ps1      # Script de publication
└── .github/workflows/
    ├── ci.yml                  # CI/CD
    └── release.yml             # Release automatique
```

## 🤝 Contribution

Les contributions sont les bienvenues ! Consultez [CONTRIBUTING.md](CONTRIBUTING.md) pour les guidelines.

1. Fork le projet
2. Créer une branche (`git checkout -b feature/AmazingFeature`)
3. Commit les changements (`git commit -m 'Add some AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

## 📜 Changelog

Voir [CHANGELOG.md](CHANGELOG.md) pour l'historique des versions.

## 🔒 Sécurité

Pour signaler une vulnérabilité, consultez [SECURITY.md](SECURITY.md).

## 📝 Licence

Distribué sous licence MIT. Voir [LICENSE](LICENSE) pour plus d'informations.

---

**⭐ Star ce repo si tu l'utilises sur ton serveur !**
