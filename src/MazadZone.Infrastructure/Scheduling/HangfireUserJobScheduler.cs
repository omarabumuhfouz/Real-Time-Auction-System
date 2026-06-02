using Hangfire;
using MazadZone.Application.Features.Users.BackgroundJobs;
using MazadZone.Application.Users.BackgroundJobs;

namespace MazadZone.Infrastructure.Scheduling;

public class HangfireUserJobScheduler : IUserJobScheduler
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireUserJobScheduler(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void ScheduleUserReactivation(Guid userId, TimeSpan delay)
    {
        _backgroundJobClient.Schedule<IAutomaticUserReactivationJob>(
            job => job.ExecuteAsync(userId, CancellationToken.None), 
            delay
        );
    }
}