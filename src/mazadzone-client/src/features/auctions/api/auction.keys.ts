import type { AuctionFilters } from "../types/auction.types";

/**
 * Stable, predictable TanStack Query keys for the Auctions feature.
 */
export const auctionKeys = {
  all: ["auctions"] as const,
  lists: () => [...auctionKeys.all, "list"] as const,
  list: (filters?: AuctionFilters) =>
    [...auctionKeys.lists(), filters ?? {}] as const,
  details: () => [...auctionKeys.all, "detail"] as const,
  detail: (id: string) => [...auctionKeys.details(), id] as const,
  similar: (id: string, limit?: number) =>
    [...auctionKeys.all, "similar", id, { limit }] as const,
};
