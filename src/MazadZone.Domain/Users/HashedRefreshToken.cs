using MazadZone.Domain.Users.ValueObjects;
namespace MazadZone.Domain.Users; 

public class HashedRefreshToken : Entity<HashedRefreshTokenId>
{
    private HashedRefreshToken() { }

    private HashedRefreshToken(HashedRefreshTokenId id, string token, UserId userId, DateTime expiresAt) : base(id)
    {
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;
    }

    public string Token { get; private set; } 
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public UserId UserId { get; private set; }

    // 3. Clean, deterministic state checks
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;
    public bool IsExpired =>   DateTime.UtcNow >= ExpiresAt;


    public static Result<HashedRefreshToken> Create(string token, UserId userId)
    {
        return new HashedRefreshToken(
            HashedRefreshTokenId.New(),
            token,
            userId,
            DateTime.UtcNow.AddDays(UserPolicies.RefreshTokenLifespanDays));
    }

    public void Revoke()
    {
        if (IsRevoked) return; 

        RevokedAt = DateTime.UtcNow;
    }
}
