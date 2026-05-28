namespace MazadZone.Application.Features.Disputes.Queries.ExportDisputes;

public record ExportDisputesQuery(
    string? SearchTerm,
    string? Status,
    Guid? CategoryId,
    DateTime? FromDate,
    DateTime? ToDate,
    string? SortColumn,
    bool IsDescending,
    int PageNumber,
    int PageSize
)
 : IQuery<ExportFileResponse>;