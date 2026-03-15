using BreakerAndOpenDoor.Config;
using BreakerAndOpenDoor.Core;

namespace BreakerAndOpenDoor.Adapters.CounterStrikeSharp;

public static class CounterStrikeSharpRuntimeFactory
{
    public static BreakerAndOpenDoorPlugin CreatePlugin(PluginConfig? config = null)
    {
        var effectiveConfig = config ?? new PluginConfig();
        return CreatePlugin(new CounterStrikeSharpEngineApi(effectiveConfig), effectiveConfig);
    }

    public static BreakerAndOpenDoorPlugin CreatePlugin(ICounterStrikeSharpApi api, PluginConfig? config = null)
    {
        var effectiveConfig = config ?? new PluginConfig();

        var provider = new CounterStrikeSharpEntityProvider(api);
        var scanner = new EntityScanner(provider);

        var classifier = new EntityClassifier(effectiveConfig);

        var actionApi = new CounterStrikeSharpGameActionApi(api);
        var executor = new ActionExecutor(actionApi);

        var coordinator = new RoundStartCoordinator(scanner, classifier, executor, effectiveConfig);
        return new BreakerAndOpenDoorPlugin(coordinator, effectiveConfig);
    }
}
