namespace MazadZone.Infrastructure.Persistence.Interceptors;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MazadZone.Infrastructure.Outbox;
using Newtonsoft.Json;
using MazadZone.Domain.Primitives;

public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context;
        if (dbContext is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var outboxMessages = dbContext.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot => 
            {
                var domainEvents = aggregateRoot.DomainEvents.ToList();
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().AssemblyQualifiedName!, // Stores the exact class type
                Content = JsonConvert.SerializeObject(
                    domainEvent, 
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })
            })
            .ToList();

        // Add them to the same DbContext! 
        // This guarantees they save ONLY if the business data saves (The Transactional Outbox)
        dbContext.Set<OutboxMessage>().AddRange(outboxMessages);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}