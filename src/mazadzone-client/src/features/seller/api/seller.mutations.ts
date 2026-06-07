import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import { type PayoutDetails } from "@/features/payment";
import { useAuthStore } from "@/stores/auth.store";

export interface BecomeSellerInput {
  payoutDetails: PayoutDetails;
}

export interface BecomeSellerResponse {
  token: string;
  refreshToken: string;
}

/**
 * Mutation to register a user as a seller on the platform.
 * Transmits verified account profile and bank account details directly to the ASP.NET Core backend.
 */
export function useBecomeSeller() {
  const queryClient = useQueryClient();

  return useMutation<BecomeSellerResponse, Error, BecomeSellerInput>({
    mutationFn: async (input: BecomeSellerInput) => {
      const userId = useAuthStore.getState().user?.id;
      if (!userId) {
        throw new Error("User is not authenticated");
      }

      // Map Stripe credit card inputs to the backend's expected bankAccountNumber property
      const bankAccountNumber =
        input.payoutDetails.lastFourDigits ||
        input.payoutDetails.cardNumber?.replace(/\D/g, "") ||
        "123456789";

      const response = await api.post<BecomeSellerResponse>(`/api/v1/sellers/${userId}/become-seller`, {
        bankAccountNumber,
        paymentMethodId: input.payoutDetails.paymentMethodId,
      });
      return response.data;
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

  return useMutation<void, Error, SubmitSellerReviewInput>({
    mutationFn: async (input: SubmitSellerReviewInput) => {
      // Fire real POST to submit feedback for the delivered order
      await api.post(`/api/v1/orders/${input.orderId}/feedback`, {
        rating: input.rating,
        comment: input.comment,
      });
    },
    onSuccess: () => {
      // Invalidate target seller queries
      void queryClient.invalidateQueries({ queryKey: ["seller", "reviews", sellerId] });
      void queryClient.invalidateQueries({ queryKey: ["public-profile", sellerId] });
    },
  });
}
