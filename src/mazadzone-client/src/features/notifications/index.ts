/**
 * Public API for the notifications feature.
 *
 * Import from here, not from internal paths:
 *   import { NotificationsPage } from '@/features/notifications';
 */

// -- Components --
export { NotificationPopover } from "./components/NotificationPopover";
export { NotificationsPage } from "./components/NotificationsPage";

// -- Hooks --
export { useRealtimeNotifications } from "./hooks/useRealtimeNotifications";
export { useNotificationSync } from "./hooks/useNotificationSync";

// -- Store --
export { useNotificationStore } from "./store/notification.store";

// -- API --
export * from "./api";

// -- Types --
export type { Notification, NotificationType, NotificationResponse } from "./types/notification.types";
