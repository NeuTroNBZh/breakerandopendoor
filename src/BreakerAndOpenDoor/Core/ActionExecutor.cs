using BreakerAndOpenDoor.Contracts;

namespace BreakerAndOpenDoor.Core;

public interface IGameActionApi
{
    bool TryOpenDoor(IGameEntity entity);
    bool TryBreakEntity(IGameEntity entity);
}

public sealed class ActionExecutor
{
    private readonly IGameActionApi _api;

    public ActionExecutor(IGameActionApi api)
    {
        _api = api;
    }

    public bool OpenDoor(IGameEntity entity)
    {
        return _api.TryOpenDoor(entity);
    }

    public bool BreakEntity(IGameEntity entity)
    {
        return _api.TryBreakEntity(entity);
    }
}
