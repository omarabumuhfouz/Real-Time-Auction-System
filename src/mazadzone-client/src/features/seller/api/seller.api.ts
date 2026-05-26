import { api } from "@/lib/api/client";
import type { SellerProfile, SellerReview, ReviewReply } from "../types/seller.types";
import type { AuctionSummary } from "@/features/auctions";
import type { PaginatedResult } from "@/types/api.types";
import { getMockSellerReviews, addMockReviewReply } from "../testing/mock-seller";
import type { PublicSellerProfileResponse } from "./seller.contracts";

interface BidderProfileDto {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  totalBidsPlaced: number;
}

/**
 * Fetches the public seller profile information by combining the public seller stats
 * and their bidder profile details.
 */
export async function fetchSellerProfile(id: string): Promise<SellerProfile> {
  const [sellerRes, bidderRes] = await Promise.all([
    api.get<PublicSellerProfileResponse>(`/api/v1/sellers/${id}/public`),
    api.get<BidderProfileDto>(`/api/v1/bidders/${id}`),
  ]);

  const seller = sellerRes.data;
  const bidder = bidderRes.data;

  return {
    id: bidder.id,
    fullName: bidder.fullName,
    email: bidder.email,
    role: "seller",
    avatarUrl: null,
    avatarInitial: bidder.fullName ? bidder.fullName.charAt(0).toUpperCase() : "S",
    isVerified: seller.isVerified,
    rating: seller.rating,
    reviewsCount: seller.reviewsCount,
    memberSince: new Date(seller.joinedOnUtc).toLocaleDateString("en-US", { year: "numeric", month: "short" }),
    salesCount: bidder.totalBidsPlaced || 0,
    bio: `Active MazadZone registered seller since ${new Date(seller.joinedOnUtc).toLocaleDateString()}.`,
  };
}

/**
 * Fetches the paginated reviews list of a specific seller.
 * Falls back to local presentational reviews since there is no bulk feedback GET endpoint in the schema.
 */
export async function fetchSellerReviews(
  id: string,
  page: number,
  pageSize: number
): Promise<PaginatedResult<SellerReview>> {
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
 * Fetches the paginated auctions list owned by a specific seller using real REST query filters.
 */
export async function fetchSellerAuctions(
  id: string,
  page: number,
  pageSize: number
): Promise<PaginatedResult<AuctionSummary>> {
  const response = await api.get<any>("/api/v1/auctions", {
    params: { SellerId: id, PageNumber: page, PageSize: pageSize },
  });

  const pagedList = response.data;
  const items = (pagedList.items || []).map((item: any) => ({
    id: item.id?.value || item.id || "",
    title: item.title || "",
    description: item.description || "",
    category: item.categoryName || "",
    currentBid: item.currentBidAmount || item.startingPrice || 0,
    startingPrice: item.startingPrice || 0,
    endTime: item.endTime || "",
    status: item.status === 1 ? "Active" : item.status === 2 ? "Ended" : "Pending",
    image: item.images?.[0]?.url || item.imageUrl || "/placeholder-auction.jpg",
    isOwner: true,
  }));

  return {
    items,
    page: pagedList.pageNumber || page,
    pageSize: pagedList.pageSize || pageSize,
    totalCount: pagedList.totalCount || items.length,
    totalPages: pagedList.totalPages || 1,
    hasNextPage: pagedList.hasNextPage || false,
    hasPreviousPage: pagedList.hasPreviousPage || false,
  };
}

/**
 * Submits a seller reply to a specific review left on a completed order.
 */
export async function submitReviewReply(
  sellerId: string,
  reviewId: string,
  comment: string
): Promise<ReviewReply> {
  // Fire real POST request to reply to the order's feedback
  await api.post(`/api/v1/orders/api/orders/${reviewId}/feedback/reply`, {
    replyText: comment,
  });

  // Local fallback response to sync the UI state seamlessly
  const localReply = addMockReviewReply(reviewId, comment);
  if (localReply) {
    return localReply;
  }

  return {
    comment,
    createdAt: "Just now",
  };
}
