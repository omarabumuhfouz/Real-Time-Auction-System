namespace MazadZone.Infrastructure.Authentication;

public class SigningKey
{
    public Guid Id { get; init; }
    public string KeyId { get; init; }
    public string PrivateKey { get; init; }
    public string PublicKey { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }

    private SigningKey() { }

    private SigningKey(Guid id, string keyId, string privateKey, string publicKey, bool isActive, DateTime createdAt, DateTime expiresAt)
    {
        Id = id;
        KeyId = keyId;
        PrivateKey = privateKey;
        PublicKey = publicKey;
        IsActive = isActive;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Factory method to create a new signing key
    /// </summary>
    public static SigningKey Create(string keyId, string privateKey, string publicKey, bool isActive, DateTime expiresAt)
    {
        return new SigningKey(Guid.NewGuid(), keyId, privateKey, publicKey, isActive, DateTime.UtcNow, expiresAt);
    }

    /// <summary>
    /// Marks the key as active or inactive
    /// </summary>
    public SigningKey WithIsActive(bool isActive) => new(Id, KeyId, PrivateKey, PublicKey, isActive, CreatedAt, ExpiresAt);
}
