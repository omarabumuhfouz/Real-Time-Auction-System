// TanStack Query hooks
export {
  auctionKeys,
  useGetAuctions,
  useGetAuctionById,
  useGetAuctionsByCategory,
  useGetClosingSoonAuctions,
  useGetUpcomingAuctions,
  useGetBidHistory,
  useGetSimilarAuctions,
  useGetSellerAuctions,
  useGetRootCategories,
  useGetCategoryTree,
  useGetSellerDashboardAuctions,
  useGetSellerDashboardOrders,
  useGetSellerDashboardFinancials,
} from "./auction.queries";

// Mutations
export {
  useCreateAuction,
  useUpdateAuction,
  useDeleteAuction,
  useActivateAuction,
  useEndAuction,
  useCancelAuction,
} from "./auction.mutations";

// Pure REST API methods
export {
  getAuctions,
  getAuctionById,
  getSimilarAuctions,
  createAuction,
  activateAuction,
  endAuction,
  cancelAuction,
  getRootCategories,
  getCategoryTree,
  getSellerDashboardAuctions,
  getSellerDashboardOrders,
  getSellerDashboardFinancials,
} from "./auction.api";

// Pure Mappers
export {
  mapFiltersToQueryParams,
  mapAuctionsListDtoToSummary,
  mapAuctionDtoToSummary,
  mapCreateAuctionInputToRequest,
} from "./auction.mappers";

export type {
  SellerAuctionSummaryDto,
  SellerOrderSummaryDto,
} from "./auction.contracts";
