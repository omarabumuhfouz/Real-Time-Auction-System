/**
 * React Query hooks for fetching Orders data.
 * Fully aligned with the real backend DTO contracts.
 */

import { useQuery } from "@tanstack/react-query";
import type { OrderActivity } from "../types/orders.types";
import { searchOrders, getOrderDetails } from "./order.api";
import { orderKeys } from "./order.keys";
import {
  mapOrderSummaryDtoToActivity,
  mapOrderDetailsDtoToActivity,
} from "./order.mappers";
import { useGetAuctionById } from "@/features/auctions";

// Re-export query keys
export { orderKeys as ORDERS_KEYS };

export interface OrderActivityResponse {
  items: OrderActivity[];
  page: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

/**
 * Hook to retrieve user won orders dynamically from backend.
 * Uses searchOrders endpoint to query for matching UserId.
 */
export const useGetMyOrders = (
  userId: string,
  params: {
    filter?: string;
    sortBy?: string;
    page?: number;
    pageSize?: number;
  } = {},
) => {
  return useQuery<OrderActivityResponse>({
    queryKey: orderKeys.list({ userId, ...params }),
    queryFn: async () => {
      const raw = await searchOrders({
        UserId: userId,
        Status: params.filter === "All" ? undefined : params.filter,
        PageSize: params.pageSize ?? 5,
        PageNumber: params.page ?? 1,
      });

      return {
        items: raw.items.map(mapOrderSummaryDtoToActivity),
        page: raw.pageNumber,
        totalPages: raw.totalPages ?? Math.ceil(raw.totalCount / raw.pageSize),
        totalCount: raw.totalCount,
        hasPreviousPage: raw.hasPreviousPage ?? false,
        hasNextPage: raw.hasNextPage ?? false,
      };
    },
    enabled: !!userId,
  });
};

/**
 * Hook to query detailed order information.
 * Composes both OrderDetails DTO and AuctionSummary info in parallel.
 */
export function useGetOrderDetails(id: string) {
  const orderQuery = useQuery({
    queryKey: orderKeys.detail(id),
    queryFn: () => getOrderDetails(id),
    enabled: !!id,
  });

  const auctionId = orderQuery.data?.auctionId;

  // Query details of the corresponding auction
  const auctionQuery = useGetAuctionById(auctionId || "");

  // Combine raw order DTO with auction info via mapping layer
  const mappedData = orderQuery.data
    ? mapOrderDetailsDtoToActivity(
        orderQuery.data,
        auctionQuery.data || undefined,
      )
    : null;

  return {
    data: mappedData,
    isLoading: orderQuery.isLoading || (!!auctionId && auctionQuery.isLoading),
    isError: orderQuery.isError || auctionQuery.isError,
    error: orderQuery.error || auctionQuery.error,
    refetch: async () => {
      await orderQuery.refetch();
      if (auctionId) {
        await auctionQuery.refetch();
      }
    },
  };
}
