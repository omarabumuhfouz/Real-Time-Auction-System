"use client";

import { useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { createBiddingHubClient } from "@/lib/signalr";
import { useAuthStore } from "@/stores/auth.store";
import { APP_CONFIG } from "@/config/app.config";
import { auctionKeys } from "../api/auction.keys";
import { AuctionStatus } from "../types/auction.types";
import type { AuctionSummary } from "../types/auction.types";

/**
 * Maps standard backend status string values to front-end AuctionStatus enum values.
 */
function mapBackendStatusToFrontend(backendStatus: string): AuctionStatus {
  const normalized = backendStatus.toLowerCase();
  if (normalized.includes("active")) return AuctionStatus.ACTIVE;
  if (normalized.includes("ended") || normalized.includes("completed")) return AuctionStatus.ENDED;
  return AuctionStatus.UPCOMING;
}

/**
 * Hook to establish a real-time SignalR connection to the Bidding / Auctions hub.
 *
 * It listens for `BidPlaced`, `StatusChanged`, and `AuctionCreated` events globally.
 * When events are received, it performs high-efficiency, zero-lag local updates to the
 * TanStack Query cache so the UI updates instantly, and schedules background query invalidations
 * to fetch official database logs and history.
 */
export function useRealtimeAuctions(): void {
  const queryClient = useQueryClient();

  useEffect(() => {
    // Respect central realtime feature flag
    if (!APP_CONFIG.enableRealtime) {
      return;
    }

    // Pass the access token factory so authenticated connections can be resolved seamlessly
    const hub = createBiddingHubClient(() => useAuthStore.getState().accessToken ?? "");
    let isMounted = true;
    let isConnected = false;
    let unsubscribeBidPlaced: (() => void) | undefined;
    let unsubscribeStatusChanged: (() => void) | undefined;
    let unsubscribeAuctionCreated: (() => void) | undefined;
    let retryTimeoutId: NodeJS.Timeout | undefined;
    let currentRetry = 0;
    const maxRetries = 3;

    const startHub = async () => {
      try {
        await hub.start();
        if (!isMounted) {
          hub.stop();
          return;
        }

        isConnected = true;
        console.log("[SignalR Auctions] Connected successfully to AuctionsHub.");

        // 1. Subscribe to Live Bid Placed events
        unsubscribeBidPlaced = hub.onBidPlaced((event) => {
          const targetId = event.auctionId || (event as any).AuctionId;
          const newPrice = event.newPrice || (event as any).NewPrice;

          if (!targetId || typeof newPrice !== "number") return;

          console.log(`[SignalR Auctions] Live bid update received for ${targetId}: ${newPrice}`);

          // Reactively update the cached detailed view immediately
          queryClient.setQueryData<AuctionSummary | null>(
            auctionKeys.detail(targetId),
            (old) => {
              if (!old) return old;

              const currentPrice = old.pricing.currentBid ?? old.pricing.startingPrice;
              // Guard against race conditions or delayed events
              if (newPrice <= currentPrice) {
                return old;
              }

              return {
                ...old,
                pricing: {
                  ...old.pricing,
                  currentBid: newPrice,
                  bidCount: old.pricing.bidCount + 1,
                },
              };
            }
          );

          // Trigger soft invalidation to fetch updated bid history entries in the background
          void queryClient.invalidateQueries({
            queryKey: auctionKeys.detail(targetId),
          });

          // Invalidate list endpoints so price updates reflect on browse pages
          void queryClient.invalidateQueries({
            queryKey: auctionKeys.lists(),
          });
        });

        // 2. Subscribe to Live Status Changed events
        unsubscribeStatusChanged = hub.onStatusChanged((event) => {
          const targetId = event.auctionId || (event as any).AuctionId;
          const rawStatus = event.status || (event as any).Status;

          if (!targetId || !rawStatus) return;

          console.log(`[SignalR Auctions] Live status change received for ${targetId}: ${rawStatus}`);
          const mappedStatus = mapBackendStatusToFrontend(rawStatus);

          // Update local details cache immediately
          queryClient.setQueryData<AuctionSummary | null>(
            auctionKeys.detail(targetId),
            (old) => {
              if (!old) return old;
              return {
                ...old,
                status: mappedStatus,
              };
            }
          );

          // Force fresh fetch for lists and details
          void queryClient.invalidateQueries({
            queryKey: auctionKeys.detail(targetId),
          });
          void queryClient.invalidateQueries({
            queryKey: auctionKeys.lists(),
          });
        });

        // 3. Subscribe to Live Auction Created events
        unsubscribeAuctionCreated = hub.onAuctionCreated((event) => {
          const targetId = event.auctionId || (event as any).AuctionId;
          console.log(`[SignalR Auctions] Live new auction created: ${targetId}`);

          // Invalidate list endpoints so the new auction appears in the feed
          void queryClient.invalidateQueries({
            queryKey: auctionKeys.lists(),
          });
        });

      } catch (err) {
        if (!isMounted) return;

        if (currentRetry < maxRetries) {
          const nextDelay = Math.pow(2, currentRetry) * 2000 + 5000; // 7s, 9s, 13s backoff
          console.warn(
            `[SignalR Auctions] Connection failed. Retrying in ${nextDelay / 1000}s... (${currentRetry + 1}/${maxRetries})`
          );
          retryTimeoutId = setTimeout(() => {
            currentRetry++;
            startHub();
          }, nextDelay);
        } else {
          console.warn(
            `[SignalR Auctions] Max initial connection attempts reached. Real-time bidding updates disabled.`
          );
        }
      }
    };

    startHub();

    return () => {
      isMounted = false;
      if (unsubscribeBidPlaced) unsubscribeBidPlaced();
      if (unsubscribeStatusChanged) unsubscribeStatusChanged();
      if (unsubscribeAuctionCreated) unsubscribeAuctionCreated();
      if (retryTimeoutId) clearTimeout(retryTimeoutId);
      
      if (isConnected) {
        hub.stop();
      }
    };
  }, [queryClient]);
}
