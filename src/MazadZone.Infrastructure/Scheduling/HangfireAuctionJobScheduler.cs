using Hangfire;
using MediatR;
using MazadZone.Application.Features.Auctions.Commands.EndAuction;
using System;
using MazadZone.Application.Services;
using MazadZone.Domain.Auctions;
using MazadZone.Application.Features.Auctions.Commands.ActivateAuction;

namespace MazadZone.Infrastructure.Scheduling;

public class HangfireAuctionJobScheduler : IAuctionJobScheduler
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireAuctionJobScheduler(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void ScheduleAuctionClosing(Guid auctionId, DateTimeOffset closeAt)
    {
        // Hangfire will generate deffird task
        _backgroundJobClient.Schedule<ISender>(
            mediator => mediator.Send(new EndAuctionCommand(AuctionId.From(auctionId)), default),
            closeAt
        );
    }

    public void ScheduleAuctionStarting(Guid auctionId, DateTimeOffset startAt)
    {
        _backgroundJobClient.Schedule<ISender>(
            mediator => mediator.Send(new ActivateAuctionCommand(AuctionId.From(auctionId)), default),
            startAt
        );
    }
}