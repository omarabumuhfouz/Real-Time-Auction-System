using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

public interface IAuctionJobScheduler : IScopedService
{
    void ScheduleAuctionClosing(Guid auctionId, DateTimeOffset closeAt);
}