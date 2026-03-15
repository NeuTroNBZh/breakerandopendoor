using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;
using BreakerAndOpenDoor.Adapters.CounterStrikeSharp;
using BreakerAndOpenDoor.Config;

namespace BreakerAndOpenDoor.Host;

[MinimumApiVersion(80)]
public sealed class BreakerAndOpenDoorHost : BasePlugin, IPluginConfig<PluginConfig>
{
    private global::BreakerAndOpenDoor.BreakerAndOpenDoorPlugin? _runtime;

    public PluginConfig Config { get; set; } = new();

    public override string ModuleName => "breakerandopendoor";
    public override string ModuleVersion => "0.1.0";
    public override string ModuleAuthor => "Retake Orchestrator";
    public override string ModuleDescription => "Round-start orchestration for doors and breakables.";

    public override void Load(bool hotReload)
    {
        BuildRuntime();

        Logger.LogInformation("breakerandopendoor loaded. hotReload={HotReload}", hotReload);
    }

    public override void Unload(bool hotReload)
    {
        Logger.LogInformation("breakerandopendoor unloaded. hotReload={HotReload}", hotReload);
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        EnsureRuntime();
        _runtime?.BeginRound();
        RunRoundStartSequence();

        if (Config.EnableLateRoundStartPass && Config.LateRoundStartDelaySeconds > 0)
        {
            AddTimer(Config.LateRoundStartDelaySeconds, () =>
            {
                if (_runtime is null)
                {
                    return;
                }

                ProcessRoundStartPass(_runtime, "late");
            });
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
    {
        if (!Config.EnableFreezeEndPass)
        {
            return HookResult.Continue;
        }

        EnsureRuntime();

        if (Config.FreezeEndPassDelaySeconds > 0)
        {
            AddTimer(Config.FreezeEndPassDelaySeconds, () =>
            {
                if (_runtime is null)
                {
                    return;
                }

                ProcessRoundStartPass(_runtime, "freeze_end");
            });
        }
        else if (_runtime is not null)
        {
            ProcessRoundStartPass(_runtime, "freeze_end");
        }

        return HookResult.Continue;
    }

    [ConsoleCommand("css_bod_dump_break_candidates", "Dump classnames potentially related to unbroken vents/windows")]
    public void OnDumpBreakCandidates(CCSPlayerController? player, CommandInfo command)
    {
        var counter = 0;

        foreach (var entity in CounterStrikeSharp.API.Utilities.GetAllEntities())
        {
            if (entity is null || !entity.IsValid)
            {
                continue;
            }

            var className = entity.DesignerName ?? string.Empty;
            if (!LooksLikeBreakCandidate(className))
            {
                continue;
            }

            Logger.LogInformation(
                "dump_candidate index={Index} classname={ClassName}",
                entity.Index,
                className);

            counter++;
            if (counter >= Config.DebugDumpMaxLines)
            {
                break;
            }
        }

        command.ReplyToCommand($"[breakerandopendoor] dump complete: {counter} lines");
    }

    private void EnsureRuntime()
    {
        if (_runtime is null)
        {
            BuildRuntime();
        }
    }

    private void RunRoundStartSequence()
    {
        var runtime = _runtime;
        if (runtime is null)
        {
            return;
        }

        ProcessRoundStartPass(runtime, "immediate");

        if (Config.EnableSecondPassAfterDelay && Config.SecondPassDelaySeconds > 0)
        {
            AddTimer(Config.SecondPassDelaySeconds, () =>
            {
                if (_runtime is null)
                {
                    return;
                }

                ProcessRoundStartPass(_runtime, "delayed");
            });
        }

        if (Config.AdditionalPassCount > 0 && Config.AdditionalPassIntervalSeconds > 0)
        {
            for (var i = 1; i <= Config.AdditionalPassCount; i++)
            {
                var passIndex = i;
                var delay = Config.SecondPassDelaySeconds + (Config.AdditionalPassIntervalSeconds * passIndex);

                AddTimer(delay, () =>
                {
                    if (_runtime is null)
                    {
                        return;
                    }

                    ProcessRoundStartPass(_runtime, $"extra-{passIndex}");
                });
            }
        }
    }

    public void OnConfigParsed(PluginConfig config)
    {
        Config = Normalize(config);
        BuildRuntime();

        Logger.LogInformation(
            "Config parsed: max_entities={MaxEntities}, verbose={Verbose}, doors={DoorCount}, breakables={BreakableCount}, excluded={ExcludedCount}",
                Config.MaxEntitiesPerRoundStart,
                Config.VerboseLogging,
                Config.DoorClassNames.Count,
                Config.BreakableClassNames.Count,
                Config.ExcludedClassNames.Count);
    }

    private void BuildRuntime()
    {
        _runtime = CounterStrikeSharpRuntimeFactory.CreatePlugin(Config);
        _runtime.Initialize();
    }

    private void ProcessRoundStartPass(global::BreakerAndOpenDoor.BreakerAndOpenDoorPlugin runtime, string passName)
    {
        var report = runtime.OnRoundStart();

        Logger.LogInformation(
            "round_start({Pass}) scanned={Scanned}, doors_detected={DoorsDetected}, breakables_detected={BreakablesDetected}, excluded={Excluded}, unknown={Unknown}, unknown_probe_attempts={UnknownProbeAttempts}, unknown_probe_broken={UnknownProbeBroken}, doors_opened={DoorsOpened}, doors_skipped_random={DoorsSkippedRandom}, breakables_broken={BreakablesBroken}, errors={Errors}",
            passName,
            report.Scanned,
            report.DoorsDetected,
            report.BreakablesDetected,
            report.ExcludedDetected,
            report.UnknownDetected,
            report.UnknownProbeAttempts,
            report.UnknownProbeBroken,
            report.DoorsOpened,
            report.DoorsSkippedByRandom,
            report.BreakablesBroken,
            report.Errors);

        if (report.DoorsOpened == 0 && report.BreakablesBroken == 0)
        {
            Logger.LogWarning(
                "round_start({Pass}) no action performed. Check classnames in config and verify map has target entities.",
                passName);
        }

        if (report.BreakablesDetected > 0 && report.BreakablesBroken == 0)
        {
            Logger.LogWarning(
                "round_start({Pass}) breakables detected but none broken. Inputs may not match map entities; fallback strategy should engage.",
                passName);
        }
    }

    private static PluginConfig Normalize(PluginConfig config)
    {
        var doorOpenChancePercent = Math.Clamp(config.DoorOpenChancePercent, 0, 100);

        return new PluginConfig
        {
            EnableOpenDoors = config.EnableOpenDoors,
            DoorOpenChancePercent = doorOpenChancePercent,
            EnableBreakWindows = config.EnableBreakWindows,
            EnableBreakVents = config.EnableBreakVents,
            EnableBreakOtherBreakables = config.EnableBreakOtherBreakables,
            MaxEntitiesPerRoundStart = config.MaxEntitiesPerRoundStart,
            VerboseLogging = config.VerboseLogging,
            EnableSecondPassAfterDelay = config.EnableSecondPassAfterDelay,
            SecondPassDelaySeconds = config.SecondPassDelaySeconds,
            AdditionalPassCount = config.AdditionalPassCount,
            AdditionalPassIntervalSeconds = config.AdditionalPassIntervalSeconds,
            EnableLateRoundStartPass = config.EnableLateRoundStartPass,
            LateRoundStartDelaySeconds = config.LateRoundStartDelaySeconds,
            EnableFreezeEndPass = config.EnableFreezeEndPass,
            FreezeEndPassDelaySeconds = config.FreezeEndPassDelaySeconds,
            EnableKillFallbackForVentWindow = config.EnableKillFallbackForVentWindow,
            EnableRemoveFallbackForVentWindow = config.EnableRemoveFallbackForVentWindow,
            ProbeUnknownEntitiesForBreakInput = config.ProbeUnknownEntitiesForBreakInput,
            UnknownProbeMaxPerPass = config.UnknownProbeMaxPerPass,
            DebugDumpMaxLines = config.DebugDumpMaxLines,
            UnknownProbeClassNameTokens = ToOrdinalIgnoreCaseSet(config.UnknownProbeClassNameTokens),
            DoorClassNames = ToOrdinalIgnoreCaseSet(config.DoorClassNames),
            BreakableClassNames = ToOrdinalIgnoreCaseSet(config.BreakableClassNames),
            ExcludedClassNames = ToOrdinalIgnoreCaseSet(config.ExcludedClassNames)
        };
    }

    private static HashSet<string> ToOrdinalIgnoreCaseSet(IEnumerable<string>? source)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (source is null)
        {
            return result;
        }

        foreach (var value in source)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            result.Add(value.Trim());
        }

        return result;
    }

    private static bool LooksLikeBreakCandidate(string className)
    {
        if (className.StartsWith("func_", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return className.Contains("vent", StringComparison.OrdinalIgnoreCase)
            || className.Contains("window", StringComparison.OrdinalIgnoreCase)
            || className.Contains("glass", StringComparison.OrdinalIgnoreCase)
            || className.Contains("shatter", StringComparison.OrdinalIgnoreCase)
            || className.Contains("surf", StringComparison.OrdinalIgnoreCase)
            || className.Contains("break", StringComparison.OrdinalIgnoreCase)
            || className.Contains("grate", StringComparison.OrdinalIgnoreCase)
            || className.Contains("prop_dynamic", StringComparison.OrdinalIgnoreCase);
    }
}
