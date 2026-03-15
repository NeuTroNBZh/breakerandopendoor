namespace BreakerAndOpenDoor.Adapters.CounterStrikeSharp;

public sealed record CounterStrikeSharpEntitySnapshot(
    string EntityId,
    string ClassName,
    bool IsValid,
    bool IsDoorHint,
    bool IsBreakableHint);
