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
 */

import { create } from "zustand";
import type { Notification } from "../types/notification.types";

interface NotificationState {
  /** All in-app notifications for the current user. */
  notifications: Notification[];
}

interface NotificationActions {

  setNotifications: (notifications: Notification[]) => void;
  addNotification: (notification: Notification) => void;
  markAsRead: (id: string) => void;
  markAllAsRead: () => void;
  removeNotification: (id: string) => void;
  clearAll: () => void;
  getUnreadCount: () => number;
}

type NotificationStore = NotificationState & NotificationActions;

export const useNotificationStore = create<NotificationStore>()((set, get) => ({
  notifications: [],

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

  clearAll: () => set({ notifications: [] }),

  getUnreadCount: () => get().notifications.filter((n) => !n.isRead).length,
}));
