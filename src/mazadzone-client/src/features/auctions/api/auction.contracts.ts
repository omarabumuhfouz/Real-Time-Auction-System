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
  status?: string;
  condition?: string;
}

export interface AuctionsListDto {
  id: string;
  imageUrl: string;
  itemTitle: string;
  currentBidAmount: number;
  startTime: string;
  endTime: string;
  itemStatus?: string;
  condtion?: string;
  status?: string;
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
  status?: string;
  condition?: string;
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

export interface CategoryDto {
  id: string;
  name: string;
  subCategories?: CategoryDto[];
  subcategories?: CategoryDto[];
}

export interface SellerDashboardQueryParams {
  Status?: string;
  SearchTerm?: string;
  SortBy?: string;
  SortDirection?: string;
  DateFrom?: string;
  DateTo?: string;
  Page: number;
  PageSize: number;
}

export interface SellerAuctionSummaryDto {
  auctionId: string;
  title: string;
  category: string;
  status: string;
  bidsCount: number;
  lastBidAmount: number;
  endDateUtc: string;
  thumbnailUrl: string | null;
}

export interface SellerAuctionsResponse {
  activeAuctions: number;
  pending: number;
  soldItems: number;
  unsold: number;
  totalCount: number;
  auctions: SellerAuctionSummaryDto[];
}

export interface SellerOrderSummaryDto {
  orderId: string;
  auctionId: string;
  auctionTitle: string;
  orderStatus: string;
  orderDateUtc: string;
  totalAmount: number;
  buyerName: string;
}

export interface SellerOrdersResponse {
  totalCount: number;
  orders: SellerOrderSummaryDto[];
}

export interface SellerFinancialsResponse {
  totalGrossRevenue: number;
  totalPlatformFees: number;
  totalNetProfit: number;
  completedOrdersCount: number;
}

