import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchMyOrders, completeOrderPayment } from "./orders.api";

export const ORDERS_KEYS = {
  all: ["orders"] as const,
  myOrders: (
    userId: string,
    params: { filter?: string; sortBy?: string; page?: number; pageSize?: number }
  ) => [...ORDERS_KEYS.all, "my-orders", userId, params] as const,
};

/**
 * useGetMyOrders Hook
 * 
 * Hook to retrieve user won orders dynamically from backend / mock layer.
 * Utilizes TanStack Query for cache invalidation and loading state monitoring.
 * 
 * @param userId - The active user's ID
 * @param params - Filtering and pagination options
 */
export const useGetMyOrders = (
  userId: string,
  params: { filter?: string; sortBy?: string; page?: number; pageSize?: number } = {}
) => {
  return useQuery({
    queryKey: ORDERS_KEYS.myOrders(userId, params),
    queryFn: () => fetchMyOrders(userId, params),
    enabled: !!userId,
  });
};

/**
 * useCompleteOrderPayment Mutation Hook
 * 
 * Submits the remaining 90% payment authorization to confirm the won order.
 * On success, invalidates won orders list query to update order status badge in the UI.
 */
export const useCompleteOrderPayment = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      orderId,
      addressId,
      paymentMethodId,
    }: {
      orderId: string;
      addressId: string;
      paymentMethodId: string;
    }) => completeOrderPayment(orderId, addressId, paymentMethodId),
    onSuccess: () => {
      // Invalidate all orders to update the status dynamically in Won Orders list
      queryClient.invalidateQueries({ queryKey: ORDERS_KEYS.all });
    },
  });
};
