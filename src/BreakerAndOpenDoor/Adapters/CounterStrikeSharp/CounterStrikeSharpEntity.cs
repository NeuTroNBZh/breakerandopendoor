using BreakerAndOpenDoor.Contracts;

namespace BreakerAndOpenDoor.Adapters.CounterStrikeSharp;

public sealed class CounterStrikeSharpEntity : IGameEntity
{
    public CounterStrikeSharpEntity(CounterStrikeSharpEntitySnapshot snapshot)
    {
        EntityId = snapshot.EntityId;
        ClassName = snapshot.ClassName;
        IsValid = snapshot.IsValid;
        IsDoorHint = snapshot.IsDoorHint;
        IsBreakableHint = snapshot.IsBreakableHint;
    }

    public string EntityId { get; }
    public string ClassName { get; }
    public bool IsValid { get; }
    public bool IsDoorHint { get; }
    public bool IsBreakableHint { get; }
}
