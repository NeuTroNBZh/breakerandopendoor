using RetakePlugin.Contracts;
using RetakePlugin.Core;

namespace RetakePlugin.Adapters.CounterStrikeSharp;

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
