using MazadZone.Domain.Users;
using MazadZone.Domain.Users.ValueObjects;
using Shouldly;

namespace Tests.Domain.Users;

public class HashedRefreshTokenTests
{
    private const string DummyTokenHash = "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b";

    #region 1. Factory Creation Verification

    [Fact]
    public void Create_Should_InitializePropertiesCorrected_When_ValidParametersProvided()
    {
        // Arrange
        var userId = UserId.New();
        var executionTime = DateTime.UtcNow;

        // Act
        var result = HashedRefreshToken.Create(DummyTokenHash, userId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Token.ShouldBe(DummyTokenHash);
        result.Value.UserId.ShouldBe(userId);
        result.Value.RevokedAt.ShouldBeNull();
        
        // Tolerance check to handle execution drift for hardcoded DateTime.UtcNow
        result.Value.ExpiresAt.ShouldBeGreaterThan(executionTime);
    }

    #endregion

    #region 2. Computed State Matrix Evaluation

    [Fact]
    public void StateFlags_Should_ReflectActiveStatus_ImmediatelyAfterCreation()
    {
        // Arrange
        var token = HashedRefreshToken.Create(DummyTokenHash, UserId.New()).Value;

        // Assert
        token.IsExpired.ShouldBeFalse();
        token.IsRevoked.ShouldBeFalse();
        token.IsActive.ShouldBeTrue(); // Active condition: !IsExpired && !IsRevoked
    }

    #endregion

    #region 3. Revocation Domain Invariant Operations

    [Fact]
    public void Revoke_Should_MutateStateToInactive_When_TokenIsCurrentlyActive()
    {
        // Arrange
        var token = HashedRefreshToken.Create(DummyTokenHash, UserId.New()).Value;
        var approximateRevocationTime = DateTime.UtcNow;

        // Act
        token.Revoke();

        // Assert
        token.IsRevoked.ShouldBeTrue();
        token.IsActive.ShouldBeFalse();
        token.RevokedAt.ShouldNotBeNull();
        token.RevokedAt.Value.ShouldBeGreaterThanOrEqualTo(approximateRevocationTime);
    }

    [Fact]
    public void Revoke_Should_BeIdempotent_And_NotOverwiteRevokedAtTimestamp_When_CalledMultipleTimes()
    {
        // Arrange
        var token = HashedRefreshToken.Create(DummyTokenHash, UserId.New()).Value;
        token.Revoke();
        
        var originalRevokedAt = token.RevokedAt;
        
        // Artificially wait a tiny fraction to allow clock progression if system is extremely fast
        Thread.Sleep(1); 

        // Act
        token.Revoke();

        // Assert
        token.IsRevoked.ShouldBeTrue();
        token.RevokedAt.ShouldBe(originalRevokedAt); // Timestamp must remain unaltered
    }

    #endregion
}