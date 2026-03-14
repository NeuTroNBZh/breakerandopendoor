# RetakePlugin

Squelette principal du plugin CS2 retake oriente round_start.

## Regles gameplay
- Toute porte doit etre ouverte.
- Aucune porte ne doit etre cassee.
- Tout cassable non-porte doit etre casse au debut du round.

## Architecture
- RetakePlugin.cs: entree plugin et hook round_start.
- Host/RetakePluginHost.cs: plugin CounterStrikeSharp concret (BasePlugin) branche sur round_start.
- Core/RoundStartCoordinator.cs: orchestration scan -> classification -> action.
- Core/EntityScanner.cs: collecte des entites depuis une abstraction testable.
- Core/EntityClassifier.cs: classification avec priorite portes.
- Core/ActionExecutor.cs: execution des actions via API moteur abstraite.
- Config/PluginConfig.cs: classnames et limites configurables.
- Contracts/*.cs: contrats minimaux pour test unitaire et inversion de dependances.
- Adapters/CounterStrikeSharp/*: adaptateurs concrets provider/actions + factory de wiring.

## Wiring CounterStrikeSharp
- Fournir une implementation de ICounterStrikeSharpApi qui:
	- expose EnumerateEntities()
	- execute TryOpenDoor(entityId)
	- execute TryBreakEntity(entityId)
- Creer le plugin via RetakePlugin.CreateWithCounterStrikeSharp(api, config).

Implementation moteur incluse:
- CounterStrikeSharpEngineApi utilise Utilities.GetAllEntities() pour l'enumeration.
- Les actions utilisent AcceptInput("Open"/"Break") avec fallback AddEntityIOEvent.
- Un wiring direct est disponible via CounterStrikeSharpRuntimeFactory.CreatePlugin(config).

Le pipeline conserve la priorite metier:
1. Exclusion
2. Porte -> Open
3. Cassable non-porte -> Break
4. Ignore

## Plugin hote CounterStrikeSharp
- Classe concrete: Host/RetakePluginHost.cs
- Hook evenement: [GameEventHandler] OnRoundStart(EventRoundStart, GameEventInfo)
- Comportement: appelle _runtime.OnRoundStart() puis log le rapport d'execution.
- Chargement config: IPluginConfig<PluginConfig> via OnConfigParsed(PluginConfig).

## Exemple de config plugin
Exemple de contenu pour le fichier de configuration CounterStrikeSharp du plugin:

```json
{
	"EnableOpenDoors": true,
	"EnableBreakWindows": true,
	"EnableBreakVents": true,
	"EnableBreakOtherBreakables": true,
	"MaxEntitiesPerRoundStart": 4096,
	"VerboseLogging": true,
	"DoorClassNames": [
		"func_door",
		"func_door_rotating",
		"prop_door_rotating"
	],
	"BreakableClassNames": [
		"func_breakable",
		"func_breakable_surf",
		"prop_physics",
		"prop_physics_multiplayer"
	],
	"ExcludedClassNames": [
		"info_player_terrorist",
		"info_player_counterterrorist",
		"logic_relay"
	]
}
```

Signification des options de gameplay:
- `EnableOpenDoors`: ouvre les portes au debut du round.
- `EnableBreakWindows`: casse les elements classes window/glass.
- `EnableBreakVents`: casse les elements classes vent/grate.
- `EnableBreakOtherBreakables`: casse les autres breakables detectes.

Fichier pret a deposer dans une arborescence serveur CounterStrikeSharp:
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json

## Etapes suivantes
1. Affiner les classnames selon les maps retake cibles.
2. Ajouter tests unitaires du classifier et du coordinator.
3. Ajouter tests d'integration serveur sur maps retake cibles.

## Release drop-in (pret a glisser-deposer)
Objectif: produire un dossier/zip que l'admin serveur copie dans `game/csgo/` et le plugin est operationnel.

Prerequis build:
- .NET 8 SDK installe (`dotnet` accessible dans le PATH).

Script fourni:
- `scripts/build-release.ps1`

Commande:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build-release.ps1
```

Sorties generees:
- `artifacts/release/breakerandopendoor/addons/counterstrikesharp/plugins/breakerandopendoor/breakerandopendoor.dll`
- `artifacts/release/breakerandopendoor/addons/counterstrikesharp/plugins/breakerandopendoor/breakerandopendoor.deps.json`
- `artifacts/release/breakerandopendoor/addons/counterstrikesharp/plugins/breakerandopendoor/breakerandopendoor.pdb`
- `artifacts/release/breakerandopendoor/addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json`
- `artifacts/release/breakerandopendoor.zip`

Deploy serveur:
1. Ouvrir `artifacts/release/breakerandopendoor`.
2. Copier le contenu du dossier dans `game/csgo/` du serveur.
3. Redemarrer la map (ou hot reload si serveur deja en route).

Notes de compatibilite (veille docs/plugins):
- Les docs officielles CounterStrikeSharp indiquent que le plugin doit etre dans `addons/counterstrikesharp/plugins/<NomDll>/` avec au minimum `.dll` + `.deps.json` (et generalement `.pdb`).
- Le template communautaire `create-cssharp-plugin` suit la meme logique de build/publish et de dossier plugin dedie.

## Diagnostic "ca ne fait rien"
Si le plugin charge mais aucune action n'est visible:
1. Mettre `VerboseLogging` a `true` dans la config.
2. Verifier les logs `round_start(immediate)` puis `round_start(delayed)`.
3. Lire les compteurs: `scanned`, `doors_detected`, `breakables_detected`, `unknown`.
4. Si `doors_opened=0` et `breakables_broken=0`, ajuster `DoorClassNames` / `BreakableClassNames` pour la map.

Le plugin execute une seconde passe apres un delai court (`EnableSecondPassAfterDelay`, `SecondPassDelaySeconds`) pour les maps ou l'etat des entites n'est pas encore stabilise sur l'event round_start immediat.

Cas Nuke/Mirage (vents/window):
- La casse essaye maintenant plusieurs inputs: `Break`, `Shatter`, `Smash`, `Destroy`.
- La detection breakable couvre aussi des tokens communs (`vent`, `window`, `glass`, `surf`).
- Fallback terminal active: `Remove()` pour entites vent/window/glass resistantes.
