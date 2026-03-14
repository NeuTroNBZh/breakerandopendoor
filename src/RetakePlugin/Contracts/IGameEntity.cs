namespace RetakePlugin.Contracts;

public interface IGameEntity
{
    string EntityId { get; }
    string ClassName { get; }
    bool IsValid { get; }
    bool IsDoorHint { get; }
    bool IsBreakableHint { get; }
}
