export const overviewKeys = {
  all: ["admin", "stats"] as const,
  overview: (period: string) => [...overviewKeys.all, "overview", period] as const,
  activity: (timeframe: string) => [...overviewKeys.all, "activity", timeframe] as const,
  growth: (timeframe: string) => [...overviewKeys.all, "growth", timeframe] as const,
};
