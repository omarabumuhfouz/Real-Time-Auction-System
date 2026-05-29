/**
 * Pure HTTP REST functions for the Auctions feature.
 * Connects directly to the ASP.NET Core endpoints.
 */

import { api } from "@/lib/api/client";
import type {
  AuctionDto,
  AuctionsListDto,
  CreateAuctionRequest,
  GetAuctionsQueryParams,
  PagedListOfAuctionsListDto,
  CategoryDto,
  SellerAuctionsResponse,
  SellerOrdersResponse,
  SellerFinancialsResponse,
  SellerDashboardQueryParams,
} from "./auction.contracts";

/**
 * Search and filter auctions from the backend.
 */
export async function getAuctions(
  params?: GetAuctionsQueryParams,
): Promise<PagedListOfAuctionsListDto> {
  const response = await api.get<PagedListOfAuctionsListDto>("/auctions", {
    params,
  });
  return response.data;
}

/**
 * Get detailed information about a single auction by ID.
 */
export async function getAuctionById(id: string): Promise<AuctionDto> {
  const response = await api.get<AuctionDto>(`/auctions/${id}`);
  return response.data;
}

/**
 * Get a list of similar auctions for a specific auction ID.
 */
export async function getSimilarAuctions(
  id: string,
  limit?: number,
): Promise<AuctionsListDto[]> {
  const response = await api.get<AuctionsListDto[]>(
    `/auctions/${id}/similar`,
    {
      params: limit ? { limit } : undefined,
    },
  );
  return response.data;
}

/**
 * Create a new auction listing.
 */
export async function createAuction(request: CreateAuctionRequest): Promise<string> {
  const response = await api.post<string>("/auctions", request);
  return response.data;
}

/**
 * Activate an upcoming/pending auction.
 */
export async function activateAuction(id: string): Promise<void> {
  await api.post<void>(`/auctions/${id}/activate`);
}

/**
 * Manually end an active auction.
 */
export async function endAuction(id: string): Promise<void> {
  await api.post<void>(`/auctions/${id}/end`);
}

/**
 * Cancel an auction listing with a reason.
 */
export async function cancelAuction(id: string, reason: string): Promise<void> {
  await api.post<void>(`/auctions/${id}/cancel`, { reason });
}

/**
 * Fetch root level categories from the backend.
 */
export async function getRootCategories(): Promise<CategoryDto[]> {
  const response = await api.get<CategoryDto[]>("/categories/roots");
  return response.data;
}

/**
 * Fetch full category tree (with subcategories) from the backend.
 */
export async function getCategoryTree(): Promise<CategoryDto[]> {
  const response = await api.get<CategoryDto[]>("/categories/tree");
  return response.data;
}

/**
 * Retrieve seller dashboard auctions.
 */
export async function getSellerDashboardAuctions(
  params?: SellerDashboardQueryParams,
): Promise<SellerAuctionsResponse> {
  const response = await api.get<SellerAuctionsResponse>("/seller-dashboard/auctions", {
    params,
  });
  return response.data;
}

/**
 * Retrieve seller dashboard orders.
 */
export async function getSellerDashboardOrders(
  params?: SellerDashboardQueryParams,
): Promise<SellerOrdersResponse> {
  const response = await api.get<SellerOrdersResponse>("/seller-dashboard/orders", {
    params,
  });
  return response.data;
}

/**
 * Retrieve seller dashboard financials.
 */
export async function getSellerDashboardFinancials(
  params?: SellerDashboardQueryParams,
): Promise<SellerFinancialsResponse> {
  const response = await api.get<SellerFinancialsResponse>("/seller-dashboard/financials", {
    params,
  });
  return response.data;
}
