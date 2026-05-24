import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { Auction, CreateAuctionInput, UpdateAuctionInput } from "../types/auction.types";
import { auctionKeys } from "./auction.queries";
import { createAuctionApi, updateAuctionApi, deleteAuctionApi } from "./auctions-mutation.api";

/**
 * Creates a new auction.
 * Invalidates the auction list cache on success.
 */
export function useCreateAuction() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (input: CreateAuctionInput) => createAuctionApi(input),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: auctionKeys.lists() });
    },
  });
}

export function useUpdateAuction(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (input: UpdateAuctionInput) => updateAuctionApi(id, input),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: auctionKeys.lists() });
      void queryClient.invalidateQueries({
        queryKey: auctionKeys.detail(id),
      });
    },
  });
}

import type { ApiError, ApiResponse } from "@/types/api.types";
import { useAppToast } from "@/lib/toast/app-toast";

/**
 * Deletes an auction.
 */
export function useDeleteAuction() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation<ApiResponse<void>, ApiError, string>({
    mutationFn: (id: string) => deleteAuctionApi(id),
    onSuccess: (response) => {
      // Invalidate all auction queries to force refetching of tables, stats, and details
      void queryClient.invalidateQueries({ queryKey: auctionKeys.all });
      appToast.success("Success", response.message || "Your auction listing has been successfully removed.");
    },
    onError: (err) => {
      appToast.error("Error", err.message || "Could not delete the auction listing.");
    },
  });
}

