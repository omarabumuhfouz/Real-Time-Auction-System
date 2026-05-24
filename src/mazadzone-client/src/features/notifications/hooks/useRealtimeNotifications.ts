"use client";

import { useEffect } from "react";
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
 * It manages its own initial connection retry lifecycle safely, preventing
 * memory leaks by clearing pending retry timeouts upon unmount.
 *
 * @param userId - The authenticated user's ID. Pass empty string / undefined
 *   when not authenticated to skip the connection.
 */
export function useRealtimeNotifications(userId: string | undefined): void {
  const addNotification = useNotificationStore((state) => state.addNotification);
  const appToast = useAppToast();

  useEffect(() => {
    if (!userId) return;

    // Decoupled token factory resolver passed directly to the client constructor
    const hub = createNotificationsHubClient(() => useAuthStore.getState().accessToken ?? "");
    let isMounted = true;
    let unsubscribeFn: (() => void) | undefined;
    let retryTimeoutId: NodeJS.Timeout | undefined;
    let currentRetry = 0;
    const maxRetries = 3;

    const startHub = async () => {
      // Respect the central feature flag dynamically
      if (!APP_CONFIG.enableRealtime) {
        return;
      }

      try {
        await hub.start();
        if (!isMounted) {
          hub.stop();
          return;
        }

        console.log("[SignalR Notifications] Connected successfully.");

        // Subscribe to live event streams
        unsubscribeFn = hub.onNotificationReceived((event) => {
          const notification: Notification = {
            id: event.id || Math.random().toString(),
            type: event.type,
            title: event.title,
            message: event.message || "",
            link: event.href || "",
            isRead: false,
            createdAt: new Date().toISOString(),
          };

          // 1. Save in the in-app notification center store
          addNotification(notification);

          // 2. Display a rich toast notification to the user
          const feedbackType = getFeedbackType(event.type);
          appToast.show(feedbackType, event.title, event.message);
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
      hub.stop();
    };
  }, [userId, addNotification, appToast]);
}
