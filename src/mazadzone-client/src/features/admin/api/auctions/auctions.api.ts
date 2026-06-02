import { api } from "@/lib/api/client";
import type { ModerateAuctionsResponse, AuctionStatus } from "../../types/admin.types";
import type { PagedListOfAuctionsListDto } from "./auctions.contracts";
import {
  mapFiltersToQueryParams,
  mapPagedAuctionsListToModerateAuctionsResponse,
  mapAuctionsListDtoToModerateAuction,
} from "./auctions.mappers";

export interface UseModerateAuctionsFilters {
  search: string;
  category: string;
  status: AuctionStatus | "All Statuses";
  sortBy: string;
  dateFrom: string;
  page: number;
  pageSize: number;
}

/**
 * Fetches real filtered, paginated moderate auctions list from the backend.
 */
export async function fetchModerateAuctions(filters: UseModerateAuctionsFilters): Promise<ModerateAuctionsResponse> {
  const queryParams = mapFiltersToQueryParams(filters);
  const response = await api.get<PagedListOfAuctionsListDto>("/auctions", {
    params: queryParams,
  });
  return mapPagedAuctionsListToModerateAuctionsResponse(response.data, filters.page, filters.pageSize);
}

/**
 * Cancels an auction listing as an administrator.
 */
export async function cancelAuctionByAdminApi(auctionId: string, reason: string): Promise<void> {
  // Respecting standard POST with optional/ignored payload as defined in OpenAPI spec
  await api.post(`/auctions/${auctionId}/cancel-by-admin`, { reason });
}

/**
 * Force ends bidding on an active auction listing.
 */
export async function exportAuctionsApi(filters: UseModerateAuctionsFilters, selectedIds: string[]): Promise<Blob> {
  const queryParams = mapFiltersToQueryParams({
    ...filters,
    page: 1,
    pageSize: 200, // Fetch a larger window of rows to guarantee full match export
  });

  const response = await api.get<PagedListOfAuctionsListDto>("/auctions", {
    params: queryParams,
  });
  const items = response.data.items || [];

  let exportItems = items.map(mapAuctionsListDtoToModerateAuction);
  if (selectedIds.length > 0) {
    exportItems = exportItems.filter((a) => selectedIds.includes(a.id));
  }

  const csvHeader = "ID,Title,Seller,Seller Email,Category,Status,Current Bid,Bid Count,Start Date,End Date\n";
  const csvRows = exportItems
    .map(
      (a) =>
        `${a.id},"${a.title}","${a.sellerName}","${a.sellerEmail}",${a.category},${a.status},${a.currentBid},${a.bidCount},${a.startDate},${a.endDate}`
    )
    .join("\n");

  return new Blob([csvHeader + csvRows], { type: "text/csv;charset=utf-8;" });
}
