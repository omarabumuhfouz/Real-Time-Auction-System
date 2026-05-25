using System.Data;
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Users.Queries.GetProfileSettings;

public class GetProfileSettingsQueryHandler(IUserQueries userQueries) : IQueryHandler<GetProfileSettingsQuery, ProfileSettingsResponse>
{
    public async Task<Result<ProfileSettingsResponse>> Handle(GetProfileSettingsQuery request, CancellationToken ct)
    {
        return await userQueries.GetProfileSettings(request.UserId, ct) ??  ProfileSettingsResponse.Empty;
    }
}