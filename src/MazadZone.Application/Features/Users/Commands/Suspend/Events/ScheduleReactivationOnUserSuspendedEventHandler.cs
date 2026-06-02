using MazadZone.Application.Features.Users.BackgroundJobs;
using MazadZone.Application.Services;
using MazadZone.Domain.Users.Events;

namespace MazadZone.Application.Features.Users.Commands.Suspend.Events;

public class ScheduleReactivationOnUserSuspendedEventHandler 
    : INotificationHandler<UserSuspendedDomainEvent>
{
    private readonly IUserJobScheduler _jobScheduler;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ScheduleReactivationOnUserSuspendedEventHandler(
        IUserJobScheduler jobScheduler,
        IDateTimeProvider dateTimeProvider
        )
    {
        _jobScheduler = jobScheduler;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(UserSuspendedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.ReinstatementDate is null)
        {
            return Task.CompletedTask;
        }

        TimeSpan delay = notification.ReinstatementDate.Value - _dateTimeProvider.Now;

        if (delay.TotalMilliseconds <= 0)
        {
            return Task.CompletedTask;
        }

        // Delegate the actual scheduling to the Infrastructure layer
        _jobScheduler.ScheduleUserReactivation(notification.UserId.Value, delay);

        return Task.CompletedTask;
    }
}