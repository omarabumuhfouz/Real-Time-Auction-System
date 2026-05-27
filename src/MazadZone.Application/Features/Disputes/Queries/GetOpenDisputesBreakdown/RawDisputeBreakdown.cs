namespace MazadZone.Application.Features.Disputes.Queries.GetOpenDisputesBreakdown;

public record RawDisputeBreakdown(
    string DisputeTypeName, 
    int CurrentCases, 
    int PreviousCases
);