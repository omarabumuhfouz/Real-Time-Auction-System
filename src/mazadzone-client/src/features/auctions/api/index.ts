// TanStack Query hooks
export {
  auctionKeys,
  useGetAuctions,
  useGetAuctionById,
  useGetAuctionsByCategory,
  useGetClosingSoonAuctions,
  useGetBidHistory,
  useGetSimilarAuctions,
  useGetSellerAuctions,
} from "./auction.queries";

// Mutations
export { useCreateAuction, useUpdateAuction, useDeleteAuction } from "./auction.mutations";

// Raw fetch functions (for non-hook usage or prefetching)
export {
  fetchActiveAuctions,
  fetchAuctionById,
  fetchAuctionsByCategory,
  fetchClosingSoonAuctions,
  fetchSellerAuctions,
} from "./auctions-query.api";

export {
  createAuctionApi,
  updateAuctionApi,
  deleteAuctionApi,
} from "./auctions-mutation.api";
