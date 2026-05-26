/**
 * Local contract representations of backend API contracts for the Seller feature.
 */

export interface PublicSellerProfileResponse {
  sellerId: { value?: string };
  rating: number;
  reviewsCount: number;
  isVerified: boolean;
  joinedOnUtc: string;
}

export interface ReplyToFeedbackRequest {
  replyText: string;
}
