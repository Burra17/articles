using Blocks.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blocks.EntityFrameworkCore;

public static class MediatorExtensions
{
    public static async Task<int> DispatchDomainEventsAsync(this IMediator mediator, DbContext dbContext, CancellationToken ct = default)
    {
        var domainAggregates = dbContext.ChangeTracker.Entries<IAggregateEntity>();

        var domainEvents = domainAggregates
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainAggregates
            .ToList()
            .ForEach(domainAggregates => domainAggregates.Entity.ClearDomainEvents()); 

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent, ct);

        return domainEvents.Count;
    }
}
