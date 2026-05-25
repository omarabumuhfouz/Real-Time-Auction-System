using AuthService.Domain.Constants;
using MazadZone.Infrastructure.Authentication;
using MazadZone.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;

namespace AuthService.Infrastructure.Backgrounds;

public class KeyRotationService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly TimeSpan _rotationInterval = TimeSpan.FromDays(SigningKeySettings.RotationIntervalDays);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // For first Attempt
        await RotateKeysAsync(); 

        var periodicTimer = new PeriodicTimer(_rotationInterval);

        while (await periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            await RotateKeysAsync();
        }
    }

    private async Task RotateKeysAsync()
{
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var activeKey = await context.SigningKeys.FirstOrDefaultAsync(k => k.IsActive);

        // Check if there’s no active key or if the active key is about to expire.
        if (activeKey == null || activeKey.ExpiresAt <= DateTime.UtcNow.AddDays(10))
        {
            // If there's an active key, mark it as inactive.
            if (activeKey != null)
            {
                // Mark the current key as inactive since it’s about to be replaced.
                activeKey = activeKey.WithIsActive(false);

                // Update the current key in the database.
                context.SigningKeys.Update(activeKey);
            }

            // Generate a new RSA key pair.
            using var rsa = RSA.Create(2048);

            // Export the private key as a Base64-encoded string.
            var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

            // Export the public key as a Base64-encoded string.
            var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());

            // Generate a unique identifier for the new key.
            var newKeyId = Guid.NewGuid().ToString();

            // Create a new SigningKey entity with the new RSA key details.
            var newKey =  SigningKey.Create
            (
                 newKeyId,
                 privateKey,
                 publicKey,
                 true,
                 DateTime.UtcNow.AddYears(1)
            );

            await context.SigningKeys.AddAsync(newKey);

            await context.SaveChangesAsync();
        }
    }
}