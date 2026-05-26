namespace MazadZone.Application.Features.Users.Queries.GetProfileSettings;

public record GetProfileSettingsQuery(UserId UserId) : IQuery<ProfileSettingsResponse>;
