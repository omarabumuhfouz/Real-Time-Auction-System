import type { HubConnection } from "@microsoft/signalr";
import {
  createHubConnection,
  startConnection,
  stopConnection,
} from "./connection-factory";

// --- Event Types (server → client) ------------------------------

export interface BidPlacedEvent {
  auctionId: string;
  newPrice: number;
}

export interface StatusChangedEvent {
  auctionId: string;
  status: string;
}

export interface AuctionCreatedEvent {
  auctionId: string;
  status: string;
}

export interface AuctionEndedEvent {
  auctionId: string;
  winnerId: string | null;
  winningBid: number | null;
  endedAt: string;
}

export interface CountdownTickEvent {
  auctionId: string;
  remainingSeconds: number;
}

// --- Invocation Payloads (client → server) -----------------------

export interface PlaceBidPayload {
  auctionId: string;
  amount: number;
}

// --- Bidding Hub Client ------------------------------------------

/**
 * Typed client for the Bidding / Auctions SignalR hub.
 *
 * Usage:
 * ```ts
 * const hub = createBiddingHubClient();
 * await hub.start();
 * hub.onBidPlaced((event) => console.log(event));
 * ```
 */
export function createBiddingHubClient(accessTokenFactory?: () => string | Promise<string>) {
  // Connect to the "/auctions" hub (which maps to "/hubs/auctions" on backend)
  const connection: HubConnection = createHubConnection("/auctions", accessTokenFactory);

  return {
    /** The underlying SignalR connection (for advanced use) */
    connection,

    // -- Lifecycle ------------------------------------------
    start: () => startConnection(connection),
    stop: () => stopConnection(connection),

    // -- Subscribe to server events -------------------------
    onBidPlaced: (callback: (event: BidPlacedEvent) => void) => {
      connection.on("BidPlaced", callback);
      return () => connection.off("BidPlaced", callback);
    },

    onStatusChanged: (callback: (event: StatusChangedEvent) => void) => {
      connection.on("StatusChanged", callback);
      return () => connection.off("StatusChanged", callback);
    },

    onAuctionCreated: (callback: (event: AuctionCreatedEvent) => void) => {
      connection.on("AuctionCreated", callback);
      return () => connection.off("AuctionCreated", callback);
    },

    // Legacy event listeners preserved for stability
    onAuctionEnded: (callback: (event: AuctionEndedEvent) => void) => {
      connection.on("AuctionEnded", callback);
      return () => connection.off("AuctionEnded", callback);
    },

    onCountdownTick: (callback: (event: CountdownTickEvent) => void) => {
      connection.on("CountdownTick", callback);
      return () => connection.off("CountdownTick", callback);
    },

    // -- Invoke server methods (Unused/Empty on backend AuctionsHub) -------
    placeBid: (payload: PlaceBidPayload) =>
      connection.invoke("PlaceBid", payload),

    joinAuction: (auctionId: string) =>
      connection.invoke("JoinAuction", auctionId),

    leaveAuction: (auctionId: string) =>
      connection.invoke("LeaveAuction", auctionId),
  };
}

export type BiddingHubClient = ReturnType<typeof createBiddingHubClient>;

