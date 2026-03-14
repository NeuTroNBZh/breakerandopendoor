# Journal Central Projet - CS2 Retake Plugin

## Mission
Creer un plugin CS2 pour serveur retake qui casse les objets cassables en debut de round, sauf les portes qui doivent etre ouvertes.

## Regles de coordination inter-agents
1. Lire ce fichier avant toute tache.
2. Ecrire un log apres toute tache.
3. Ne pas ecraser l'historique precedent.

## Decisions techniques
- Multi-stack autorise: CounterStrikeSharp, SourceMod, Metamod:Source, selon besoin.
- Regle absolue: toute porte doit etre ouverte, jamais cassee.
- Delegation inter-agents autorisee, avec controle du chef d'orchestre.

## Journal d'execution

### 2026-03-14 - Initialisation
- Creation de l'agent orchestrateur: .github/agents/chef-orchestre-cs2.agent.md
- Creation du journal central: .github/agents/agent.md
- Scope confirme: plugin CS2 retake, casser objets cassables, ouvrir portes.

### 2026-03-14 - Validation utilisateur
- Stack confirmee: approche flexible, potentiellement CounterStrikeSharp + SourceMod + Metamod:Source selon besoin.
- Regle confirmee: toutes les portes doivent etre ouvertes.
- Regle confirmee: tout ce qui est cassable et n'est pas une porte doit etre casse.
- Gouvernance confirmee: delegation autorisee avec controle central.

### 2026-03-14 - Architecture initiale CounterStrikeSharp (Chef d'orchestre)
- Objectif traite: preparer l'architecture plugin round_start pour gestion portes + breakables avec priorite gameplay.
- Stack retenue pour ce lot: squelette principal CounterStrikeSharp en C# avec abstractions testables (provider/action API).
- Decision cle: pipeline de decision strict Exclusion -> Porte -> Cassable -> Ignore.
- Decision cle: regle absolue renforcee dans le classifier et le coordinator: une porte est toujours ouverte, jamais cassee.
- Decision cle: decomposition en couches (Scanner, Classifier, Executor, Coordinator) pour limiter le couplage et faciliter les tests.
- Gestion robustesse: try/catch local par entite pour eviter qu'une erreur stoppe tout le batch round_start.

#### Fichiers crees
- docs/architecture-cs2-retake-plugin.md
- src/RetakePlugin/RetakePlugin.cs
- src/RetakePlugin/Core/RoundStartCoordinator.cs
- src/RetakePlugin/Core/EntityScanner.cs
- src/RetakePlugin/Core/EntityClassifier.cs
- src/RetakePlugin/Core/ActionExecutor.cs
- src/RetakePlugin/Config/PluginConfig.cs
- src/RetakePlugin/Contracts/IGameEntity.cs
- src/RetakePlugin/Contracts/IEntityAction.cs
- src/RetakePlugin/README.md

#### Points ouverts
- Definir mapping exact des classnames portes/cassables sur le pool de maps retake cible.
- Implementer les adaptateurs concrets CounterStrikeSharp pour IEntityProvider et IGameActionApi.
- Ajouter suite de tests unitaires et tests d'integration serveur local.

### 2026-03-14 - Adaptateurs CounterStrikeSharp (Implementation)
- Objectif traite: ajouter les adaptateurs concrets CounterStrikeSharp pour scanner les entites et executer les actions ouvrir/casser.
- Decision cle: conserver les couches Core/Contracts intactes et brancher CounterStrikeSharp via une facade ICounterStrikeSharpApi.
- Decision cle: ajouter un factory runtime pour cabler scanner/classifier/executor/coordinator sans couplage fort au moteur.
- Regle gameplay verifiee: pipeline conserve la priorite porte (open only) avant cassable.

#### Fichiers crees
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpEntitySnapshot.cs
- src/RetakePlugin/Adapters/CounterStrikeSharp/ICounterStrikeSharpApi.cs
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpEntity.cs
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpEntityProvider.cs
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpGameActionApi.cs
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpRuntimeFactory.cs

#### Fichiers modifies
- src/RetakePlugin/RetakePlugin.cs
- src/RetakePlugin/README.md

