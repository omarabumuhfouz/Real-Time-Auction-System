import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useAppToast } from "@/lib/toast/app-toast";
import {
  fetchModerateAuctions,
  cancelAuctionByAdminApi,
  type UseModerateAuctionsFilters,
} from "./auctions.api";
import { auctionsKeys } from "./auctions.keys";

/**
 * Hook to query backend moderate auctions based on search parameters, filters, and page index.
 */
export function useModerateAuctions(filters: UseModerateAuctionsFilters) {
  return useQuery({
    queryKey: auctionsKeys.list(filters),
    queryFn: () => fetchModerateAuctions(filters),
    staleTime: 30 * 1000,
  });
}

export interface CancelAuctionParams {
  auctionId: string;
  reason: string;
}

/**
 * Hook to execute force cancellation of an auction.
 */
export function useCancelAuction() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation({
    mutationFn: ({ auctionId, reason }: CancelAuctionParams) =>
      cancelAuctionByAdminApi(auctionId, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: auctionsKeys.all });
      appToast.success("Auction Cancelled", "The auction has been successfully cancelled.");
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to cancel the auction. Please try again.";
      appToast.error("Action Failed", msg);
    },
  });
}
