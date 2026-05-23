import { api } from "@/lib/api/client";
import { OrderActivity } from "../types/orders.types";
import { getMockOrders } from "../testing/mock-orders";
import { CheckoutPaymentResponse } from "../types/checkout.types";

export interface OrderActivityResponse {
  items: OrderActivity[];
  page: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

/**
 * fetchMyOrders Function
 * 
 * Fetches the user's won orders.
 * Backed by mock data during development and structured to easily swap
 * with the real backend API endpoint by uncommenting the block below.
 * 
 * @param userId - The unique identifier of the authenticated user.
 * @param params - Query parameters for filtering, sorting, and paginating.
 * @returns A promise resolving to a paginated OrderActivityResponse.
 */
export async function fetchMyOrders(
  userId: string,
  params: { filter?: string; sortBy?: string; page?: number; pageSize?: number } = {}
): Promise<OrderActivityResponse> {
  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * const { data } = await api.get<OrderActivityResponse>(`/orders/my-orders`, {
   *   params: { userId, ...params }
   * });
   * return data;
   */

  // --- MOCK IMPLEMENTATION ---
  await new Promise((resolve) => setTimeout(resolve, 650)); // Simulate network latency

  const allOrders = getMockOrders();

  // Filter
  const filter = params.filter || "All";
  const filteredOrders = allOrders.filter((order) => {
    if (filter === "All") return true;
    return order.status === filter;
  });

  // Sort
  const sortBy = params.sortBy || "latest";
  const sortedOrders = [...filteredOrders].sort((a, b) => {
    const timeA = new Date(a.date).getTime();
    const timeB = new Date(b.date).getTime();
    if (sortBy === "latest") {
      return timeB - timeA;
    }
    if (sortBy === "oldest") {
      return timeA - timeB;
    }
    return 0;
  });

  // Paginate
  const page = params.page || 1;
  const pageSize = params.pageSize || 5;
  const totalCount = sortedOrders.length;
  const totalPages = Math.ceil(totalCount / pageSize);
  const offset = (page - 1) * pageSize;
  const items = sortedOrders.slice(offset, offset + pageSize);

  return {
    items,
    page,
    totalPages,
    totalCount,
    hasPreviousPage: page > 1,
    hasNextPage: page < totalPages,
  };
}

export interface CompletePaymentRequest {
  orderId: string;
  addressId: string;
  paymentMethodId: string;
}

/**
 * completeOrderPayment Function
 * 
 * Submits checkout confirmation and handles simulated credit card authorization/charge
 * for the remaining 90% of the bid amount.
 */
export async function completeOrderPayment(
  orderId: string,
  addressId: string,
  paymentMethodId: string
): Promise<CheckoutPaymentResponse> {
  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * const { data } = await api.post<CheckoutPaymentResponse>(`/orders/${orderId}/complete-payment`, {
   *   addressId,
   *   paymentMethodId,
   * });
   * return data;
   */

  // --- MOCK IMPLEMENTATION ---
  await new Promise((resolve) => setTimeout(resolve, 1500)); // Simulate payment processing delay

  const allOrders = getMockOrders();
  const order = allOrders.find((o) => o.id === orderId);
  if (!order) {
    throw new Error("Order not found");
  }

  // Update mock order state
  order.status = "Confirmed";
  order.deliveryStatus = "Payment verified. Awaiting shipment from seller.";

  // Mock response payload
  return {
    orderId: order.id,
    orderNumber: order.orderNumber,
    status: "Confirmed",
    amountPaid: order.finalBid * 0.9, // 90% remaining charged
    deliveryAddress: {
      id: addressId,
      label: "Home (Default)",
      fullName: "Omar Ahmad",
      phoneNumber: "+962 7 9123 4567",
      streetAddress: "Al-Hamra Street",
      building: "Building 12, Floor 3",
      city: "Amman",
      isDefault: true,
    },
    paymentMethod: {
      id: paymentMethodId,
      cardType: "VISA",
      lastFourDigits: "4242",
      expiryDate: "12/28",
      cardholderName: "Omar Ahmad",
      isDefault: true,
    },
    transactionId: `tx-${Date.now()}-${Math.floor(Math.random() * 10000)}`,
    paidAt: new Date().toISOString(),
  };
}
