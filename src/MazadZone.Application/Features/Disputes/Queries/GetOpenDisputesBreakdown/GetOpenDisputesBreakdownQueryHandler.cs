namespace MazadZone.Application.Features.Disputes.Queries.GetOpenDisputesBreakdown;

public class GetOpenDisputesBreakdownQueryHandler : IQueryHandler<GetOpenDisputesBreakdownQuery, OpenDisputesBreakdownDto>
{
    private readonly IDisputeQueries _disputeQueries;

    public GetOpenDisputesBreakdownQueryHandler(IDisputeQueries disputeQueries)
    {
        _disputeQueries = disputeQueries;
    }

    public async Task<Result<OpenDisputesBreakdownDto>> Handle(GetOpenDisputesBreakdownQuery request, CancellationToken ct)
    {
        var duration = request.EndDate - request.StartDate;
        var prevStartDate = request.StartDate - duration;
        var prevEndDate = request.StartDate;

        var rawData = await _disputeQueries.GetOpenDisputesBreakdownAsync(
            request.StartDate, request.EndDate, 
            prevStartDate, prevEndDate, 
            ct);

        int totalOpenCases = 0;
        var breakdownList = new List<DisputeTypeBreakdownDto>();

        foreach (var row in rawData)
        {
            totalOpenCases += row.CurrentCases;

            decimal percentageChange = 0;
            if (row.PreviousCases == 0)
            {
                percentageChange = row.CurrentCases > 0 ? 100.0m : 0.0m;
            }
            else
            {
                percentageChange = Math.Round(((decimal)(row.CurrentCases - row.PreviousCases) / row.PreviousCases) * 100, 1);
            }

            breakdownList.Add(new DisputeTypeBreakdownDto(row.DisputeTypeName, row.CurrentCases, percentageChange));
        }

        return new OpenDisputesBreakdownDto(totalOpenCases, breakdownList);
    }
}