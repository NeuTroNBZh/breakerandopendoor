# Journal Central - CS2 Retake Orchestration

## Objectif projet
Plugin CS2 retake qui, au debut de chaque round:
- casse les objets cassables
- n'endommage jamais les portes
- ouvre les portes au lieu de les casser

## Stack et decisions techniques
- Runtime principal: CounterStrikeSharp + .NET 8
- Architecture: scanner/classifier/executor/coordinator pour separer detection, decision et action
- Regle gameplay non negociable: toute entite de type porte doit etre ouverte, jamais cassee

## Log des actions

### 2026-03-14 17:34:10 +01:00
- Action: ajout d'un script de publication GitHub en une commande.
- Fichiers modifies:
  - scripts/publish-github.ps1
  - README.md
- Decision:
  - normaliser la publication avec push `main` + tag `v0.1.0` pour declencher la release GitHub Actions.
- Points ouverts:
  - besoin du depot cible exact au format `owner/repo` (ou au minimum `owner`) pour finaliser le push distant.

### 2026-03-14 17:47:00 +01:00
- Action: publication effective sur GitHub vers https://github.com/NeuTroNBZh/breakerandopendoor.
- Resultat:
  - tag `v0.1.0` publie sur le remote
  - branche `main` poussee et synchronisee apres fusion propre de l'initial commit distant
- Note:
  - workflow release GitHub Actions declenche par le tag `v0.1.0`.

### 2026-03-15 21:34:14 +01:00
- Action: ajout d'une randomisation de l'ouverture des portes pour eviter que toutes les portes soient ouvertes a chaque round.
- Fichiers modifies:
  - src/RetakePlugin/Config/PluginConfig.cs
  - src/RetakePlugin/Core/RoundStartCoordinator.cs
  - src/RetakePlugin/RetakePlugin.cs
  - src/RetakePlugin/Host/RetakePluginHost.cs
  - addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json
  - README.md
- Decision:
  - nouvelle option `DoorOpenChancePercent` (0-100), avec decision stable par porte sur un round complet (multi-pass inclus).
