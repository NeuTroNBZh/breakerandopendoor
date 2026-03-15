using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using RetakePlugin.Config;

namespace RetakePlugin.Adapters.CounterStrikeSharp;

public sealed class CounterStrikeSharpEngineApi : ICounterStrikeSharpApi
{
    private static readonly string[] BreakInputs = { "Break", "Shatter", "Smash", "Destroy" };
    private static readonly string[] RemoveInputs = { "Kill", "KillHierarchy" };
    private readonly PluginConfig _config;

    public CounterStrikeSharpEngineApi(PluginConfig? config = null)
    {
        _config = config ?? new PluginConfig();
    }
    public IEnumerable<CounterStrikeSharpEntitySnapshot> EnumerateEntities()
    {
        foreach (var entity in Utilities.GetAllEntities())
        {
            if (entity is null)
            {
                continue;
            }

            var className = entity.DesignerName ?? string.Empty;
            var entityId = entity.Index.ToString();

            yield return new CounterStrikeSharpEntitySnapshot(
                EntityId: entityId,
                ClassName: className,
                IsValid: entity.IsValid,
                IsDoorHint: IsDoorHint(className),
                IsBreakableHint: IsBreakableHint(className));
        }
    }

    public bool TryOpenDoor(string entityId)
    {
        if (!TryResolveEntity(entityId, out var entity))
        {
            return false;
        }

        try
        {
            // Defensive sequence: Unlock then Open covers more mapper-side door variants.
            entity.AcceptInput("Unlock");
            entity.AcceptInput("Open");
            return true;
        }
        catch
        {
            try
            {
                entity.AddEntityIOEvent("Open");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public bool TryBreakEntity(string entityId)
    {
        if (!TryResolveEntity(entityId, out var entity))
        {
            return false;
        }

        var className = entity.DesignerName ?? string.Empty;
        var ventWindowLike = IsVentWindowLike(className);
        var acceptedDestructiveInput = false;

        if (TryAcceptInputs(entity, BreakInputs))
        {
            acceptedDestructiveInput = true;
        }

        if (!entity.IsValid)
        {
            return true;
        }

        if (TryQueuedInputs(entity, BreakInputs))
        {
            acceptedDestructiveInput = true;
        }

        if (!entity.IsValid)
        {
            return true;
        }

        if (_config.EnableKillFallbackForVentWindow && ventWindowLike)
        {
            if (TryAcceptInputs(entity, RemoveInputs) || TryQueuedInputs(entity, RemoveInputs))
            {
                acceptedDestructiveInput = true;
            }

            if (!entity.IsValid)
            {
                return true;
            }
        }

        if (_config.EnableRemoveFallbackForVentWindow && ventWindowLike)
        {
            try
            {
                entity.Remove();
                return true;
            }
            catch
            {
                // Keep evaluating below.
            }
        }

        if (!entity.IsValid)
        {
            return true;
        }

        // Some map vents break asynchronously after input dispatch and may still be
        // valid in the same server tick; treat accepted destructive input as success.
        if (ventWindowLike && acceptedDestructiveInput)
        {
            return true;
        }

        // For stubborn map entities, accepting an input is not enough; we require
        // invalidation to report a successful break and avoid false positives.
        return false;
    }

    private static bool TryResolveEntity(string entityId, out CEntityInstance entity)
    {
        entity = null!;

        if (!uint.TryParse(entityId, out var index))
        {
            return false;
        }

        var resolved = Utilities.GetEntityFromIndex<CEntityInstance>((int)index);
        if (resolved is null || !resolved.IsValid)
        {
            return false;
        }

        entity = resolved;
        return true;
    }

    private static bool IsDoorHint(string className)
    {
        return className.Contains("door", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsBreakableHint(string className)
    {
        return className.Contains("breakable", StringComparison.OrdinalIgnoreCase)
            || className.Contains("physics", StringComparison.OrdinalIgnoreCase)
            || className.Contains("prop_dynamic", StringComparison.OrdinalIgnoreCase)
            || className.Contains("shatter", StringComparison.OrdinalIgnoreCase)
            || className.Contains("window", StringComparison.OrdinalIgnoreCase)
            || className.Contains("vent", StringComparison.OrdinalIgnoreCase)
            || className.Contains("glass", StringComparison.OrdinalIgnoreCase)
            || className.Contains("surf", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryAcceptInputs(CEntityInstance entity, IEnumerable<string> inputs)
    {
        foreach (var input in inputs)
        {
            try
            {
                entity.AcceptInput(input);
                return true;
            }
            catch
            {
                // Try next input.
            }
        }

        return false;
    }

    private static bool TryQueuedInputs(CEntityInstance entity, IEnumerable<string> inputs)
    {
        foreach (var input in inputs)
        {
            try
            {
                entity.AddEntityIOEvent(input);
                return true;
            }
            catch
            {
                // Try next input.
            }
        }

        return false;
    }

    private static bool IsVentWindowLike(string className)
    {
        return className.Contains("vent", StringComparison.OrdinalIgnoreCase)
            || className.Contains("window", StringComparison.OrdinalIgnoreCase)
            || className.Contains("glass", StringComparison.OrdinalIgnoreCase)
            || className.Contains("shatter", StringComparison.OrdinalIgnoreCase)
            || className.Contains("surf", StringComparison.OrdinalIgnoreCase)
            || className.Contains("grate", StringComparison.OrdinalIgnoreCase)
            || className.Contains("breakable", StringComparison.OrdinalIgnoreCase)
            || className.Contains("prop_dynamic", StringComparison.OrdinalIgnoreCase);
    }
}
