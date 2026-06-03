namespace MazadZone.Application.Features.Disputes.Queries.GetOpenDisputesBreakdown;

public record DisputeTypeBreakdownDto(
    string TypeName, 
    int Cases, 
    decimal PercentageChange
);

public record OpenDisputesBreakdownDto(
    int TotalOpenCases,
    List<DisputeTypeBreakdownDto> Breakdown
);