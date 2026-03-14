namespace RetakePlugin.Contracts;

public interface IEntityAction
{
    string Name { get; }
    bool CanApply(IGameEntity entity);
    bool Apply(IGameEntity entity);
}
