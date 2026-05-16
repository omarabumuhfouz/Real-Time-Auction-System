namespace MazadZone.Application.Features.Auctions.Queries;

public sealed record AuctionsListDto(
    Guid Id,
    string imageUrl, //primary image url
    string ItemTitle,
    decimal StartingPrice,
    DateTime StartTime,
    DateTime EndTime,
    TimeSpan RemainderTime, // EndTime - DateTime.UtcNow
    bool IsActive,
    int BidsCount
);