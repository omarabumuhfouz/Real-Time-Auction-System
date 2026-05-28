/**
 * notification.store.ts
 *
 * In-app notification center store for MazadZone.
 *
 * Responsibilities:
 *  - Holds the list of in-app notifications shown in the header bell
 *    (e.g. "You were outbid", "Auction won", "Payment failed").
 *  - Tracks unread count and read/unread status.
 *  - Provides actions for marking as read and clearing notifications.
 *  - Coordinates optimistic updates (SignalR) with server hydration
 *    via an internal `_optimisticPending` guard counter.
 *  - Shows toast notifications when new unread items arrive from server refetch.
 */

import { create } from "zustand";
import type { Notification } from "../types/notification.types";
import { appToast } from "@/lib/toast/app-toast";

interface NotificationState {
  /** All in-app notifications for the current user. */
  notifications: Notification[];
  /**
   * The authoritative unread count for the header badge.
   * Updated synchronously on SignalR events and mark-as-read actions.
   * Hydrated from the server via the header's useGetUnreadCount query.
   */
  unreadCount: number;
  /**
   * Internal guard counter.
   * Incremented when SignalR fires an optimistic update.
   * Checked (and decremented) by server-hydration useEffects to avoid
   * overwriting the fresh optimistic value with stale server data.
   */
  _optimisticPending: number;
  /** Whether the store has been hydrated at least once from the server. */
  _hydrated: boolean;
}

interface NotificationActions {
  setNotifications: (notifications: Notification[]) => void;
  addNotification: (notification: Notification) => void;
  markAsRead: (id: string) => void;
  markAllAsRead: () => void;
  removeNotification: (id: string) => void;
  clearAll: () => void;
  /** Replace the badge count with the authoritative server value. */
  setUnreadCount: (count: number) => void;
  /** Increment the badge count by 1 when a live notification arrives. */
  incrementUnreadCount: () => void;
  /** Decrement the badge count by 1 when a notification is marked as read. */
  decrementUnreadCount: () => void;
  /** Zero the badge count after marking all as read. */
  resetUnreadCount: () => void;
  /**
   * Signal that an optimistic update just happened.
   * Call this after addNotification + incrementUnreadCount from SignalR.
   */
  _markOptimistic: () => void;
  /**
   * Check and consume the optimistic guard.
   * Returns `true` if there was a pending optimistic update (caller should
   * skip server overwrite). Returns `false` if none pending (safe to hydrate).
   */
  _consumeOptimistic: () => boolean;
  /**
   * Optimistic-aware sync from server data.
   * - If optimistic pending → merges (preserves live items).
   * - Otherwise → replaces the notification list entirely.
   * - Detects NEW unread notifications and shows toasts for them.
   * - Does NOT update unreadCount (the header's useGetUnreadCount is the authority).
   */
  syncFromServer: (serverItems: Notification[]) => void;
}

type NotificationStore = NotificationState & NotificationActions;

export const useNotificationStore = create<NotificationStore>()((set, get) => ({
  notifications: [],
  unreadCount: 0,
  _optimisticPending: 0,
  _hydrated: false,

  setNotifications: (notifications) => set({ notifications }),

  addNotification: (notification) =>
    set((state) => ({
      notifications: [notification, ...state.notifications],
    })),

  markAsRead: (id) =>
    set((state) => ({
      notifications: state.notifications.map((n) =>
        n.id === id ? { ...n, isRead: true } : n
      ),
    })),

  markAllAsRead: () =>
    set((state) => ({
      notifications: state.notifications.map((n) => ({ ...n, isRead: true })),
    })),

  removeNotification: (id) =>
    set((state) => ({
      notifications: state.notifications.filter((n) => n.id !== id),
    })),

  clearAll: () => set({ notifications: [], unreadCount: 0, _optimisticPending: 0, _hydrated: false }),

  setUnreadCount: (count) => set({ unreadCount: Math.max(0, count) }),

  incrementUnreadCount: () =>
    set((state) => ({ unreadCount: state.unreadCount + 1 })),

  decrementUnreadCount: () =>
    set((state) => ({ unreadCount: Math.max(0, state.unreadCount - 1) })),

  resetUnreadCount: () => set({ unreadCount: 0 }),

  _markOptimistic: () =>
    set((state) => ({ _optimisticPending: state._optimisticPending + 1 })),

  _consumeOptimistic: () => {
    const pending = get()._optimisticPending;
    if (pending > 0) {
      set({ _optimisticPending: pending - 1 });
      return true;
    }
    return false;
  },

  syncFromServer: (serverItems) => {
    const { _optimisticPending, notifications: existing, _hydrated } = get();

    if (_optimisticPending > 0) {
      // Merge: keep existing store items, append any server items not already present
      const existingIds = new Set(existing.map((n) => n.id));
      const newFromServer = serverItems.filter((n) => !existingIds.has(n.id));
      if (newFromServer.length > 0) {
        set({ notifications: [...existing, ...newFromServer] });
      }
      // Don't touch unreadCount — optimistic value is authoritative
    } else {
      // Detect genuinely NEW unread notifications (only after initial hydration)
      if (_hydrated && existing.length > 0) {
        const existingIds = new Set(existing.map((n) => n.id));
        const newUnread = serverItems.filter(
          (n) => !n.isRead && !existingIds.has(n.id)
        );

        // Show a toast for each new unread notification
        for (const n of newUnread) {
          appToast.info(n.title, n.message);
        }
      }

      // Replace the notification list (but NOT unreadCount — the header handles that)
      set({ notifications: serverItems, _hydrated: true });
    }
  },
}));
