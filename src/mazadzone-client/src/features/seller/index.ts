/**
 * Seller feature — public API.
 *
 * Handles seller dashboard, become a seller applications,
 * analytics, and seller-specific operations.
 */

// Components
export { BecomeSellerPage } from "./components/BecomeSellerPage";
export { SellerReviewsTab } from "./components/SellerReviewsTab";
export { SellerAuctionsTab } from "./components/SellerAuctionsTab";

// Hooks & API
export {
  useBecomeSeller,
  useSubmitSellerReview,
  useGetSellerProfile,
  useGetSellerReviews,
  useGetSellerProfileAuctions,
  useCreateReviewReply,
} from "./api";

// Types
export type { BecomeSellerInput, BecomeSellerResponse, SubmitSellerReviewInput } from "./api";
export type { SellerProfile, SellerReview, ReviewReply } from "./types/seller.types";

// Testing
export { resetMockReviews } from "./testing/mock-seller";
