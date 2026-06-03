namespace MazadZone.Application.Features.Disputes.Queries.ExportSelectedDisputes;

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// Update with your actual namespaces
using MazadZone.Application.Features.Users.Queries.ExportUsers;

public class ExportSelectedDisputesQueryHandler : IQueryHandler<ExportSelectedDisputesQuery, ExportFileResponse>
{
    private readonly IDisputeQueries _disputeQueries;

    public ExportSelectedDisputesQueryHandler(IDisputeQueries disputeQueries)
    {
        _disputeQueries = disputeQueries;
    }

    public async Task<Result<ExportFileResponse>> Handle(ExportSelectedDisputesQuery request, CancellationToken ct)
    {
        // 1. Fetch exact matching records from our dedicated Dapper query
        var disputes = await _disputeQueries.ExportSelectedDisputesAsync(request.SelectedDisputeIds, ct);

        // 2. Build CSV string
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Id,Bidder Name,Seller Name,Category,Status,Submitted Date");

        foreach (var dispute in disputes)
        {
            // Escape string fields with quotes to prevent internal commas from breaking the CSV layout
            csvBuilder.AppendLine(
                $"{dispute.Id}," +
                $"\"{dispute.BidderName}\"," +
                $"\"{dispute.SellerName}\"," +
                $"\"{dispute.Category}\"," +
                $"{dispute.Status}," +
                $"{dispute.SubmittedDate:yyyy-MM-dd HH:mm:ss}");
        }

        // 3. Convert to File payload
        var fileContents = Encoding.UTF8.GetBytes(csvBuilder.ToString());
        var fileName = $"Selected_Disputes_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
        
        return new ExportFileResponse(fileContents, "text/csv", fileName);
    }
}