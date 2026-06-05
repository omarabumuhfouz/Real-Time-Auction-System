/**
 * Pure HTTP REST functions for the Bidding feature.
 * Connects directly to the ASP.NET Core endpoints.
 */

import { api } from "@/lib/api/client";
import type {
  PagedListOfMyBidAuctionDto,
  PlaceBidRequestDto,
} from "./bidding.contracts";

const MY_BIDS_SORT_BY = "CreationDate";

/**
 * Fetches the user's active/ended bid activities.
 */
export async function fetchMyBids(
  params: {
    filter?: string;
    sortBy?: string;
    page?: number;
    pageSize?: number;
  } = {},
): Promise<PagedListOfMyBidAuctionDto> {
  const response = await api.get<PagedListOfMyBidAuctionDto>(
    "/bidders/my-bids",
    {
      params: {
        page: params.page ?? 1,
        pageSize: params.pageSize ?? 5,
        tab: params.filter === "All" ? undefined : params.filter,
        // The backend only accepts explicit contract values here.
        sortBy: MY_BIDS_SORT_BY,
      },
    },
  );
  return response.data;
}

/**
 * Places a bid on an auction.
 */
export async function placeBid(
  auctionId: string,
  request: PlaceBidRequestDto,
): Promise<void> {
  await api.post<void>(`/auctions/${auctionId}/bids`, request);
}
