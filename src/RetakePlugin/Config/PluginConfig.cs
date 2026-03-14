using CounterStrikeSharp.API.Core;

namespace RetakePlugin.Config;

public sealed class PluginConfig : BasePluginConfig
{
    public bool EnableOpenDoors { get; set; } = true;
    public bool EnableBreakWindows { get; set; } = true;
    public bool EnableBreakVents { get; set; } = true;
    public bool EnableBreakOtherBreakables { get; set; } = true;

    public int MaxEntitiesPerRoundStart { get; set; } = 4096;
    public bool VerboseLogging { get; set; } = false;
    public bool EnableSecondPassAfterDelay { get; set; } = true;
    public float SecondPassDelaySeconds { get; set; } = 0.20f;
    public int AdditionalPassCount { get; set; } = 4;
    public float AdditionalPassIntervalSeconds { get; set; } = 0.25f;
    public bool EnableLateRoundStartPass { get; set; } = true;
    public float LateRoundStartDelaySeconds { get; set; } = 2.50f;
    public bool EnableFreezeEndPass { get; set; } = true;
    public float FreezeEndPassDelaySeconds { get; set; } = 0.10f;
    public bool EnableKillFallbackForVentWindow { get; set; } = true;
    public bool EnableRemoveFallbackForVentWindow { get; set; } = true;
    public bool ProbeUnknownEntitiesForBreakInput { get; set; } = true;
    public int UnknownProbeMaxPerPass { get; set; } = 64;
    public int DebugDumpMaxLines { get; set; } = 200;
    public HashSet<string> UnknownProbeClassNameTokens { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "vent",
        "window",
        "glass",
        "shatter",
        "surf",
        "grate",
        "break",
        "brush",
        "func_",
        "prop_dynamic"
    };

    public HashSet<string> DoorClassNames { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "func_door",
        "func_door_rotating",
        "prop_door_rotating"
    };

    public HashSet<string> BreakableClassNames { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "func_breakable",
        "func_breakable_surf",
        "func_window",
        "func_glass",
        "prop_physics",
            "prop_physics_multiplayer",
        "prop_glass",
        "prop_vent",
        "func_shatterglass"
    };

    public HashSet<string> ExcludedClassNames { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "info_player_terrorist",
        "info_player_counterterrorist",
        "logic_relay"
    };
}
