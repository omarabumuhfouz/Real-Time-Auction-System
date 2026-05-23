import { api } from "@/lib/api/client";
import { BidActivity, BidStatus } from "../types/bidding.types";
import {
  PlaceBidRequest,
  PlaceBidResponse,
  SavedPaymentMethod,
  DeliveryAddress,
} from "../types/place-bid.types";
import { getMockAuctions } from "@/features/auctions/testing/mock-auctions";

/**
 * Fetches the user's active/ended bid activities.
 * Backed by mock data during development and structured to easily swap
 * with the real backend API endpoint by uncommenting the block below.
 */
export interface BidActivityResponse {
  items: BidActivity[];
  page: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

/**
 * Fetches the user's active/ended bid activities.
 * Backed by mock data during development and structured to easily swap
 * with the real backend API endpoint by uncommenting the block below.
 */
export async function fetchMyBids(
  userId: string,
  params: { filter?: string; sortBy?: string; page?: number; pageSize?: number } = {}
): Promise<BidActivityResponse> {
  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * const { data } = await api.get<BidActivityResponse>("/bids/my-bids", {
   *   params: { userId, ...params }
   * });
   * return data;
   */

  // --- MOCK IMPLEMENTATION ---
  await new Promise((resolve) => setTimeout(resolve, 500)); // Simulate networking delay

  const mockAuctions = getMockAuctions().slice(0, 15);
  const allBids: BidActivity[] = mockAuctions.map((auction, idx) => {
    let status: BidStatus = "Leading";
    const mod = idx % 4;
    if (mod === 0) {
      status = "Leading";
    } else if (mod === 1) {
      status = "Outbid";
    } else if (mod === 2) {
      status = "Won";
    } else {
      status = "Lost";
    }

    const currentBid = auction.pricing.currentBid || auction.pricing.startingPrice;
    const yourBid = (status === "Leading" || status === "Won") ? currentBid : currentBid - 75;

    return {
      id: `bid-${auction.id}`,
      auction,
      yourBid,
      status,
    };
  });

  // Filter
  const filter = params.filter || "All";
  const filteredBids = allBids.filter((bid) => {
    if (filter === "All") return true;
    return bid.status === filter;
  });

  // Sort
  const sortBy = params.sortBy || "latest";
  const sortedBids = [...filteredBids].sort((a, b) => {
    const timeA = new Date(a.auction.timing.endDate).getTime();
    const timeB = new Date(b.auction.timing.endDate).getTime();
    if (sortBy === "latest") {
      return timeA - timeB;
    }
    if (sortBy === "oldest") {
      return timeB - timeA;
    }
    return 0;
  });

  // Paginate
  const page = params.page || 1;
  const pageSize = params.pageSize || 5;
  const totalCount = sortedBids.length;
  const totalPages = Math.ceil(totalCount / pageSize);
  const offset = (page - 1) * pageSize;
  const items = sortedBids.slice(offset, offset + pageSize);

  return {
    items,
    page,
    totalPages,
    totalCount,
    hasPreviousPage: page > 1,
    hasNextPage: page < totalPages,
  };
}

// ---------------------------------------------------------------------------
// Place Bid
// ---------------------------------------------------------------------------

/**
 * Places a bid on an auction with address and payment details.
 *
 * @param request - Bid placement request containing auction ID, bid amount,
 *                  delivery address, and payment information.
 * @returns The bid placement response with confirmation details.
 */
export async function placeBid(
  request: PlaceBidRequest
): Promise<PlaceBidResponse> {
  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * const { data } = await api.post<PlaceBidResponse>("/bids", request);
   * return data;
   */

  // --- MOCK IMPLEMENTATION ---
  await new Promise((resolve) => setTimeout(resolve, 1200));

  return {
    bidId: `bid-${Date.now()}`,
    auctionId: request.auctionId,
    auctionTitle: "Auction Item",
    bidAmount: request.bidAmount,
    authorizationHold: request.bidAmount * 0.1,
    deliveryAddress: {
      id: request.addressId,
      label: "Home",
      fullName: "Omar Ahmad",
      phoneNumber: "07 1234 5678",
      streetAddress: "Queen Rania St.",
      building: "Building 12",
      city: "Amman, Jordan",
      isDefault: true,
    },
    paymentMethod: {
      id: "pm-mock-1",
      cardType: "VISA",
      lastFourDigits: "4242",
      expiryDate: "12/26",
      cardholderName: request.paymentDetails?.cardholderName || "Omar Ahmad",
      isDefault: true,
    },
    placedAt: new Date().toISOString(),
  };
}

// ---------------------------------------------------------------------------
// Saved payment methods
// ---------------------------------------------------------------------------

/**
 * Fetches the user's saved payment methods.
 *
 * @returns Array of saved payment methods.
 */
export async function fetchSavedPaymentMethods(): Promise<SavedPaymentMethod[]> {
  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * const { data } = await api.get<SavedPaymentMethod[]>("/payment-methods");
   * return data;
   */

  // --- MOCK IMPLEMENTATION ---
  await new Promise((resolve) => setTimeout(resolve, 300));

  // Return empty array to simulate no saved payment methods (user must add one)
  return [];
}
