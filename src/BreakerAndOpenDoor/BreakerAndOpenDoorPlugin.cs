using BreakerAndOpenDoor.Config;
using BreakerAndOpenDoor.Core;
using BreakerAndOpenDoor.Adapters.CounterStrikeSharp;

namespace BreakerAndOpenDoor;

public sealed class BreakerAndOpenDoorPlugin
{
    private readonly RoundStartCoordinator _coordinator;
    private readonly PluginConfig _config;

    public BreakerAndOpenDoorPlugin(
        RoundStartCoordinator coordinator,
        PluginConfig config)
    {
        _coordinator = coordinator;
        _config = config;
    }

    public string ModuleName => "breakerandopendoor";
    public string ModuleVersion => "0.1.0";

    public static BreakerAndOpenDoorPlugin CreateWithCounterStrikeSharp(
        ICounterStrikeSharpApi api,
        PluginConfig? config = null)
    {
        return CounterStrikeSharpRuntimeFactory.CreatePlugin(api, config);
    }

    public void Initialize()
    {
        // Point d'integration CounterStrikeSharp attendu:
        // - Enregistrer le listener round_start
        // - Charger/valider la configuration
    }

    public void BeginRound()
    {
        _coordinator.BeginNewRound();
    }

    public RoundStartReport OnRoundStart()
    {
        var report = _coordinator.HandleRoundStart();

        if (_config.VerboseLogging)
        {
            // Stub de logging serveur.
            _ = report;
        }

        return report;
    }
}
