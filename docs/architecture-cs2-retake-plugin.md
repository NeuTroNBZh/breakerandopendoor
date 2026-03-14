# Architecture - Plugin CS2 Retake (round_start)

## Objectifs
- Casser tous les objets cassables non-porte au debut de chaque round.
- Ouvrir toutes les portes au debut de chaque round.
- Ne jamais casser une porte (regle gameplay absolue).
- Fournir une architecture testable, extensible et robuste pour serveur.

## Sequence round_start
1. Reception de l'evenement round_start via l'entree plugin.
2. Debut d'une fenetre de traitement courte (budget temps configurable).
3. Scan des entites de la map via un scanner dedie.
4. Classification de chaque entite:
   - Porte
   - Cassable non-porte
   - Exclusion
   - Inconnue
5. Application du pipeline de decision:
   - Si porte: action Ouvrir.
   - Si cassable non-porte: action Casser.
   - Sinon: Ignorer.
6. Journalisation d'un resume (nb portes ouvertes, nb cassables casses, erreurs).

## Classification des entites
### Portes
Signaux de classification possibles:
- Classnames explicites de porte (exemple: prop_door_rotating, func_door, func_door_rotating).
- Presence d'inputs d'ouverture (Open, Unlock + Open).

Regle prioritaire:
- Toute entite reconnue comme porte est exclue de toute logique de casse.

### Cassables non-portes
Signaux de classification possibles:
- Entites historiquement cassables (exemple: func_breakable, func_breakable_surf, prop_physics* selon flags moteur).
- Presence d'inputs de destruction ou etat de breakability.

Regle:
- Une entite cassable est cassable cible uniquement si elle n'est pas classee porte.

### Exclusions
Exemples:
- Entites critique gameplay ou serveur (spawn, logique map, triggers systeme).
- Entites explicitement exclues via config.

## Pipeline de decision
Ordre strict pour eviter toute regression gameplay:
1. Validation entite (existence, handle valide, non deja retiree).
2. Test exclusion explicite (liste config).
3. Test porte (prioritaire).
4. Test cassable non-porte.
5. Fallback ignore.

Pseudo-regle:
- Si entite est porte => OpenDoor(entite)
- Sinon si entite est cassable => BreakEntity(entite)
- Sinon => NoOp

## Gestion erreurs et performance
### Erreurs
- Chaque action est encapsulee en try/catch local pour eviter l'arret du traitement global.
- Les erreurs sont comptees et journalisees avec le classname et l'identifiant entite.
- En cas d'echec OpenDoor, aucune tentative de casse n'est autorisee pour cette entite.

### Performance
- Scanner iteratif sans allocations inutiles (liste preallouee si possible).
- Budget maximum d'entites traitees configurable pour prevenir les spikes CPU.
- Journalisation concise en production (mode verbose optionnel).

## Plan de tests
### Tests unitaires
- Classifier: porte prioritaire sur cassable.
- Classifier: exclusions respectees.
- Coordinator: portes routent vers OpenDoor uniquement.
- Coordinator: cassables non-portes routent vers BreakEntity.
- Coordinator: erreurs locales n'interrompent pas le lot.

### Tests d'integration (serveur local)
- Map avec portes rotatives: verification qu'elles sont ouvertes apres round_start.
- Map avec breakables mixtes: verification que seuls non-portes sont casses.
- Test charge: map dense en entites pour valider budget temps et stabilite.

### Validation gameplay
- Verification visuelle et logs: aucune porte cassee sur plusieurs rounds consecutifs.

## Extension prevue
- Strategie de classification par regles configurables (json/yaml) par map.
- Ajout de metriques (temps de traitement, taux de succes par type d'action).
- Hook optionnel post-round_start differe si certaines maps necessitent un delai.
