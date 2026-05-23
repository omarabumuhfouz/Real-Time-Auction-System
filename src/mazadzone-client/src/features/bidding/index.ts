/**
 * Bidding feature — public API.
 *
 * TODO: Implement bid placement form, bid history,
 * real-time bid updates via SignalR, and bidding hooks.
 */

// Components
export { PlaceBidButton } from "./components/PlaceBidButton";
export { PlaceBidModal } from "./components/place-bid/PlaceBidModal";
export { MyBidsPage } from "./components/my-bids/MyBidsPage";

// Hooks / Queries
export { useGetMyBids, usePlaceBid, useGetSavedPaymentMethods } from "./api/bidding.queries";
export { fetchMyBids } from "./api/bidding.api";

// Types
export type { BidActivity, BidStatus } from "./types/bidding.types";
export type { PlaceBidRequest, PlaceBidResponse, PlaceBidModalProps } from "./types/place-bid.types";

