using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Users.BackgroundJobs;

public interface IAutomaticUserReactivationJob : IScopedService
{
    Task ExecuteAsync(Guid userId, CancellationToken cancellationToken);
}
