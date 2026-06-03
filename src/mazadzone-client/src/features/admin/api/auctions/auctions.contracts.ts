/**
 * Local contract representations of backend API contracts for the Admin Auctions feature.
 *
 * These types match the ASP.NET Core OpenAPI contract exactly.
 * Do NOT import generated types directly.
 */

export interface AuctionsListDto {
  id: string;
  imageUrl: string;
  itemTitle: string;
  currentBidAmount: number;
  startTime: string;
  endTime: string;
  itemStatus?: string;
  condtion?: string; // Respecting backend 'condtion' typo exactly
  status?: string;
  bidsCount: number;
}

export interface PagedListOfAuctionsListDto {
  pageNumber: number;
  pageSize: number;
  totalPages?: number;
  totalCount: number;
  items: AuctionsListDto[];
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

export interface GetAuctionsQueryParams {
  Page: number;
  PageSize: number;
  SearchTerm?: string;
  CategoryId?: string;
  MinCurrentBid?: number;
  MaxCurrentBid?: number;
  Status?: string;
  SortBy?: string;
  SortDirection?: string;
}
