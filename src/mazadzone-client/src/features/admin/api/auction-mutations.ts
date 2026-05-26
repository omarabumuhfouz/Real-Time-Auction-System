import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import { useAppToast } from "@/lib/toast/app-toast";
import { updateMockAuctionStatus } from "./queries";

export interface CancelAuctionParams {
  auctionId: string;
  reason: string;
}

export interface ForceEndAuctionParams {
  auctionId: string;
  reason: string;
}

/**
 * Mutation to cancel an active or pending auction.
 * Hits backend API but falls back to mock status changes if offline.
 */
export function useCancelAuction() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation({
    mutationFn: async ({ auctionId, reason }: CancelAuctionParams) => {
      try {
        const response = await api.post(`/admin/auctions/${auctionId}/cancel`, { reason });
        return response.data;
      } catch (error) {
        console.warn(`Failed to cancel auction ${auctionId} on backend, falling back to mock:`, error);

        // Mock fallback
        await new Promise((resolve) => setTimeout(resolve, 800));
        const success = updateMockAuctionStatus(auctionId, "Cancelled");
        if (!success) {
          throw new Error("Auction not found in mock data");
        }
        return { success: true };
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "moderate-auctions"] });
      appToast.success("Auction Cancelled", "The auction has been successfully cancelled.");
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to cancel the auction. Please try again.";
      appToast.error("Action Failed", msg);
    },
  });
}

/**
 * Mutation to force-end an auction (immediately close bidding).
 * Hits backend API but falls back to mock status changes if offline.
 */
export function useForceEndAuction() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation({
    mutationFn: async ({ auctionId, reason }: ForceEndAuctionParams) => {
      try {
        const response = await api.post(`/admin/auctions/${auctionId}/force-end`, { reason });
        return response.data;
      } catch (error) {
        console.warn(`Failed to force-end auction ${auctionId} on backend, falling back to mock:`, error);

        // Mock fallback
        await new Promise((resolve) => setTimeout(resolve, 800));
        const success = updateMockAuctionStatus(auctionId, "Ended");
        if (!success) {
          throw new Error("Auction not found in mock data");
        }
        return { success: true };
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "moderate-auctions"] });
      appToast.success("Auction Force-Ended", "The auction has been force-ended and bidding is now closed.");
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to force-end the auction. Please try again.";
      appToast.error("Action Failed", msg);
    },
  });
}
