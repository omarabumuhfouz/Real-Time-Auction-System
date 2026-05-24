using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

public interface IOrderJobScheduler : IScopedService
{
    void ScheduleUnpaidOrderCancellation(Guid orderId, DateTimeOffset cancellationTime);
}