namespace MazadZone.Application.Features.Disputes.Queries.GetOpenDisputesBreakdown;

public record GetOpenDisputesBreakdownQuery(DateTime StartDate, DateTime EndDate, int Limit, bool IncludeOther) : IQuery<OpenDisputesBreakdownDto>;