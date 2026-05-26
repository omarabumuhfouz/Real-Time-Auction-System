using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using AuthService.Infrastructure.Services; // Ensure this matches your namespace

namespace MazadZone.Api;

public class DatabaseKeyProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCache _cache;

    public DatabaseKeyProvider(IServiceProvider serviceProvider, IMemoryCache cache)
    {
        _serviceProvider = serviceProvider;
        _cache = cache;
    }

    public IEnumerable<SecurityKey> GetKeys(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters)
    {
        // Cache the parsed RSA keys so we don't hit the database or parse base64 strings on every request
        return _cache.GetOrCreate("DB_SIGNING_KEYS", entry =>
        {
            // Cache duration. Adjust based on how quickly you want the API to pick up new keys.
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            // Create a scope to safely resolve your Scoped repository from this Singleton provider
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ISigningKeyRepository>();

            var dbKeys = repository.GetKeysForValidation();
            var securityKeys = new List<SecurityKey>();

            foreach (var dbKey in dbKeys)
            {
                // Instantiate RSA instance. Do NOT dispose it (no 'using'), 
                // because the JwtBearer handler needs it alive to validate the token.
                var rsa = RSA.Create();
                
                // Read the public key your background service generated
                rsa.ImportRSAPublicKey(Convert.FromBase64String(dbKey.PublicKey), out _);
                
                // Match the KeyId stored in the database with the one the token expects
                securityKeys.Add(new RsaSecurityKey(rsa) { KeyId = dbKey.KeyId });
            }

            return securityKeys;
        })!;
    }
}