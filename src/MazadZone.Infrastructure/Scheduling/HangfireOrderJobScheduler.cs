using System;
using Hangfire;
using MediatR;
using MazadZone.Application.Features.Orders.Commands.CancelOrder;
using MazadZone.Domain.Orders;
using MazadZone.Application.Services;

namespace MazadZone.Infrastructure.Scheduling;

public class HangfireOrderJobScheduler : IOrderJobScheduler
{
    public void ScheduleUnpaidOrderCancellation(Guid orderId, DateTimeOffset cancellationTime)
    {
        BackgroundJob.Schedule<ISender>(
            mediator => mediator.Send(new CancelOrderCommand(OrderId.From(orderId)), default),
            cancellationTime
        );
    }

}