### 2026-03-14 - ICounterStrikeSharpApi reel branche moteur
- Objectif traite: implementer une version reelle de ICounterStrikeSharpApi connectee au moteur CounterStrikeSharp.
- Decision cle: enumeration via Utilities.GetAllEntities() pour capturer l'etat map au round_start.
- Decision cle: actions portes/breakables via AcceptInput avec fallback AddEntityIOEvent pour robustesse runtime.
- Decision cle: ajout d'un overload de factory pour wiring direct (CreatePlugin(config)).

#### Fichiers crees
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpEngineApi.cs

#### Fichiers modifies
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpRuntimeFactory.cs
- src/RetakePlugin/README.md

### 2026-03-14 - Plugin hote CounterStrikeSharp (BasePlugin)
- Objectif traite: ajouter une classe plugin concrete CounterStrikeSharp qui declenche le pipeline au round_start.
- Decision cle: hote separe (RetakePluginHost) pour conserver le coeur metier decouple du framework.
- Decision cle: wiring runtime initialise en Load() avec repli defensif dans OnRoundStart si runtime null.
- Verification gameplay: la logique round_start appelee est celle du coordinator (portes ouvertes en priorite, jamais cassees).

#### Fichiers crees
- src/RetakePlugin/Host/RetakePluginHost.cs

#### Fichiers modifies
- src/RetakePlugin/README.md

### 2026-03-14 - Packaging release glisser-deposer
- Objectif traite: rendre le plugin installable en mode "drop-in" comme une vraie release.
- Decision cle: ajout d'un vrai projet .csproj net8 avec CounterStrikeSharp.API pour build/publish standard.
- Decision cle: ajout d'un script PowerShell qui publie puis assemble automatiquement l'arborescence `addons/counterstrikesharp/...` + zip.
- Decision cle: PluginConfig derive de BasePluginConfig pour alignement avec les conventions config CounterStrikeSharp.

#### Fichiers crees
- src/RetakePlugin/RetakePluginHost.csproj
- scripts/build-release.ps1

#### Fichiers modifies
- src/RetakePlugin/Config/PluginConfig.cs
- src/RetakePlugin/README.md

#### Validation
- Script de release execute localement: echec attendu dans cet environnement car `dotnet` absent du PATH.
- Action prise: ajout d'un pre-check explicite dans le script pour message d'erreur clair et prerequis documente.

### 2026-03-14 - Correctif detection dotnet hors PATH
- Objectif traite: rendre le script de release fonctionnel meme si dotnet est installe sans variable PATH.
- Decision cle: detection automatique de `dotnet.exe` via PATH, puis chemins usuels Windows (`Program Files`) et profil utilisateur.
- Validation: environnement local confirme `C:\Program Files\dotnet\dotnet.exe` disponible.

#### Fichiers modifies
- scripts/build-release.ps1

### 2026-03-14 - Correctifs build CounterStrikeSharp
- Objectif traite: corriger les erreurs compile detectees pendant `dotnet publish`.
- Decision cle: implementation explicite de `IPluginConfig<PluginConfig>.Config` dans le host.
- Decision cle: utilisation de l'attribut d'enregistrement depuis `CounterStrikeSharp.API.Core.Attributes.Registration`.
- Decision cle: script release durci pour stopper immediatement si `dotnet publish` echoue.

#### Fichiers modifies
- src/RetakePlugin/Host/RetakePluginHost.cs
- scripts/build-release.ps1

### 2026-03-14 - Validation release complete
- Objectif traite: obtenir une release reellement glisser-deposer operationnelle.
- Correctifs appliques: import `Microsoft.Extensions.Logging`, garde nullable runtime, detection dotnet hors PATH.
- Resultat: `dotnet publish` reussi et bundle release + zip generes.

#### Artefacts verifies
- artifacts/release/RetakePluginHost/addons/counterstrikesharp/plugins/RetakePluginHost/RetakePluginHost.dll
- artifacts/release/RetakePluginHost/addons/counterstrikesharp/plugins/RetakePluginHost/RetakePluginHost.deps.json
- artifacts/release/RetakePluginHost/addons/counterstrikesharp/plugins/RetakePluginHost/RetakePluginHost.pdb
- artifacts/release/RetakePluginHost/addons/counterstrikesharp/configs/plugins/RetakePluginHost/RetakePluginHost.json
- artifacts/release/RetakePluginHost.zip

