import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import type { Auction, CreateAuctionInput, UpdateAuctionInput } from "../types/auction.types";
import { auctionKeys } from "./auction.queries";

/**
 * Creates a new auction.
 * Invalidates the auction list cache on success.
 */
export function useCreateAuction() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (input: CreateAuctionInput) => {
      const formData = new FormData();
      formData.append("title", input.title);
      formData.append("description", input.description);
      formData.append("category", input.category);
      formData.append("subcategory", input.subcategory);
      formData.append("condition", input.condition);
      if (input.conditionDescription) {
        formData.append("conditionDescription", input.conditionDescription);
      }
      formData.append("startingPrice", input.startingPrice.toString());
      formData.append("minimumIncrement", input.minimumIncrement.toString());
      formData.append("shippingLocation", input.shippingLocation);
      formData.append("startDate", input.startDate);
      formData.append("endDate", input.endDate);

      input.images.forEach((image) => {
        formData.append("images", image);
      });

      return api.post<Auction>("/auctions", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: auctionKeys.lists() });
    },
  });
}

/**
 * Updates an existing auction.
 * Invalidates both the list cache and the specific detail cache on success.
 */
export function useUpdateAuction(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (input: UpdateAuctionInput) =>
      api.patch<Auction>(`/auctions/${id}`, input),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: auctionKeys.lists() });
      void queryClient.invalidateQueries({
        queryKey: auctionKeys.detail(id),
      });
    },
  });
}
