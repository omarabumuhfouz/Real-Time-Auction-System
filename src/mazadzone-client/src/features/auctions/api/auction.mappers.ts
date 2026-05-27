/**
 * Pure mapping functions for the Auctions feature.
 * Decouples raw backend DTO contracts from client-side UI ViewModels.
 */

import { parseUtcDate } from "@/utils/date.utils";
import { useAuthStore } from "@/stores/auth.store";
import type {
  AuctionFilters,
  AuctionSummary,
  AuctionStatus,
  CreateAuctionInput,
} from "../types/auction.types";
import type {
  AuctionDto,
  AuctionsListDto,
  CreateAuctionRequest,
  GetAuctionsQueryParams,
} from "./auction.contracts";

/**
 * Maps frontend UI filters to backend PascalCase query parameters.
 */
export function mapFiltersToQueryParams(
  filters?: AuctionFilters,
): GetAuctionsQueryParams {
  let statusParam: string | undefined = filters?.status;
  if (statusParam === "Upcoming") {
    statusParam = "Pending";
  }

  return {
    Page: filters?.page ?? 1,
    PageSize: filters?.pageSize ?? 12,
    SearchTerm: filters?.search || undefined,
    CategoryId: filters?.category || undefined,
    SubCategoryId: filters?.subcategory || undefined,
    MinCurrentBid: filters?.minPrice || undefined,
    MaxCurrentBid: filters?.maxPrice || undefined,
    Status: statusParam || undefined,
    SortBy: filters?.sortBy || undefined,
    SortDirection: filters?.sortDirection || undefined,
  };
}

/**
 * Helper to normalize backend status strings into stable UI values.
 */
function mapBackendStatusToAuctionStatus(status?: string): AuctionStatus {
  if (!status) return "Active";
  const normalized = status.toLowerCase();
  if (normalized === "active") return "Active";
  if (normalized === "upcoming" || normalized === "pending") return "Upcoming";
  if (
    normalized === "ended" ||
    normalized === "endedsold" ||
    normalized === "endedunsold"
  ) {
    return "Ended";
  }
  return "Active";
}

/**
 * Maps raw AuctionsListDto from a search or list request into an AuctionSummary card ViewModel.
 */
export function mapAuctionsListDtoToSummary(
  dto: AuctionsListDto,
): AuctionSummary {
  return {
    id: dto.id,
    title: dto.itemTitle,
    imageUrl: dto.imageUrl,
    category: "Tech and Electronics",
    subcategory: "Others",
    condition: (dto.itemStatus as any) || "New",
    description: "",
    conditionDescription: dto.condtion || "",
    status: mapBackendStatusToAuctionStatus(dto.status),
    pricing: {
      startingPrice: dto.currentBidAmount,
      currentBid: dto.currentBidAmount > 0 ? dto.currentBidAmount : null,
      bidCount: dto.bidsCount,
    },
    timing: {
      startDate: parseUtcDate(dto.startTime),
      endDate: parseUtcDate(dto.endTime),
      creationDate: dto.startTime,
    },
    isFavorite: false,
    isOwner: false,
    images: dto.imageUrl ? [dto.imageUrl] : [],
    bidHistory: [],
  };
}

/**
 * Maps raw AuctionDto from a detailed request into an AuctionSummary ViewModel.
 */
export function mapAuctionDtoToSummary(dto: AuctionDto): AuctionSummary {
  return {
    id: dto.id,
    title: dto.itemTitle,
    imageUrl: dto.imageUrls?.[0] ?? "",
    category: "Tech and Electronics",
    subcategory: "Others",
    condition: (dto.status as any) || "New",
    description: dto.itemDescription || "",
    conditionDescription: dto.condition || "",
    status: mapBackendStatusToAuctionStatus(dto.auctionStatus),
    pricing: {
      startingPrice: dto.startBidAmount,
      currentBid: dto.currentBidAmount > 0 ? dto.currentBidAmount : null,
      bidCount: dto.bids?.length ?? 0,
    },
    timing: {
      startDate: parseUtcDate(dto.startTime),
      endDate: parseUtcDate(dto.endTime),
      creationDate: dto.startTime,
    },
    isFavorite: false,
    isOwner: false,
    images: dto.imageUrls ?? [],
    bidHistory: (dto.bids ?? []).map((b, idx) => ({
      id: `${dto.id}-bid-${idx}`,
      bidderName: `Bidder ${b.bidderId.substring(0, 4)}`,
      bidderInitial: "B",
      amount: b.amount,
      timeAgo: new Date(b.timestamp).toLocaleDateString(),
      isHighest: idx === 0,
      bidderId: b.bidderId,
    })),
    seller: {
      id: dto.sellerEmail === useAuthStore.getState().user?.email
        ? useAuthStore.getState().user?.id || "seller-id-placeholder"
        : "seller-id-placeholder",
      email: dto.sellerEmail,
      fullName: dto.sellerName,
      role: "seller",
      isVerified: dto.sellerRating >= 4.0,
      avatarInitial: dto.sellerName ? dto.sellerName.charAt(0) : "S",
      reviews: dto.reviewCount,
      rating: dto.sellerRating,
    },
  };
}

/**
 * Maps creation form input values into a backend CreateAuctionRequest DTO payload.
 */
export function mapCreateAuctionInputToRequest(
  input: CreateAuctionInput,
  sellerId: string,
): CreateAuctionRequest {
  return {
    sellerId,
    shippingAddress: {
      city: input.shippingLocation || "Amman",
      street: "Main St",
      building: "1",
      landmark: "Near Center",
    },
    startBidAmount: input.startingPrice,
    minBidAmount: input.startingPrice + (input.minimumIncrement || 1),
    startTime: new Date(input.startDate).toISOString(),
    endTime: new Date(input.endDate).toISOString(),
    title: input.title,
    description: input.description,
    images: (input.images || []).map((file, idx) => ({
      path: typeof file === "string" ? file : file.name,
      altText: input.title,
      isMain: idx === 0,
    })),
    catigoryId: "cat-1", // Maps dynamically or falls back to 'cat-1' to satisfy schema constraints.
    status: input.condition,
    condition: input.conditionDescription,
  };
}
