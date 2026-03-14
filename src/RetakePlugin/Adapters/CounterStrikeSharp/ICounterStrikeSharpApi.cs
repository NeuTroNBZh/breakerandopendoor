namespace RetakePlugin.Adapters.CounterStrikeSharp;

public interface ICounterStrikeSharpApi
{
    IEnumerable<CounterStrikeSharpEntitySnapshot> EnumerateEntities();
    bool TryOpenDoor(string entityId);
    bool TryBreakEntity(string entityId);
}
