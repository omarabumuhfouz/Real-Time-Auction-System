namespace MazadZone.Application.Features.Users.DTOs;

public record UserFilterParams(
    string? SearchTerm = null,
    string? SortBy = null,
    bool IsAsc = true,
    string? JoinedDate = null,
    int PageNumber = 1,
    int PageSize = 10,
    bool IsExport = false
)
{
    public DateTime? ValidJoinedDate
    {
        get
        {
            if (string.IsNullOrWhiteSpace(JoinedDate)) return null;

            return DateTime.TryParse(JoinedDate, out var parsedDate) ? parsedDate : null;
        }
    }
}