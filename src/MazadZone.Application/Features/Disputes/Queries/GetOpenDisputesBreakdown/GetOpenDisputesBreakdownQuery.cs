namespace MazadZone.Application.Features.Disputes.Queries.GetOpenDisputesBreakdown;

public record GetOpenDisputesBreakdownQuery(DateTime StartDate, DateTime EndDate) : IQuery<OpenDisputesBreakdownDto>;