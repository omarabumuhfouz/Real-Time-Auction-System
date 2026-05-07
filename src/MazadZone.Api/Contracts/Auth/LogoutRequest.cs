namespace MazadZone.Api.Contracts.Auth;

public sealed record LogoutRequest(string RefreshToken, bool IsLogoutFromAllDevices);