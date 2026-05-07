namespace MazadZone.Infrastructure.Outbox;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MazadZone.Domain.Primitives;
using MazadZone.Application.Common.Messaging;
using MazadZone.Infrastructure.Persistence;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using MazadZone.Infrastructure.Configuration;

public sealed class ProcessOutboxMessagesJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;
    private readonly OutboxOptions _options;
    private readonly IAsyncPolicy _retryPolicy;

    public ProcessOutboxMessagesJob(
        IServiceScopeFactory scopeFactory,
        ILogger<ProcessOutboxMessagesJob> logger,
        IAsyncPolicy retryPolicy,
        IOptions<OutboxOptions> options
        )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
        _retryPolicy = retryPolicy;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing outbox messages.");
            }

            // Wait 10 seconds before polling again. 
            // Adjust this based on how real-time you need emails/reactions to be.
            await Task.Delay(TimeSpan.FromSeconds(_options.IntervalInSeconds), stoppingToken);
        }
    }

    private async Task ProcessMessagesAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        // 1. Fetch top 20 unprocessed messages (batching prevents memory bloat)
        var messages = await dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .Take(_options.BatchSize)
            .ToListAsync(stoppingToken);

        if (!messages.Any()) return;

        foreach (var message in messages)
        {
            try
            {
                // 2. Deserialize back into the pure Domain Event
                var domainEvent = JsonConvert.DeserializeObject(
                    message.Content, 
                    Type.GetType(message.Type)!) as IDomainEvent;

                if (domainEvent is null) continue;

                // 3. Wrap it in our MediatR Envelope (like we did in the previous lesson)
                var notification = CreateNotification(domainEvent);

                // 4. Publish it to MediatR With Polly retry policy to handle transient failures
                await _retryPolicy.ExecuteAsync(async ()
                     => await publisher.Publish(notification, stoppingToken));

                // 5. Mark as processed
                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process outbox message {message.Id}");
                message.Error = ex.Message;
                // Note: We don't mark ProcessedOnUtc here, so it will retry next loop.
                // In a production app, you'd want a max-retry count so it doesn't loop forever.
            }
        }

        // 6. Save the Processed states back to the DB
        await dbContext.SaveChangesAsync(stoppingToken);
    }

    private static INotification CreateNotification(IDomainEvent domainEvent)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
    }
}