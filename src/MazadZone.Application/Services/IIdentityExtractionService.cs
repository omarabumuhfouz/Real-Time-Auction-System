using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

public record ExtractedIdData(string? NationalId, bool Success, string? ErrorMessage);

public interface IIdentityExtractionService : IScopedService
{
    Task<ExtractedIdData> ExtractDataAsync(byte[] imageBytes);
}
