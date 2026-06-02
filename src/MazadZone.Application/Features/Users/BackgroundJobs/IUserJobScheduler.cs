using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Features.Users.BackgroundJobs;
public interface IUserJobScheduler : IScopedService
{
    void ScheduleUserReactivation(Guid userId, TimeSpan delay);
}