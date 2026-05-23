import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchMyBids, placeBid, fetchSavedPaymentMethods } from "./bidding.api";
import type { PlaceBidRequest, PlaceBidResponse, SavedPaymentMethod } from "../types/place-bid.types";

export const BIDDING_KEYS = {
  all: ["bidding"] as const,
  myBids: (
    userId: string,
    params: { filter?: string; sortBy?: string; page?: number; pageSize?: number }
  ) => [...BIDDING_KEYS.all, "my-bids", userId, params] as const,
  savedPaymentMethods: () => [...BIDDING_KEYS.all, "payment-methods"] as const,
};

/**
 * Hook to retrieve user bids dynamically from backend / mock layer.
 */
export const useGetMyBids = (
  userId: string,
  params: { filter?: string; sortBy?: string; page?: number; pageSize?: number } = {}
) => {
  return useQuery({
    queryKey: BIDDING_KEYS.myBids(userId, params),
    queryFn: () => fetchMyBids(userId, params),
    enabled: !!userId,
  });
};

/**
 * Mutation hook to place a bid on an auction.
 * Invalidates both the user's bid list and the auction detail on success.
 */
export const usePlaceBid = () => {
  const queryClient = useQueryClient();

  return useMutation<PlaceBidResponse, Error, PlaceBidRequest>({
    mutationFn: placeBid,
    onSuccess: (_data, variables) => {
      // Invalidate relevant queries after a successful bid
      void queryClient.invalidateQueries({ queryKey: BIDDING_KEYS.all });
      void queryClient.invalidateQueries({
        queryKey: ["auctions", variables.auctionId],
      });
    },
  });
};

/**
 * Hook to retrieve the user's saved payment methods.
 */
export const useGetSavedPaymentMethods = () => {
  return useQuery<SavedPaymentMethod[]>({
    queryKey: BIDDING_KEYS.savedPaymentMethods(),
    queryFn: fetchSavedPaymentMethods,
  });
};
