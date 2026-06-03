import { api } from "@/lib/api/client";
import type {
  SellerAuctionsResponse,
  SellerDashboardQueryParams,
  SellerFinancialsResponse,
  SellerOrdersResponse,
} from "./seller-dashboard.contracts";

/**
 * Retrieve seller dashboard auctions.
 */
export async function getSellerDashboardAuctions(
  params?: SellerDashboardQueryParams,
): Promise<SellerAuctionsResponse> {
  const response = await api.get<SellerAuctionsResponse>("/seller-dashboard/auctions", {
    params,
  });
  return response.data;
}

/**
 * Retrieve seller dashboard orders.
 */
export async function getSellerDashboardOrders(
  params?: SellerDashboardQueryParams,
): Promise<SellerOrdersResponse> {
  const response = await api.get<SellerOrdersResponse>("/seller-dashboard/orders", {
    params,
  });
  return response.data;
}

/**
 * Retrieve seller dashboard financials.
 */
export async function getSellerDashboardFinancials(
  params?: SellerDashboardQueryParams,
): Promise<SellerFinancialsResponse> {
  const response = await api.get<SellerFinancialsResponse>("/seller-dashboard/financials", {
    params,
  });
  return response.data;
}

