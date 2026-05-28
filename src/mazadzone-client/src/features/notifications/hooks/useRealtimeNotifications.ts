"use client";

import { useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { createNotificationsHubClient } from "@/lib/signalr";
import { useNotificationStore } from "../store/notification.store";
import { useAppToast } from "@/lib/toast/app-toast";
import { useAuthStore } from "@/stores/auth.store";
import { APP_CONFIG } from "@/config/app.config";
import type { Notification, NotificationType } from "../types/notification.types";

/**
 * Maps a domain NotificationType to a semantic FeedbackType for toast coloring.
 */
function getFeedbackType(type: NotificationType): "success" | "error" | "warning" | "info" {
  switch (type) {
    case "bid_accepted":
    case "bid_placed":
    case "auction_won":
    case "order_received":
    case "payment_authorized":
    case "seller_approved":
    case "account_verified":
      return "success";
    case "outbid":
    case "auction_ending":
    case "dispute_opened":
      return "warning";
    case "payment_failed":
    case "auction_cancelled":
      return "error";
    case "order_shipped":
    case "dispute_resolved":
    case "feedback_received":
    case "new_message":
    case "general":
    default:
      return "info";
  }
}

/**
 * Hook to listen for real-time notifications from the ASP.NET Core SignalR backend.
 *
 * It subscribes to the notifications hub, pushes incoming events into the
 * local Zustand notification store (which renders inside the header bell),
 * and triggers a toast message.
 *
 * Optimistic updates are guarded via `_markOptimistic()` so that stale
 * server refetches (triggered by the delayed invalidation) cannot overwrite
 * the freshly-set Zustand values.
 *
 * @param userId - The authenticated user's ID. Pass empty string / undefined
 *   when not authenticated to skip the connection.
 */
export function useRealtimeNotifications(userId: string | undefined): void {
  const queryClient = useQueryClient();
  const addNotification = useNotificationStore((state) => state.addNotification);
  const incrementUnreadCount = useNotificationStore((state) => state.incrementUnreadCount);
  const markOptimistic = useNotificationStore((state) => state._markOptimistic);
  const appToast = useAppToast();

  useEffect(() => {
    if (!userId) return;

    // Decoupled token factory resolver passed directly to the client constructor
    const hub = createNotificationsHubClient(() => useAuthStore.getState().accessToken ?? "");
    let isMounted = true;
    let unsubscribeFn: (() => void) | undefined;
    let retryTimeoutId: NodeJS.Timeout | undefined;
    let delayedInvalidationId: NodeJS.Timeout | undefined;
    let currentRetry = 0;
    const maxRetries = 3;

    const startHub = async () => {
      // Respect the central feature flag dynamically
      if (!APP_CONFIG.enableRealtime) {
        console.log("[SignalR Notifications] Realtime disabled by feature flag.");
        return;
      }

      try {
        const token = useAuthStore.getState().accessToken;
        console.log(
          `[SignalR Notifications] Attempting connection for userId=${userId}, hasToken=${!!token}`
        );

        await hub.start();
        if (!isMounted) {
          hub.stop();
          return;
        }

        console.log("[SignalR Notifications] Connected successfully. Listening for ReceiveNotification events...");

        // Subscribe to live event streams
        unsubscribeFn = hub.onNotificationReceived((event: any) => {
          console.log("[SignalR Notifications] Live notification received:", event);

          const titleText = event.title || "Notification";
          const messageText = event.message || event.Message || "";

          // Parse type dynamically from title/message
          let type: NotificationType = "general";
          const titleLower = titleText.toLowerCase();
          const messageLower = messageText.toLowerCase();

          if (titleLower.includes("outbid") || messageLower.includes("outbid")) {
            type = "outbid";
          } else if (titleLower.includes("won") || titleLower.includes("win") || messageLower.includes("won") || messageLower.includes("win")) {
            type = "auction_won";
          } else if (titleLower.includes("ending") || titleLower.includes("end") || messageLower.includes("ending")) {
            type = "auction_ending";
          } else if (titleLower.includes("shipped") || messageLower.includes("shipped")) {
            type = "order_shipped";
          } else if (titleLower.includes("received") || titleLower.includes("delivered") || messageLower.includes("received") || messageLower.includes("delivered")) {
            type = "order_received";
          } else if (titleLower.includes("payment failed") || titleLower.includes("failed payment") || messageLower.includes("payment failed")) {
            type = "payment_failed";
          } else if (titleLower.includes("payment") || titleLower.includes("authorized") || messageLower.includes("payment")) {
            type = "payment_authorized";
          } else if (titleLower.includes("dispute") && (titleLower.includes("opened") || messageLower.includes("opened"))) {
            type = "dispute_opened";
          } else if (titleLower.includes("dispute") && (titleLower.includes("resolved") || messageLower.includes("resolved"))) {
            type = "dispute_resolved";
          } else if (titleLower.includes("feedback") || messageLower.includes("feedback")) {
            type = "feedback_received";
          } else if (titleLower.includes("verified") || messageLower.includes("verified")) {
            type = "account_verified";
          } else if (titleLower.includes("approved") || titleLower.includes("become seller") || messageLower.includes("approved")) {
            type = "seller_approved";
          } else if (titleLower.includes("message") || messageLower.includes("message")) {
            type = "new_message";
          } else if (titleLower.includes("cancel") || messageLower.includes("cancel")) {
            type = "auction_cancelled";
          }

          // Parse href dynamically from UUID in message/title
          let link = "";
          const idRegex = /[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}/;
          const match = messageText.match(idRegex) || titleText.match(idRegex);
          if (match) {
            const uuid = match[0];
            if (type.startsWith("auction_") || type === "outbid") {
              link = `/auctions/${uuid}`;
            } else if (type.startsWith("order_") || type.startsWith("payment_")) {
              link = `/orders/${uuid}`;
            }
          }

          const notification: Notification = {
            id: event.id || Math.random().toString(),
            type,
            title: titleText,
            message: messageText,
            link: link || undefined,
            isRead: false,
            createdAt: new Date().toISOString(),
          };

          // 1. Save notification in Zustand store (renders it in the open popover list instantly)
          addNotification(notification);

          // 2. Increment the badge counter synchronously — zero-latency re-render
          incrementUnreadCount();

          // 3. Mark optimistic so hydration useEffects don't overwrite with stale data
          markOptimistic();

          // 4. Display a rich toast notification to the user
          const feedbackType = getFeedbackType(type);
          appToast.show(feedbackType, titleText, messageText);

          // 5. Delayed background refetch (3s) to silently align with database
          //    after the backend has had time to commit the write.
          if (delayedInvalidationId) {
            clearTimeout(delayedInvalidationId);
          }
          delayedInvalidationId = setTimeout(() => {
            if (isMounted) {
              void queryClient.invalidateQueries({ queryKey: ["notifications"] });
            }
          }, 3000);
        });
      } catch (err) {
        if (!isMounted) return;

        // Custom lifecycle-safe retry logic for initial connection
        if (currentRetry < maxRetries) {
          const nextDelay = Math.pow(2, currentRetry) * 2000 + 5000; // 7s, 9s, 13s backoff
          console.warn(
            `[SignalR Notifications] Connection failed. Retrying in ${nextDelay / 1000}s... (${currentRetry + 1}/${maxRetries})`
          );
          retryTimeoutId = setTimeout(() => {
            currentRetry++;
            startHub();
          }, nextDelay);
        } else {
          console.warn(
            `[SignalR Notifications] Max initial connection attempts reached. Real-time notifications disabled.`
          );
        }
      }
    };

    startHub();

    return () => {
      isMounted = false;
      if (unsubscribeFn) {
        unsubscribeFn();
      }
      if (retryTimeoutId) {
        clearTimeout(retryTimeoutId);
      }
      if (delayedInvalidationId) {
        clearTimeout(delayedInvalidationId);
      }
      hub.stop();
    };
  }, [userId, addNotification, incrementUnreadCount, markOptimistic, appToast, queryClient]);
}

