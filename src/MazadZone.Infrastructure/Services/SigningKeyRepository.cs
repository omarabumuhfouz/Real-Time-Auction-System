using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Infrastructure.Authentication;
using MazadZone.Infrastructure.Persistence;

namespace AuthService.Infrastructure.Services;

public interface ISigningKeyRepository : IScopedService
{
    List<SigningKey> GetAllActiveSigningKeys();
    List<SigningKey> GetKeysForValidation(); 
}

public class SigningKeyRepository(AppDbContext context) : ISigningKeyRepository
{
    public List<SigningKey> GetAllActiveSigningKeys()
    {
        return context.SigningKeys.Where(sk => sk.IsActive).ToList();
    }

    // Use this method for JWT validation so recently rotated keys still work
    public List<SigningKey> GetKeysForValidation()
    {
        return context.SigningKeys
            .Where(sk => sk.IsActive || sk.ExpiresAt > DateTime.UtcNow)
            .ToList();
    }
}