### 2026-03-14 - Rename plugin + diagnostic runtime
- Objectif traite: renommer le plugin en `breakerandopendoor` et ajouter une methode fiable pour diagnostiquer le cas "ne fait rien".
- Decision cle: identite release/module/assembly/config migree vers `breakerandopendoor`.
- Decision cle: logs round_start enrichis avec compteurs de classification (scanned, doors_detected, breakables_detected, unknown).
- Decision cle: ajout d'une seconde passe differee (configurable) pour les maps ou round_start est trop tot.

#### Fichiers modifies
- src/RetakePlugin/Host/RetakePluginHost.cs
- src/RetakePlugin/Core/RoundStartCoordinator.cs
- src/RetakePlugin/Config/PluginConfig.cs
- src/RetakePlugin/RetakePluginHost.csproj
- scripts/build-release.ps1
- src/RetakePlugin/README.md

#### Fichiers crees
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json

### 2026-03-14 - Correctif vents/window non casses
- Objectif traite: forcer la casse des elements type vent/window observes sur Nuke et Mirage.
- Decision cle: detection breakable elargie (tokens `vent`, `window`, `glass`, `surf`).
- Decision cle: execution casse robuste avec sequence d'inputs `Break` puis `Shatter`, `Smash`, `Destroy` (direct + queue IO).
- Verification gameplay: priorite portes conservee, aucune casse de porte introduite.

#### Fichiers modifies
- src/RetakePlugin/Core/EntityClassifier.cs
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpEngineApi.cs
- src/RetakePlugin/Config/PluginConfig.cs
- src/RetakePlugin/README.md

### 2026-03-14 - Correctif renforce vents/window (tentative 2)
- Objectif traite: corriger le cas utilisateur "ca n'a pas marche" pour vents/window encore non casses.
- Decision cle: ajout de passes supplementaires apres round_start (extra passes) pour capturer les entites initialisees plus tard.
- Decision cle: ajout d'un fallback `Kill` cible pour entites type vent/window/glass/shatter/surf quand les inputs de casse standards echouent.
- Validation: build release reussi et nouveau bundle `breakerandopendoor` genere.

#### Fichiers modifies
- src/RetakePlugin/Host/RetakePluginHost.cs
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpEngineApi.cs
- src/RetakePlugin/Config/PluginConfig.cs
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json

### 2026-03-14 - Analyse logs serveur utilisateur + probe unknown
- Constat logs: le plugin casse bien les entites detectees (`breakables_broken` > 0) mais il reste un volume important d'entites `unknown`.
- Decision cle: ajout d'un mode "probe" sur unknown (filtre par tokens classname) pour tenter les inputs de casse et capturer les vents/window non classes.
- Decision cle: exposition de compteurs `unknown_probe_attempts` et `unknown_probe_broken` dans les logs round_start.
- Constat important: les erreurs `MissingMethodException` et `SQLiteException` dans les logs proviennent d'autres plugins (AFKManager, SimpleAdmin, etc.), pas de `breakerandopendoor`.

#### Fichiers modifies
- src/RetakePlugin/Core/RoundStartCoordinator.cs
- src/RetakePlugin/Host/RetakePluginHost.cs
- src/RetakePlugin/Config/PluginConfig.cs
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json

### 2026-03-14 - Correctif timing (race avec autres plugins)
- Constat logs utilisateur: `breakerandopendoor` casse des entites (counters > 0), mais certains elements semblent restaures ensuite selon map/stack.
- Hypothese: race condition avec plugin retake/autres callbacks executes apres `round_start`.
- Decision cle: ajout de passes supplementaires a `round_freeze_end` + passe tardive configurable pour casser apres la phase de reset.

#### Fichiers modifies
- src/RetakePlugin/Host/RetakePluginHost.cs
- src/RetakePlugin/Config/PluginConfig.cs
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json

### 2026-03-14 - Correctif final vents/window: Remove fallback
- Constat utilisateur: les compteurs de casse montent mais certains elements visibles restent intacts.
- Decision cle: ajout d'un fallback terminal `Remove()` pour les entites type vent/window/glass/shatter/surf resistantes aux inputs.
- Decision cle: changement de semantique succes casse pour limiter les faux positifs sur entites vent/window.
- Validation: build/release reussi avec le nouveau comportement.

