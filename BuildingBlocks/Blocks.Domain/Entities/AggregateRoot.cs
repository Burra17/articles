namespace Blocks.Domain.Entities;

public interface IAggregateRoot : IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}

public class AggregateRoot : Entity, IAggregateRoot
{
    private List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    protected void AddDomainEvent(IDomainEvent eventItem) => _domainEvents.Add(eventItem);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
