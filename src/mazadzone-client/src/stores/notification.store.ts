/**
 * @deprecated
 *
 * This file is a compatibility shim.
 *
 * The notification store has been moved to:
 *   src/features/notifications/store/notification.store.ts
 *
 * Import from the feature instead:
 *   import { useNotificationStore } from '@/features/notifications';
 */

export {
  useNotificationStore,
} from "@/features/notifications/store/notification.store";

export type {
  Notification,
  NotificationType,
} from "@/features/notifications/types/notification.types";
