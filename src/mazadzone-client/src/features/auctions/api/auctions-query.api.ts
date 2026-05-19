/**
 * Auction Query API fetch functions.
 * Consumed by TanStack Query hooks.
 */

import type {
  AuctionSummary,
  AuctionCategory,
  AuctionSubcategory,
  AuctionFilters,
  AuctionSortBy,
  PaginatedResponse,
} from "../types/auction.types";
import { AuctionStatus, AuctionSortBy as SortByValues } from "../types/auction.types";
import {
  getMockAuctions,
  getMockAuctionById,
} from "../testing/mock-auctions";

const MOCK_DELAY_MS = 400;

export function simulateDelay(): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, MOCK_DELAY_MS));
}

/**
 * Fetches active auctions, optionally filtered and sorted.
 */
export async function fetchActiveAuctions(
  filters?: AuctionFilters,
): Promise<PaginatedResponse<AuctionSummary>> {
  await simulateDelay();

  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * const { data } = await api.get<PaginatedResponse<AuctionSummary>>("/auctions", { 
   *   params: filters 
   * });
   * return data;
   */

  const page = filters?.page || 1;
  const pageSize = filters?.pageSize || 12;

  let results = getMockAuctions();
  
  if (filters?.status) {
    results = results.filter((a) => a.status === filters.status);
  } else {
    results = results.filter((a) => a.status === AuctionStatus.ACTIVE);
  }

  if (filters) {
    if (filters.search) {
      const query = filters.search.toLowerCase();
      results = results.filter((a) =>
        a.title.toLowerCase().includes(query),
      );
    }

    if (filters.category) {
      results = results.filter((a) => a.category === filters.category);
    }
    
    if (filters.subcategory) {
      results = results.filter((a) => a.subcategory === filters.subcategory);
    }

    if (filters.condition) {
      results = results.filter((a) => a.condition === filters.condition);
    }

    if (filters.minPrice != null) {
      results = results.filter(
        (a) => (a.pricing.currentBid ?? a.pricing.startingPrice) >= filters.minPrice!,
      );
    }
    if (filters.maxPrice != null) {
      results = results.filter(
        (a) => (a.pricing.currentBid ?? a.pricing.startingPrice) <= filters.maxPrice!,
      );
    }

    if (filters.sortBy) {
      results = sortAuctions(results, filters.sortBy, filters.sortDirection);
    }
  }

  const totalCount = results.length;
  const totalPages = Math.ceil(totalCount / pageSize);
  const startIndex = (page - 1) * pageSize;
  const paginatedItems = results.slice(startIndex, startIndex + pageSize);

  return {
    items: paginatedItems,
    totalCount,
    page,
    pageSize,
    totalPages,
    hasNextPage: page < totalPages,
    hasPreviousPage: page > 1,
  };
}

/**
 * Fetches a single auction by ID.
 */
export async function fetchAuctionById(
  id: string,
): Promise<AuctionSummary | undefined> {
  await simulateDelay();

  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * const { data } = await api.get<AuctionSummary>(`/auctions/${id}`);
   * return data;
   */

  return getMockAuctionById(id);
}

/**
 * Fetches auctions matching the given category (active only).
 */
export async function fetchAuctionsByCategory(
  category: AuctionCategory,
): Promise<PaginatedResponse<AuctionSummary>> {
  return fetchActiveAuctions({ category });
}

/**
 * Fetches bid history for a specific auction.
 */
export async function fetchBidHistory(
  auctionId: string
): Promise<import("../types/auction.types").BidHistoryEntry[]> {
  await simulateDelay();

  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * const { data } = await api.get<import("../types/auction.types").BidHistoryEntry[]>(`/auctions/${auctionId}/bids`);
   * return data;
   */

  const auction = getMockAuctionById(auctionId);
  return auction?.bidHistory ?? [];
}

/**
 * Fetches auctions that are ending soonest.
 */
export async function fetchClosingSoonAuctions(
  limit: number = 4,
): Promise<AuctionSummary[]> {
  const response = await fetchActiveAuctions({
    status: AuctionStatus.ACTIVE,
    sortBy: SortByValues.END_TIME,
    sortDirection: "asc",
    pageSize: limit,
  });
  return response.items;
}

/**
 * Fetches similar auctions based on category/subcategory.
 */
