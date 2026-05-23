/**
 * Seller feature — public API.
 *
 * Handles seller dashboard, become a seller applications,
 * analytics, and seller-specific operations.
 */

export { BecomeSellerPage } from "./components/BecomeSellerPage";
export { SellerProfilePage } from "./components/SellerProfilePage";
export { SellerReviewsTab } from "./components/SellerReviewsTab";
export { SellerAuctionsTab } from "./components/SellerAuctionsTab";
export { useBecomeSeller } from "./api/seller.mutations";
export {
  useGetSellerProfile,
  useGetSellerReviews,
  useGetSellerProfileAuctions,
  useCreateReviewReply,
} from "./api/seller.queries";
export type { BecomeSellerInput, BecomeSellerResponse } from "./api/seller.mutations";
export type { SellerProfile, SellerReview, ReviewReply } from "./types/seller.types";

