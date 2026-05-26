/**
 * Local contract representations of backend API contracts for the Auctions feature.
 *
 * These types match the ASP.NET Core OpenAPI contract exactly.
 * Do NOT import generated types directly.
 */

export interface BidDto {
  bidderId: string;
  amount: number;
  bidStatus: number;
  timestamp: string;
}

export interface AuctionDto {
  id: string;
  itemTitle: string;
  itemDescription: string;
  imageUrls: string[];
  sellerName: string;
  sellerEmail: string;
  sellerRating: number;
  reviewCount: number;
  startBidAmount: number;
  minBidAmount: number;
  currentBidAmount: number;
  startTime: string;
  endTime: string;
  auctionStatus: string;
  bids: BidDto[];
}

export interface AuctionsListDto {
  id: string;
  imageUrl: string;
  itemTitle: string;
  currentBidAmount: number;
  startTime: string;
  endTime: string;
  status: string;
  bidsCount: number;
}

export interface AddressDto {
  city: string;
  street: string;
  building: string;
  landmark: string;
}

export interface ImageModelDto {
  path?: string;
  altText?: string;
  isMain?: boolean;
}

export interface CreateAuctionRequest {
  sellerId: string;
  shippingAddress: AddressDto;
  startBidAmount: number;
  minBidAmount: number;
  startTime: string;
  endTime: string;
  title: string;
  description: string;
  images: ImageModelDto[];
  catigoryId: string; // Note: respecting the backend 'catigoryId' typo exactly.
}

export interface GetAuctionsQueryParams {
  Page: number;
  PageSize: number;
  SearchTerm?: string;
  CategoryId?: string;
  SubCategoryId?: string;
  MinCurrentBid?: number;
  MaxCurrentBid?: number;
  Status?: string;
  SortBy?: string;
  SortDirection?: string;
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
