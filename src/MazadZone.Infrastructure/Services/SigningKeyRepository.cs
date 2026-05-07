// using MazadZone.Infrastructure.Authentication;
// using MazadZone.Infrastructure.Persistence;

// namespace AuthService.Infrastructure.Services;


// public class SigningKeyRepository(AppDbContext context) : ISigningKeyRepository
// {
//     public List<SigningKey> GetAllActiveSigningKeys()
//     {
//         return context.SigningKeys.Where(sk => sk.IsActive).ToList();
//     }
// }