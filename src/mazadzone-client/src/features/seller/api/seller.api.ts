import { api } from "@/lib/api/client";
import type { SellerProfile, SellerReview, ReviewReply } from "../types/seller.types";
import type { AuctionSummary } from "@/features/auctions";
import type { PaginatedResult } from "@/types/api.types";
import { getMockSellerProfile, getMockSellerReviews, addMockReviewReply } from "../testing/mock-seller";
import { getMockAuctions } from "@/features/auctions/testing/mock-auctions";

const MOCK_DELAY_MS = 300;
const simulateDelay = () => new Promise((resolve) => setTimeout(resolve, MOCK_DELAY_MS));

/**
 * Fetches the public seller profile information from the backend.
 */
export async function fetchSellerProfile(id: string): Promise<SellerProfile> {
  await simulateDelay();

  /*
  // --- REAL API CALL (Uncomment when backend is ready) ---
  const response = await api.get<SellerProfile>(`/sellers/${id}`);
  return response.data;
  */

  const profile = getMockSellerProfile(id);
  if (!profile) {
    throw new Error("Seller profile not found");
  }
  return profile;
}

/**
 * Fetches the paginated reviews list of a specific seller.
 */
export async function fetchSellerReviews(
  id: string,
  page: number,
  pageSize: number
): Promise<PaginatedResult<SellerReview>> {
  await simulateDelay();

  /*
  // --- REAL API CALL (Uncomment when backend is ready) ---
  const response = await api.get<PaginatedResult<SellerReview>>(`/sellers/${id}/reviews`, {
    params: { page, pageSize },
  });
  return response.data;
  */

  const allReviews = getMockSellerReviews(id);
  const startIndex = (page - 1) * pageSize;
  const paginatedReviews = allReviews.slice(startIndex, startIndex + pageSize);
  const totalPages = Math.ceil(allReviews.length / pageSize);

  return {
    items: paginatedReviews,
    page,
    pageSize,
    totalCount: allReviews.length,
    totalPages,
    hasNextPage: page < totalPages,
    hasPreviousPage: page > 1,
  };
}

/**
 * Fetches the paginated auctions list owned by a specific seller.
 */
export async function fetchSellerAuctions(
  id: string,
  page: number,
  pageSize: number
): Promise<PaginatedResult<AuctionSummary>> {
  await simulateDelay();

  /*
  // --- REAL API CALL (Uncomment when backend is ready) ---
  const response = await api.get<PaginatedResult<AuctionSummary>>(`/sellers/${id}/auctions`, {
    params: { page, pageSize },
  });
  return response.data;
  */

  const allAuctions = getMockAuctions();
  let sellerAuctions: AuctionSummary[] = [];
  if (id === "seller-123" || id === "1") {
    // Mocking active auctions owned by the test seller
    sellerAuctions = allAuctions.slice(0, 8).map((a) => ({
      ...a,
      isOwner: true,
    }));
  }

  const startIndex = (page - 1) * pageSize;
  const paginatedAuctions = sellerAuctions.slice(startIndex, startIndex + pageSize);
  const totalPages = Math.ceil(sellerAuctions.length / pageSize);

  return {
    items: paginatedAuctions,
    page,
    pageSize,
    totalCount: sellerAuctions.length,
    totalPages,
    hasNextPage: page < totalPages,
    hasPreviousPage: page > 1,
  };
}

/**
 * Submits a seller reply to a specific review.
 */
export async function submitReviewReply(
  sellerId: string,
  reviewId: string,
  comment: string
): Promise<ReviewReply> {
  await simulateDelay();

  /*
  // --- REAL API CALL (Uncomment when backend is ready) ---
  const response = await api.post<ReviewReply>(`/sellers/${sellerId}/reviews/${reviewId}/reply`, {
    comment,
  });
  return response.data;
  */

  const reply = addMockReviewReply(reviewId, comment);
  if (!reply) {
    throw new Error("Review not found");
  }
  return reply;
}
