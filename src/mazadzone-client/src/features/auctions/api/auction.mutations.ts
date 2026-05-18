import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { Auction, CreateAuctionInput, UpdateAuctionInput } from "../types/auction.types";
import { auctionKeys } from "./auction.queries";
import { createAuctionApi, updateAuctionApi } from "./auctions.api";

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
