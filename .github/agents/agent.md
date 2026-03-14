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
