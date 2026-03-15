using BreakerAndOpenDoor.Contracts;
using BreakerAndOpenDoor.Core;

namespace BreakerAndOpenDoor.Adapters.CounterStrikeSharp;

public sealed class CounterStrikeSharpEntityProvider : IEntityProvider
{
    private readonly ICounterStrikeSharpApi _api;

    public CounterStrikeSharpEntityProvider(ICounterStrikeSharpApi api)
    {
        _api = api;
    }

    public IEnumerable<IGameEntity> GetAllEntities()
    {
        foreach (var snapshot in _api.EnumerateEntities())
        {
            yield return new CounterStrikeSharpEntity(snapshot);
        }
    }
}
