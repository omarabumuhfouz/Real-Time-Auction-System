namespace MazadZone.Application.Features.Users.DTOs;

public record UserFilterParams(
    string? SearchTerm = null,
    string? SortBy = null,
    bool IsAsc = true,
    DateTime? JoinedDate = null,
    int PageNumber = 1,
    int PageSize = 10,
    bool IsExport = false,
    List<Guid>? SelectedUserIds = null // NEW: For "Export Selected"
);