#### Fichiers modifies
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpEngineApi.cs
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpRuntimeFactory.cs
- src/RetakePlugin/Config/PluginConfig.cs
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json
- src/RetakePlugin/README.md

### 2026-03-14 - Iteration diagnostic cible (commande dump)
- Constat utilisateur: ca ne marche toujours pas malgre compteurs eleves.
- Decision cle: ajout de la commande console `css_bod_dump_break_candidates` pour lister les classnames/index candidats et identifier l'entite exacte qui resiste.
- Decision cle: semantique succes casse rendue stricte (invalidation requise) pour eliminer les faux positifs de compteurs.
- Decision cle: probe unknown elargi aux classes `func_*` pour mieux couvrir les brushes map.

#### Fichiers modifies
- src/RetakePlugin/Host/RetakePluginHost.cs
- src/RetakePlugin/Core/RoundStartCoordinator.cs
- src/RetakePlugin/Adapters/CounterStrikeSharp/CounterStrikeSharpEngineApi.cs
- src/RetakePlugin/Config/PluginConfig.cs
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json

### 2026-03-14 - Options config gameplay par categorie
- Objectif traite: permettre aux admins de choisir si le plugin ouvre les portes, casse les windows, casse les vents, et casse les autres breakables.
- Decision cle: ajout de 4 toggles config (`EnableOpenDoors`, `EnableBreakWindows`, `EnableBreakVents`, `EnableBreakOtherBreakables`).
- Decision cle: classification separee des breakables en `window`, `vent`, `other` pour piloter chaque comportement independamment.
- Validation: build/release reussi avec les nouveaux toggles exposes dans la config JSON.

#### Fichiers modifies
- src/RetakePlugin/Config/PluginConfig.cs
- src/RetakePlugin/Core/EntityClassifier.cs
- src/RetakePlugin/Core/RoundStartCoordinator.cs
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json
- src/RetakePlugin/README.md

### 2026-03-14 - Professionalisation repository + release automation
- Objectif traite: preparer un depot GitHub professionnel et simple d'usage pour les admins serveurs.
- Decision cle: ajout de documentation racine claire (installation, config, build/release rapide).
- Decision cle: ajout des standards open-source (LICENSE, CONTRIBUTING, SECURITY, templates issues/PR).
- Decision cle: ajout de workflows GitHub Actions CI + Release (release automatique sur tag v*).

#### Fichiers crees
- README.md
- LICENSE
- CHANGELOG.md
- CONTRIBUTING.md
- SECURITY.md
- .gitignore
- .editorconfig
- .github/PULL_REQUEST_TEMPLATE.md
- .github/ISSUE_TEMPLATE/bug_report.yml
- .github/ISSUE_TEMPLATE/feature_request.yml
- .github/workflows/ci.yml
- .github/workflows/release.yml

### 2026-03-14 - Chargement config CounterStrikeSharp
- Objectif traite: charger la configuration plugin via le mecanisme natif CounterStrikeSharp.
- Decision cle: RetakePluginHost implemente IPluginConfig<PluginConfig> et reconstruit le runtime dans OnConfigParsed.
- Decision cle: normalisation defensive des listes (trim + comparer OrdinalIgnoreCase) pour garder un matching robuste.
- Verification gameplay: la regle porte prioritaire reste inchangee; seules les listes/parametres deviennent configurables runtime.

#### Fichiers modifies
- src/RetakePlugin/Host/RetakePluginHost.cs
- src/RetakePlugin/Config/PluginConfig.cs
- src/RetakePlugin/README.md

### 2026-03-14 - Fichier config exemple pret serveur
- Objectif traite: ajouter un fichier de config JSON directement dans une arborescence CounterStrikeSharp prete a deposer.
- Decision cle: chemin de deploiement retenu addons/counterstrikesharp/configs/plugins/RetakePluginHost/RetakePluginHost.json.
- Verification gameplay: les listes portes/cassables/exclusions restent conformes a la regle "portes ouvertes, jamais cassees".

#### Fichiers crees
- addons/counterstrikesharp/configs/plugins/RetakePluginHost/RetakePluginHost.json

#### Fichiers modifies
- src/RetakePlugin/README.md

## Questions ouvertes
- Quelles maps cibles en priorite pour les tests?
- Definition exacte d'une porte a exclure (classnames / entites specifiques)?
