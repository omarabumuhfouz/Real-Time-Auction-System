import type { HubConnection } from "@microsoft/signalr";
import {
  createHubConnection,
  startConnection,
  stopConnection,
} from "./connection-factory";
import type { NotificationType } from "@/features/notifications";

// --- Event Types (server → client) ------------------------------

export interface NotificationReceivedEvent {
  id: string;
  type: NotificationType;
  title: string;
  message?: string;
  href?: string;
  metadata?: Record<string, unknown>;
}

// --- Notifications Hub Client ------------------------------------

/**
 * Typed client for the Notifications SignalR hub.
 *
 * Usage:
 * ```ts
 * const hub = createNotificationsHubClient();
 * await hub.start();
 * hub.onNotificationReceived((event) => console.log(event));
 * ```
 */
export function createNotificationsHubClient(accessTokenFactory?: () => string | Promise<string>) {
  const connection: HubConnection = createHubConnection("/notifications", accessTokenFactory);

  return {
    /** The underlying SignalR connection (for advanced use) */
    connection,

    // -- Lifecycle ------------------------------------------
    start: () => startConnection(connection),
    stop: () => stopConnection(connection),

    // -- Subscribe to server events -------------------------
    onNotificationReceived: (callback: (event: NotificationReceivedEvent) => void) => {
      connection.on("NotificationReceived", callback);
      return () => connection.off("NotificationReceived", callback);
    },
  };
}

export type NotificationsHubClient = ReturnType<typeof createNotificationsHubClient>;
