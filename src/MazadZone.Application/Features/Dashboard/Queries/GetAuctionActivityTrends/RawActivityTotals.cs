namespace MazadZone.Application.Features.Dashboard.Queries.GetAuctionActivityTrends;

public record RawActivityTotals(
    int CurrTotalAuctions, int PrevTotalAuctions,
    int CurrTotalBids, int PrevTotalBids
);

public record RawDailyActivity(
    DateTime DatePoint,
    int Count
);

public record AuctionActivityDataResult(
    RawActivityTotals Totals,
    IReadOnlyList<RawDailyActivity> DailyAuctions,
    IReadOnlyList<RawDailyActivity> DailyBids
);
