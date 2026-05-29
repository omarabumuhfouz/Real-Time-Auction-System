import { useQuery } from "@tanstack/react-query";
import {
  getSellerDashboardAuctions,
  getSellerDashboardFinancials,
  getSellerDashboardOrders,
} from "./seller-dashboard.api";
import type {
  SellerAuctionsResponse,
  SellerDashboardQueryParams,
  SellerFinancialsResponse,
  SellerOrdersResponse,
} from "./seller-dashboard.contracts";

/**
 * Hook to retrieve seller dashboard auctions.
 */
export function useGetSellerDashboardAuctions(filters: SellerDashboardQueryParams) {
  return useQuery<SellerAuctionsResponse>({
    queryKey: ["seller-dashboard", "auctions", filters],
    queryFn: () => getSellerDashboardAuctions(filters),
  });
}

/**
 * Hook to retrieve seller dashboard orders.
 */
export function useGetSellerDashboardOrders(filters: SellerDashboardQueryParams) {
  return useQuery<SellerOrdersResponse>({
    queryKey: ["seller-dashboard", "orders", filters],
    queryFn: () => getSellerDashboardOrders(filters),
  });
}

/**
 * Hook to retrieve seller dashboard financials.
 */
export function useGetSellerDashboardFinancials(filters?: Partial<SellerDashboardQueryParams>) {
  return useQuery<SellerFinancialsResponse>({
    queryKey: ["seller-dashboard", "financials", filters || {}],
    queryFn: () =>
      getSellerDashboardFinancials({
        Page: filters?.Page ?? 1,
        PageSize: filters?.PageSize ?? 10,
        ...filters,
      }),
  });
}

