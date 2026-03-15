using RetakePlugin.Config;
using RetakePlugin.Contracts;

namespace RetakePlugin.Core;

public enum EntityKind
{
    Excluded,
    Door,
    BreakableWindow,
    BreakableVent,
    BreakableOther,
    Unknown
}

public sealed class EntityClassifier
{
    private readonly PluginConfig _config;
    private static readonly string[] BreakableHintTokens =
    {
        "break",
        "shatter",
        "glass",
        "window",
        "vent",
        "surf"
    };

    public EntityClassifier(PluginConfig config)
    {
        _config = config;
    }

    public EntityKind Classify(IGameEntity entity)
    {
        var className = entity.ClassName ?? string.Empty;

        if (!entity.IsValid)
        {
            return EntityKind.Excluded;
        }

        if (_config.ExcludedClassNames.Contains(className))
        {
            return EntityKind.Excluded;
        }

            // Absolute gameplay rule: a door is never eligible for breaking.
        if (_config.DoorClassNames.Contains(className) || entity.IsDoorHint)
        {
            return EntityKind.Door;
        }

        var looksBreakable = _config.BreakableClassNames.Contains(className)
            || entity.IsBreakableHint
            || HasBreakableToken(className);

        if (looksBreakable)
        {
            if (IsWindowLike(className))
            {
                return EntityKind.BreakableWindow;
            }

            if (IsVentLike(className))
            {
                return EntityKind.BreakableVent;
            }

            return EntityKind.BreakableOther;
        }

        return EntityKind.Unknown;
    }

    private static bool HasBreakableToken(string className)
    {
        foreach (var token in BreakableHintTokens)
        {
            if (className.Contains(token, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsWindowLike(string className)
    {
        return className.Contains("window", StringComparison.OrdinalIgnoreCase)
            || className.Contains("glass", StringComparison.OrdinalIgnoreCase)
            || className.Contains("shatter", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsVentLike(string className)
    {
        return className.Contains("vent", StringComparison.OrdinalIgnoreCase)
            || className.Contains("grate", StringComparison.OrdinalIgnoreCase);
    }
}
