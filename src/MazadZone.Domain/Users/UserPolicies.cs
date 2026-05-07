namespace MazadZone.Domain.Users;

public static class UserPolicies
{
    public const int MaxActiveTokens = 5;
    public const int RefreshTokenLifespanDays = 7;

    public const int MinimumPasswordLength = 8;

    public const int MinimumAgeToRegister = 18;
}