using BreakerAndOpenDoor.Contracts;
using BreakerAndOpenDoor.Core;

namespace BreakerAndOpenDoor.Adapters.CounterStrikeSharp;

public sealed class CounterStrikeSharpGameActionApi : IGameActionApi
{
    private readonly ICounterStrikeSharpApi _api;

    public CounterStrikeSharpGameActionApi(ICounterStrikeSharpApi api)
    {
        _api = api;
    }

    public bool TryOpenDoor(IGameEntity entity)
    {
        return _api.TryOpenDoor(entity.EntityId);
    }

    public bool TryBreakEntity(IGameEntity entity)
    {
        return _api.TryBreakEntity(entity.EntityId);
    }
}
