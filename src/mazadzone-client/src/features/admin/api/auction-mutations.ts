import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import { useNotificationStore } from "@/stores/notification.store";
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
  const addNotification = useNotificationStore((state) => state.addNotification);

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
      addNotification({
        type: "success",
        title: "Auction Cancelled",
        message: "The auction has been successfully cancelled.",
        duration: 4000,
      });
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to cancel the auction. Please try again.";
      addNotification({
        type: "error",
        title: "Action Failed",
        message: msg,
        duration: 5000,
      });
    },
  });
}

/**
 * Mutation to force-end an auction (immediately close bidding).
 * Hits backend API but falls back to mock status changes if offline.
 */
export function useForceEndAuction() {
  const queryClient = useQueryClient();
  const addNotification = useNotificationStore((state) => state.addNotification);

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
      addNotification({
        type: "success",
        title: "Auction Force-Ended",
        message: "The auction has been force-ended and bidding is now closed.",
        duration: 4000,
      });
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to force-end the auction. Please try again.";
      addNotification({
        type: "error",
        title: "Action Failed",
        message: msg,
        duration: 5000,
      });
    },
  });
}
