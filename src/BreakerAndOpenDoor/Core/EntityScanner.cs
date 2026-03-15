using BreakerAndOpenDoor.Contracts;

namespace BreakerAndOpenDoor.Core;

public interface IEntityProvider
{
    IEnumerable<IGameEntity> GetAllEntities();
}

public sealed class EntityScanner
{
    private readonly IEntityProvider _entityProvider;

    public EntityScanner(IEntityProvider entityProvider)
    {
        _entityProvider = entityProvider;
    }

    public IReadOnlyList<IGameEntity> ScanEntities(int maxEntities)
    {
        var results = new List<IGameEntity>(Math.Max(128, maxEntities / 2));

        foreach (var entity in _entityProvider.GetAllEntities())
        {
            if (results.Count >= maxEntities)
            {
                break;
            }

            if (entity.IsValid)
            {
                results.Add(entity);
            }
        }

        return results;
    }
}
