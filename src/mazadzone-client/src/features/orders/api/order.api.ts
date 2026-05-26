/**
 * Pure HTTP REST functions for the Orders feature.
 * Connects directly to the ASP.NET Core endpoints.
 */

import { api } from "@/lib/api/client";
import type {
  CreateOrderRequest,
  OrderDetailsDto,
  PagedListOfOrderSummaryDto,
  SearchOrdersQueryParams,
} from "./order.contracts";

/**
 * Search and filter orders from the backend.
 */
export async function searchOrders(
  params?: SearchOrdersQueryParams,
): Promise<PagedListOfOrderSummaryDto> {
  const response = await api.get<PagedListOfOrderSummaryDto>(
    "/api/v1/orders/search",
    { params },
  );
  return response.data;
}

/**
 * Get detailed information about a single order by ID.
 */
export async function getOrderDetails(id: string): Promise<OrderDetailsDto> {
  const response = await api.get<OrderDetailsDto>(`/api/v1/orders/${id}`);
  return response.data;
}

/**
 * Create a new order listing via PUT.
 */
export async function createOrder(request: CreateOrderRequest): Promise<void> {
  await api.put<void>("/api/v1/orders", request);
}

/**
 * Pays the remaining amount for an order after authorization.
 */
export async function payRemainingAmount(
  orderId: string,
  paymentMethodId: string,
): Promise<void> {
  await api.post<void>(`/api/v1/payments/${orderId}/pay-remaining`, {
    paymentMethodId,
  });
}

/**
 * Confirm an order (Seller lifecycle action).
 */
export async function confirmOrder(id: string): Promise<void> {
  await api.put<void>(`/api/v1/orders/${id}/confirm`);
}

/**
 * Mark an order as shipped (Seller lifecycle action).
 */
export async function shipOrder(id: string): Promise<void> {
  await api.put<void>(`/api/v1/orders/${id}/ship`);
}

/**
 * Mark an order as delivered (Buyer lifecycle action).
 */
export async function deliverOrder(id: string): Promise<void> {
  await api.put<void>(`/api/v1/orders/${id}/deliver`);
}

/**
 * Cancel an order.
 */
export async function cancelOrder(id: string): Promise<void> {
  await api.put<void>(`/api/v1/orders/${id}/cancel`);
}
