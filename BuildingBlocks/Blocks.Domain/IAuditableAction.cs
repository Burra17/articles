namespace Blocks.Domain;

public interface IAuditableAction
{
    public DateTime CreatedOn => DateTime.UtcNow;
    public int CreatedById { get; set; }

    public string Action {  get; }
}

public interface IAuditableAction<TActionType> : IAuditableAction
    where TActionType : Enum
{
    TActionType ActionType { get; }

    string IAuditableAction.Action => ActionType.ToString();
}
