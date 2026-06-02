export const auctionsKeys = {
  all: ["admin", "moderate-auctions"] as const,
  list: (filters: any) => [...auctionsKeys.all, filters] as const,
};
