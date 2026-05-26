/**
 * Stable, predictable TanStack Query keys for the Bidding feature.
 */
export const biddingKeys = {
  all: ["bidding"] as const,
  myBids: (userId: string, filters: Record<string, unknown>) =>
    [...biddingKeys.all, "my-bids", userId, filters] as const,
  savedPaymentMethods: () => [...biddingKeys.all, "payment-methods"] as const,
};
