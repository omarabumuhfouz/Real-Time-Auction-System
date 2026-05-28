using MazadZone.Application.Services;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Features.Users.Queries.GetProfileSettings;

public class GetProfileSettingsQueryHandler : IQueryHandler<GetProfileSettingsQuery, ProfileSettingsResponse>
{
    private readonly IUserQueries _userQueries;
    private readonly ILogger<GetProfileSettingsQueryHandler> _logger;

    public GetProfileSettingsQueryHandler(IUserQueries userQueries, ILogger<GetProfileSettingsQueryHandler> logger)
    {
        _userQueries = userQueries;
        _logger = logger;
    }

    public async Task<Result<ProfileSettingsResponse>> Handle(GetProfileSettingsQuery request, CancellationToken ct)
    {
        var user =  await _userQueries.GetProfileSettings(request.UserId, ct);
        if (user is null)
        {
            GlobalLogs.LogUserNotFound(_logger, request.UserId);
            return UserErrors.NotFound;
        }

        return user;
    }
}