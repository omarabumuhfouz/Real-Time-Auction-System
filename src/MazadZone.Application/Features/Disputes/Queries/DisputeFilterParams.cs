namespace MazadZone.Application.Features.Disputes.Queries;

public record DisputeFilterParams
{
    // Search Box
    public string? SearchTerm { get; init; }

    // Dropdowns
    public string? Status { get; init; }
    public Guid? CategoryId { get; init; } // Maps to DisputeTypeId
    
    // Date Range (Submitted Date)
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }

    // Sorting
    public string? SortColumn { get; init; } = "SubmittedDate";
    public bool IsDescending { get; init; } = true;

    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;


    public bool IsExport { get; init; } = false;

}