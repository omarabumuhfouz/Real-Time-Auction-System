/**
 * Local contract representations of backend API contracts for the Orders feature.
 *
 * These types match the ASP.NET Core OpenAPI contract exactly.
 */

export interface AddressDto {
  city: string;
  street: string;
  building: string;
  landmark: string;
}

export interface OrderDetailsDto {
  id: string;
  totalAmount: number;
  currency: string;
  bidderId: string;
  winningBidId: string;
  auctionId: string;
  status: string;
  hasActiveDispute: boolean;
  isDisputable: boolean;
  canLeaveFeedback: boolean;
}

export interface OrderSummaryDto {
  id: string;
  status: string;
  totalAmount: number;
  currency: string;
  createdAt: string;
  isDisputable: boolean;
  hasActiveDispute: boolean;
  canLeaveFeedback: boolean;
}

export interface CreateOrderRequest {
  auctionId: string;
  bidderId: string;
  winningBidId: string;
  receiptAddress: AddressDto;
  amount: number;
}

export interface PayRemainingAmountRequest {
  paymentMethodId: string;
}

export interface SearchOrdersQueryParams {
  UserId?: string;
  Status?: string;
  PageSize?: number;
  PageNumber?: number;
}

export interface PagedListOfOrderSummaryDto {
  pageNumber: number;
  pageSize: number;
  totalPages?: number;
  totalCount: number;
  items: OrderSummaryDto[];
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}