export async function fetchSimilarAuctions(
  auctionId: string,
  category: AuctionCategory,
  subcategory: AuctionSubcategory,
  limit: number = 4
): Promise<AuctionSummary[]> {
  await simulateDelay();

  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * const data = await api.get<AuctionSummary[]>(`/auctions/${auctionId}/similar`, { 
   *   params: { limit } 
   * });
   * return data;
   */

  const useCategoryOnly = subcategory === "Others";
  const allAuctions = getMockAuctions();
  
  let results = allAuctions.filter((a) => {
    if (a.id === auctionId || a.status !== AuctionStatus.ACTIVE) return false;

    if (useCategoryOnly) {
      return a.category === category;
    } else {
      return a.subcategory === subcategory;
    }
  });

  if (!useCategoryOnly && results.length < limit) {
    const categoryFallbacks = allAuctions.filter((a) => {
      return (
        a.id !== auctionId && 
        a.status === AuctionStatus.ACTIVE && 
        a.category === category &&
        !results.some(r => r.id === a.id)
      );
    });
    results = [...results, ...categoryFallbacks];
  }

  return results.slice(0, limit);
}

/**
 * Fetches auctions owned by the current seller.
 */
export async function fetchSellerAuctions(
  filters?: { status?: string; sortBy?: string; page?: number; pageSize?: number }
): Promise<PaginatedResponse<AuctionSummary>> {
  await simulateDelay();

  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * const { data } = await api.get<PaginatedResponse<AuctionSummary>>("/auctions/seller", { 
   *   params: filters 
   * });
   * return data;
   */

  const page = filters?.page || 1;
  const pageSize = filters?.pageSize || 5;

  let results = getMockAuctions().map((a, idx) => ({
    ...a,
    isOwner: idx % 3 === 0,
  })).filter(a => a.isOwner);

  if (filters?.status && filters.status !== "All" && filters.status !== "All Statuses") {
    const status = filters.status.toLowerCase();
    results = results.filter((a) => {
      if (status === "active") {
        return a.status === AuctionStatus.ACTIVE;
      }
      if (status === "sold") {
        return a.status === AuctionStatus.ENDED && a.pricing.bidCount > 0;
      }
      if (status === "pending") {
        return a.status === AuctionStatus.UPCOMING;
      }
      if (status === "ended") {
        return a.status === AuctionStatus.ENDED && a.pricing.bidCount === 0;
      }
      return true;
    });
  }

  if (filters?.sortBy) {
    const sortBy = filters.sortBy;
    const getPrice = (a: AuctionSummary) => a.pricing.currentBid ?? a.pricing.startingPrice;
    const getTime = (date: string | Date) => new Date(date).getTime();

    if (sortBy === "EndDate") {
      results.sort((a, b) => getTime(a.timing.endDate) - getTime(b.timing.endDate));
    } else if (sortBy === "CurrentBid") {
      results.sort((a, b) => getPrice(b) - getPrice(a));
    } else if (sortBy === "DateCreated") {
      results.sort((a, b) => getTime(b.timing.creationDate) - getTime(a.timing.creationDate));
    }
  }

  const totalCount = results.length;
  const totalPages = Math.ceil(totalCount / pageSize);
  const startIndex = (page - 1) * pageSize;
  const paginatedItems = results.slice(startIndex, startIndex + pageSize);

  return {
    items: paginatedItems,
    totalCount,
    page,
    pageSize,
    totalPages,
    hasNextPage: page < totalPages,
    hasPreviousPage: page > 1,
  };
}

function sortAuctions(
  auctions: AuctionSummary[],
  sortBy: AuctionSortBy,
  direction: "asc" | "desc" = "desc",
): AuctionSummary[] {
  const sorted = [...auctions];

  const getPrice = (a: AuctionSummary) =>
    a.pricing.currentBid ?? a.pricing.startingPrice;
  const getTime = (date: string | Date) => new Date(date).getTime();

  const strategy: Record<AuctionSortBy, (a: AuctionSummary, b: AuctionSummary) => number> = {
    [SortByValues.CREATION_DATE]: (a, b) =>
      getTime(a.timing.creationDate) - getTime(b.timing.creationDate),
    [SortByValues.PRICE]: (a, b) =>
      getPrice(a) - getPrice(b),
    [SortByValues.START_TIME]: (a, b) =>
      getTime(a.timing.startDate) - getTime(b.timing.startDate),
    [SortByValues.END_TIME]: (a, b) =>
      getTime(a.timing.endDate) - getTime(b.timing.endDate),
  };

  const comparator = strategy[sortBy] ?? (() => 0);

  return sorted.sort((a, b) => {
    const result = comparator(a, b);
    return direction === "asc" ? result : -result;
  });
}
