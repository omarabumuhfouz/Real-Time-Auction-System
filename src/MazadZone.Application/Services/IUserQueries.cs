using MazadZone.Application.Features.Users.DTOs;
using MazadZone.Application.Features.Users.Queries.GetProfileSettings;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

public interface IUserQueries : IScopedService
{
    //Task<Result<UserDto>> GetUserByIdAsync(UserId userId, CancellationToken cancellationToken);
   // Task<Result<UserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task<Result<Address>> GetAddressByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<Email>> GetEmailByIdAsync(Guid userId, CancellationToken cancellationToken); 
    Task<ProfileSettingsResponse> GetProfileSettings(UserId userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserDto>> GetUsersAsync(UserFilterParams filter, CancellationToken ct);

}