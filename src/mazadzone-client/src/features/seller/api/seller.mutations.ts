import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { type PayoutDetails } from "@/features/payment";
import { type ApiResponse } from "@/types/api.types";

export interface BecomeSellerInput {
  payoutDetails: PayoutDetails;
}

export interface BecomeSellerResponse {
  success: boolean;
  message: string;
  sellerProfileId?: string;
  role: string;
}

/**
 * Mutation to register a user as a seller on the platform.
 * Transmits verified account profile and selected payout method details (card or bank transfer) to the server.
 */
export function useBecomeSeller() {
  const queryClient = useQueryClient();

  return useMutation<ApiResponse<BecomeSellerResponse>, Error, BecomeSellerInput>({
    mutationFn: (input: BecomeSellerInput) => {
      return api.post<BecomeSellerResponse>("/seller/register", input);
    },
    onSuccess: () => {
      // Invalidate core auth/profile queries on success
      void queryClient.invalidateQueries({ queryKey: ["auth", "user"] });
      void queryClient.invalidateQueries({ queryKey: ["user", "profile"] });
    },
  });
}

export interface SubmitSellerReviewInput {
  reviewerId: string;
  sellerId: string;
  rating: number;
  comment: string;
  orderId: string;
}

/**
 * Mutation to submit a public review for a seller based on a delivered order.
 */
export function useSubmitSellerReview(sellerId: string) {
  const queryClient = useQueryClient();

  return useMutation<ApiResponse<{ success: boolean }>, Error, SubmitSellerReviewInput>({
    mutationFn: async (input: SubmitSellerReviewInput) => {
      /*
      // --- REAL API CALL (Uncomment when backend is ready) ---
      const response = await api.post(`/sellers/${input.sellerId}/reviews`, input);
      return response;
      */
      
      // Mock simulation:
      await new Promise((resolve) => setTimeout(resolve, 800));
      return {
        data: { success: true },
        message: "Review submitted successfully",
        success: true,
        timestamp: new Date().toISOString(),
      };
    },
    onSuccess: () => {
      // Invalidate target seller queries
      void queryClient.invalidateQueries({ queryKey: ["seller", "reviews", sellerId] });
      void queryClient.invalidateQueries({ queryKey: ["public-profile", sellerId] });
    },
  });
}

