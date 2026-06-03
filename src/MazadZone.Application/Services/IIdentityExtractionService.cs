using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

public record ExtractedIdData(
    string? NationalId,
    bool Success,
    string? ErrorMessage,
    string? EnglishFullName = null,
    string? ArabicFullName = null,
    string? DateOfBirth = null,
    string? Gender = null);

public interface IIdentityExtractionService : IScopedService
{
    Task<ExtractedIdData> ExtractDataAsync(byte[] imageBytes);
}
