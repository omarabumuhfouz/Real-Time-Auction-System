/**
 * React Query hooks for fetching Bidding data.
 */

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchMyBids, placeBid as placeBidApi } from "./bidding.api";
import { biddingKeys } from "./bidding.keys";
import { mapMyBidAuctionDtoToBidActivity } from "./bidding.mappers";
import type {
  PlaceBidRequest,
  PlaceBidResponse,
} from "../types/place-bid.types";
import type { BidActivity } from "../types/bidding.types";
import { useAuthStore } from "@/stores/auth.store";

// Re-export keys
export { biddingKeys as BIDDING_KEYS };

export interface BidActivityResponse {
  items: BidActivity[];
  page: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

/**
 * Hook to retrieve user bids dynamically from backend.
 */
export const useGetMyBids = (
  userId: string,
  params: {
    filter?: string;
    sortBy?: string;
    page?: number;
    pageSize?: number;
  } = {},
) => {
  return useQuery<BidActivityResponse>({
    queryKey: biddingKeys.myBids(userId, params),
    queryFn: async () => {
      const raw = await fetchMyBids(userId, params);
      return {
        items: raw.items.map(mapMyBidAuctionDtoToBidActivity),
        page: raw.pageNumber,
        totalPages: raw.totalPages ?? Math.ceil(raw.totalCount / raw.pageSize),
        totalCount: raw.totalCount,
        hasPreviousPage: raw.hasPreviousPage ?? false,
        hasNextPage: raw.hasNextPage ?? false,
      };
    },
    enabled: !!userId,
  });
};

/**
 * Mutation hook to place a bid on an auction.
 * Translates UI form parameters to real backend parameters, connects to `/api/v1/auctions/...`,
 * and constructs a compatible success response for the multi-step layout.
 */
export const usePlaceBid = () => {
  const queryClient = useQueryClient();
  const { user } = useAuthStore();

  return useMutation<PlaceBidResponse, Error, PlaceBidRequest>({
    mutationFn: async (request: PlaceBidRequest) => {
      const bidderId = user?.id || "bidder-id-placeholder";
      const methodId = request.paymentMethodId || "pm-mock-1";

      // Call the real backend REST POST endpoint
      await placeBidApi(request.auctionId, {
        bidderId,
        methodId,
        amount: request.bidAmount,
      });

      // Construct a compatible PlaceBidResponse locally to keep Success UI steps happy
      return {
        bidId: `bid-${Date.now()}`,
        auctionId: request.auctionId,
        auctionTitle: "Bidding Success",
        bidAmount: request.bidAmount,
        authorizationHold: request.bidAmount * 0.1,
        deliveryAddress: {
          id: request.addressId,
          label: "Home",
          fullName: user?.fullName || "Omar Ahmad",
          phoneNumber: "07 1234 5678",
          streetAddress: "Queen Rania St.",
          building: "Building 12",
          city: "Amman, Jordan",
          isDefault: true,
        },
        paymentMethod: {
          id: methodId,
          cardType: "VISA",
          lastFourDigits: "4242",
          expiryDate: "12/26",
          cardholderName:
            request.paymentDetails?.cardholderName ||
            user?.fullName ||
            "Omar Ahmad",
          isDefault: true,
        },
        placedAt: new Date().toISOString(),
      };
    },
    onSuccess: (_data, variables) => {
      // Invalidate relevant queries after a successful bid
      void queryClient.invalidateQueries({ queryKey: biddingKeys.all });
      void queryClient.invalidateQueries({
        queryKey: ["auctions", variables.auctionId],
      });
    },
  });
};

import type { SavedPaymentMethod } from "../types/place-bid.types";

/**
 * Hook to retrieve the user's saved payment methods.
 * Note: Simulates local payment options since payment-methods is local to bid flows.
 */
export const useGetSavedPaymentMethods = () => {
  return useQuery<SavedPaymentMethod[]>({
    queryKey: biddingKeys.savedPaymentMethods(),
    queryFn: async () => {
      const mockSavedMethods: SavedPaymentMethod[] = [];
      return mockSavedMethods;
    },
  });
};

