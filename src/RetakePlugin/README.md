# RetakePlugin

Core CS2 retake plugin implementation focused on round_start orchestration.

## Gameplay Rules
- Every door must be opened.
- No door must ever be broken.
- Every non-door breakable should be broken at round start.

## Architecture
- RetakePlugin.cs: plugin entry point and round lifecycle integration.
- Host/RetakePluginHost.cs: concrete CounterStrikeSharp plugin (BasePlugin) wired to events.
- Core/RoundStartCoordinator.cs: scan -> classification -> action orchestration.
- Core/EntityScanner.cs: entity collection through a testable abstraction.
- Core/EntityClassifier.cs: entity classification with door-first priority.
- Core/ActionExecutor.cs: action execution through an abstract engine API.
- Config/PluginConfig.cs: configurable classnames and limits.
- Contracts/*.cs: minimal contracts for unit tests and dependency inversion.
- Adapters/CounterStrikeSharp/*: concrete provider/action adapters plus runtime factory.

## CounterStrikeSharp Wiring
- Provide an ICounterStrikeSharpApi implementation that:
  - exposes EnumerateEntities()
  - executes TryOpenDoor(entityId)
  - executes TryBreakEntity(entityId)
- Create plugin runtime via RetakePlugin.CreateWithCounterStrikeSharp(api, config).

Included engine implementation:
- CounterStrikeSharpEngineApi uses Utilities.GetAllEntities() for enumeration.
- Actions use AcceptInput("Open"/"Break") with AddEntityIOEvent fallback.
- Direct wiring is available via CounterStrikeSharpRuntimeFactory.CreatePlugin(config).

Business-priority pipeline:
1. Exclusion
2. Door -> Open
3. Non-door breakable -> Break
4. Ignore

## CounterStrikeSharp Host Plugin
- Concrete class: Host/RetakePluginHost.cs
- Event hook: [GameEventHandler] OnRoundStart(EventRoundStart, GameEventInfo)
- Behavior: calls _runtime.OnRoundStart() then logs the execution report.
- Config loading: IPluginConfig<PluginConfig> via OnConfigParsed(PluginConfig).

## Example Plugin Config
Example CounterStrikeSharp configuration:

```json
{
	"EnableOpenDoors": true,
	"DoorOpenChancePercent": 70,
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

Gameplay options:
- `EnableOpenDoors`: enables door opening at round start.
- `DoorOpenChancePercent`: percentage chance (0-100) to open each door each round.
- `EnableBreakWindows`: breaks entities classified as window/glass.
- `EnableBreakVents`: breaks entities classified as vent/grate.
- `EnableBreakOtherBreakables`: breaks other detected breakables.

Ready-to-deploy config path:
- addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json

## Next Steps
1. Fine-tune classnames for your target retake maps.
2. Add unit tests for classifier and coordinator.
3. Add integration tests on target retake maps.

## Drop-in Release Bundle
Goal: produce a folder/zip that server admins can copy into `game/csgo/`.

Build prerequisite:
- .NET 8 SDK installed (`dotnet` available in PATH).

Provided script:
- `scripts/build-release.ps1`

Command:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build-release.ps1
```

Generated outputs:
- `artifacts/release/breakerandopendoor/addons/counterstrikesharp/plugins/breakerandopendoor/breakerandopendoor.dll`
- `artifacts/release/breakerandopendoor/addons/counterstrikesharp/plugins/breakerandopendoor/breakerandopendoor.deps.json`
- `artifacts/release/breakerandopendoor/addons/counterstrikesharp/plugins/breakerandopendoor/breakerandopendoor.pdb`
- `artifacts/release/breakerandopendoor/addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json`
- `artifacts/release/breakerandopendoor.zip`

Server deployment:
1. Open `artifacts/release/breakerandopendoor`.
2. Copy folder contents into server `game/csgo/`.
3. Restart the map (or hot reload if supported).

Compatibility notes:
- CounterStrikeSharp docs indicate plugins should be in `addons/counterstrikesharp/plugins/<DllName>/` with at least `.dll` + `.deps.json` (and usually `.pdb`).
- The community `create-cssharp-plugin` template follows the same publish layout.

## Troubleshooting "No visible effect"
If the plugin loads but no action is visible:
1. Set `VerboseLogging` to `true` in config.
2. Check `round_start(immediate)` then `round_start(delayed)` logs.
3. Inspect counters: `scanned`, `doors_detected`, `breakables_detected`, `unknown`.
4. If `doors_opened=0` and `breakables_broken=0`, tune `DoorClassNames` / `BreakableClassNames` for the map.

The plugin runs an additional delayed pass (`EnableSecondPassAfterDelay`, `SecondPassDelaySeconds`) for maps where entities are not yet stable on immediate round_start.

Nuke/Mirage vent-window notes:
- Break now tries multiple inputs: `Break`, `Shatter`, `Smash`, `Destroy`.
- Break detection also covers common tokens (`vent`, `window`, `glass`, `surf`).
- Final fallback can call `Remove()` for stubborn vent/window/glass entities.
