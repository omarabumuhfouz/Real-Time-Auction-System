namespace MazadZone.Application.Features.Disputes.Queries.ExportDisputes;

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class ExportDisputesQueryHandler : IQueryHandler<ExportDisputesQuery, ExportFileResponse>
{
    private readonly IDisputeQueries _repository;

    public ExportDisputesQueryHandler(IDisputeQueries repository)
    {
        _repository = repository;
    }

    public async Task<Result<ExportFileResponse>> Handle(ExportDisputesQuery request, CancellationToken ct)
    {
        // 1. Map to Dapper filter params, enforcing the export flag to bypass pagination
        var filterParams = new DisputeFilterParams
        {
            SearchTerm = request.SearchTerm,
            Status = request.Status,
            CategoryId = request.CategoryId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            SortColumn = request.SortColumn ?? "SubmittedDate", 
            IsDescending = request.IsDescending,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            // Assuming your DisputeFilterParams has this flag to bypass OFFSET/FETCH
            IsExport = true 
        };

        // 2. Fetch all matching records
        var disputes = await _repository.GetFilteredDisputesAsync(filterParams, ct);

        // 3. Build CSV string
        var csvBuilder = new StringBuilder();
        
        // Headers
        csvBuilder.AppendLine("Id,Category,Status,Bidder Name,Seller Name,Submitted Date");

        // Rows
        if (disputes != null)
        {
            foreach (var dispute in disputes.Items)
            {
                // Escape values with double quotes to prevent commas inside data from breaking columns
                csvBuilder.AppendLine(
                    $"{dispute.Id}," +
                    $"\"{dispute.Category}\"," +
                    $"{dispute.Status}," +
                    $"\"{dispute.BidderName}\"," +
                    $"\"{dispute.SellerName}\"," +
                    $"{dispute.SubmittedDate:yyyy-MM-dd HH:mm:ss}");
            }
        }

        // 4. Convert string to byte array
        var fileContents = Encoding.UTF8.GetBytes(csvBuilder.ToString());
        var fileName = $"Disputes_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
        
        return Result.Success(new ExportFileResponse(fileContents, "text/csv", fileName));
    }
}