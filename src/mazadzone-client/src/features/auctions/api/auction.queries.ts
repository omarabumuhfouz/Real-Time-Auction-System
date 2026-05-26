/**
 * React Query hooks for fetching Auctions data.
 * Fully aligned with the real backend DTO contracts.
 */

import { useQuery } from "@tanstack/react-query";
import type {
  AuctionSummary,
  AuctionCategory,
  AuctionSubcategory,
  AuctionFilters,
  PaginatedResponse,
} from "../types/auction.types";

import {
  getAuctions,
  getAuctionById,
  getSimilarAuctions,
} from "./auction.api";

import { auctionKeys } from "./auction.keys";
import {
  mapFiltersToQueryParams,
  mapAuctionsListDtoToSummary,
  mapAuctionDtoToSummary,
} from "./auction.mappers";

// Re-export query keys
export { auctionKeys };

/**
 * Fetches active auctions with optional filters and sorting.
 * Maps raw backend paginated list to presentational ViewModels.
 */
export function useGetAuctions(filters?: AuctionFilters) {
  return useQuery<PaginatedResponse<AuctionSummary>>({
    queryKey: auctionKeys.list(filters || {}),
    queryFn: async () => {
      const queryParams = mapFiltersToQueryParams(filters);
      const raw = await getAuctions(queryParams);
      
      return {
        items: raw.items.map(mapAuctionsListDtoToSummary),
        totalCount: raw.totalCount,
        page: raw.pageNumber,
        pageSize: raw.pageSize,
        totalPages: raw.totalPages ?? Math.ceil(raw.totalCount / raw.pageSize),
        hasNextPage: raw.hasNextPage ?? false,
        hasPreviousPage: raw.hasPreviousPage ?? false,
      };
    },
  });
}

/**
 * Fetches detailed information about a single auction by ID.
 */
export function useGetAuctionById(id: string) {
  return useQuery<AuctionSummary | null>({
    queryKey: auctionKeys.detail(id),
    queryFn: async () => {
      const raw = await getAuctionById(id);
      return mapAuctionDtoToSummary(raw);
    },
    enabled: !!id,
  });
}

/**
 * Fetches auctions by category display string.
 */
export function useGetAuctionsByCategory(category: AuctionCategory) {
  return useQuery<PaginatedResponse<AuctionSummary>>({
    queryKey: [...auctionKeys.all, "category", category],
    queryFn: async () => {
      const raw = await getAuctions({
        Page: 1,
        PageSize: 12,
        CategoryId: category,
      });

      return {
        items: raw.items.map(mapAuctionsListDtoToSummary),
        totalCount: raw.totalCount,
        page: raw.pageNumber,
        pageSize: raw.pageSize,
        totalPages: raw.totalPages ?? 1,
        hasNextPage: raw.hasNextPage ?? false,
        hasPreviousPage: raw.hasPreviousPage ?? false,
      };
    },
    enabled: !!category,
  });
}

/**
 * Hook to get active auctions ending soon.
 */
export function useGetClosingSoonAuctions(limit: number = 4) {
  return useQuery<AuctionSummary[]>({
    queryKey: [...auctionKeys.all, "closing-soon", limit],
    queryFn: async () => {
      const raw = await getAuctions({
        Page: 1,
        PageSize: limit,
        Status: "Active",
        SortBy: "EndTime",
        SortDirection: "asc",
      });
      return raw.items.map(mapAuctionsListDtoToSummary);
    },
  });
}

/**
 * Hook to get upcoming active auctions.
 */
export function useGetUpcomingAuctions(limit: number = 4) {
  return useQuery<AuctionSummary[]>({
    queryKey: [...auctionKeys.all, "upcoming", limit],
    queryFn: async () => {
      const raw = await getAuctions({
        Page: 1,
        PageSize: limit,
        Status: "Pending",
        SortBy: "StartTime",
        SortDirection: "asc",
      });
      return raw.items.map(mapAuctionsListDtoToSummary);
    },
  });
}

/**
 * Hook to get bid history for a specific auction.
 * Extracts bid list directly from the detailed auction DTO.
 */
export function useGetBidHistory(auctionId: string) {
  return useQuery({
    queryKey: [...auctionKeys.detail(auctionId), "bids"],
    queryFn: async () => {
      const raw = await getAuctionById(auctionId);
      return (raw.bids ?? []).map((b, idx) => ({
        id: `${auctionId}-bid-${idx}`,
        bidderName: `Bidder ${b.bidderId.substring(0, 4)}`,
        bidderInitial: "B",
        amount: b.amount,
        timeAgo: new Date(b.timestamp).toLocaleDateString(),
        isHighest: idx === 0,
      }));
    },
    enabled: !!auctionId,
  });
}

/**
 * Hook to get similar auctions.
 */
export function useGetSimilarAuctions(
  auctionId: string,
  category: AuctionCategory,
  subcategory: AuctionSubcategory,
  limit: number = 4,
) {
  return useQuery<AuctionSummary[]>({
    queryKey: auctionKeys.similar(auctionId, limit),
    queryFn: async () => {
      const raw = await getSimilarAuctions(auctionId, limit);
      return raw.map(mapAuctionsListDtoToSummary);
    },
    enabled: !!auctionId,
  });
}

/**
 * Hook to get auctions owned by the current seller.
 */
export function useGetSellerAuctions(filters?: {
  status?: string;
  sortBy?: string;
  page?: number;
  pageSize?: number;
}) {
  return useQuery<PaginatedResponse<AuctionSummary>>({
    queryKey: [...auctionKeys.all, "seller", filters || {}],
    queryFn: async () => {
      let statusParam = filters?.status === "All" ? undefined : filters?.status;
      if (statusParam === "Upcoming") {
        statusParam = "Pending";
      }

      const raw = await getAuctions({
        Page: filters?.page ?? 1,
        PageSize: filters?.pageSize ?? 5,
        Status: statusParam,
        SortBy: filters?.sortBy,
      });

      return {
        items: raw.items.map(mapAuctionsListDtoToSummary),
        totalCount: raw.totalCount,
        page: raw.pageNumber,
        pageSize: raw.pageSize,
        totalPages: raw.totalPages ?? 1,
        hasNextPage: raw.hasNextPage ?? false,
        hasPreviousPage: raw.hasPreviousPage ?? false,
      };
    },
  });
}
