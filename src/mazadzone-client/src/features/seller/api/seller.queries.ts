import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type { SellerProfile, SellerReview, ReviewReply } from "../types/seller.types";
import type { AuctionSummary } from "@/features/auctions";
import type { PaginatedResult } from "@/types/api.types";
import {
  fetchSellerProfile,
  fetchSellerReviews,
  fetchSellerAuctions,
  submitReviewReply,
} from "./seller.api";

export const sellerKeys = {
  all: ["seller"] as const,
  profile: (id: string) => [...sellerKeys.all, "profile", id] as const,
  reviews: (id: string) => [...sellerKeys.all, "reviews", id] as const,
  auctions: (id: string) => [...sellerKeys.all, "auctions", id] as const,
};

/**
 * Hook to retrieve a seller's profile information.
 */
export function useGetSellerProfile(sellerId: string) {
  return useQuery<SellerProfile | undefined>({
    queryKey: sellerKeys.profile(sellerId),
    queryFn: () => fetchSellerProfile(sellerId),
    enabled: !!sellerId,
  });
}

/**
 * Hook to retrieve reviews for a specific seller (with pagination).
 */
export function useGetSellerReviews(sellerId: string, page: number = 1, pageSize: number = 6) {
  return useQuery<PaginatedResult<SellerReview>>({
    queryKey: [...sellerKeys.reviews(sellerId), page, pageSize],
    queryFn: () => fetchSellerReviews(sellerId, page, pageSize),
    enabled: !!sellerId,
  });
}

/**
 * Hook to retrieve auctions belonging to a specific seller (with pagination).
 */
export function useGetSellerProfileAuctions(sellerId: string, page: number = 1, pageSize: number = 6) {
  return useQuery<PaginatedResult<AuctionSummary>>({
    queryKey: [...sellerKeys.auctions(sellerId), page, pageSize],
    queryFn: () => fetchSellerAuctions(sellerId, page, pageSize),
    enabled: !!sellerId,
  });
}

interface CreateReplyInput {
  reviewId: string;
  comment: string;
}

/**
 * Mutation to submit a reply to a user review.
 */
export function useCreateReviewReply(sellerId: string) {
  const queryClient = useQueryClient();

  return useMutation<ReviewReply, Error, CreateReplyInput>({
    mutationFn: ({ reviewId, comment }) =>
      submitReviewReply(sellerId, reviewId, comment),
    onSuccess: () => {
      void queryClient.invalidateQueries({
        queryKey: sellerKeys.reviews(sellerId),
      });
    },
  });
}
