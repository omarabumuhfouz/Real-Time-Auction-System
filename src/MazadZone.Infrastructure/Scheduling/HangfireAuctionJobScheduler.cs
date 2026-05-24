using Hangfire;
using MediatR;
using MazadZone.Application.Features.Auctions.Commands.EndAuction;
using System;
using MazadZone.Application.Services;
using MazadZone.Domain.Auctions;

namespace MazadZone.Infrastructure.Scheduling;

public class HangfireAuctionJobScheduler : IAuctionJobScheduler
{
    public void ScheduleAuctionClosing(Guid auctionId, DateTimeOffset closeAt)
    {
        // Hangfire will generate deffird task
        BackgroundJob.Schedule<ISender>(
            mediator => mediator.Send(new EndAuctionCommand(AuctionId.From(auctionId)), default),
            closeAt
        );
    }
}