using RetakePlugin.Config;
using RetakePlugin.Core;
using RetakePlugin.Adapters.CounterStrikeSharp;

namespace RetakePlugin;

public sealed class RetakePlugin
{
    private readonly RoundStartCoordinator _coordinator;
    private readonly PluginConfig _config;

    public RetakePlugin(
        RoundStartCoordinator coordinator,
        PluginConfig config)
    {
        _coordinator = coordinator;
        _config = config;
    }

    public string ModuleName => "RetakePlugin";
    public string ModuleVersion => "0.1.0";

    public static RetakePlugin CreateWithCounterStrikeSharp(
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
