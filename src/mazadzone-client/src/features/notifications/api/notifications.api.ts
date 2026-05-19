import { api } from "@/lib/api/client";
import { NotificationResponse } from "../types/notification.types";
import {
  getMockNotifications,
  updateMockNotificationReadStatus,
  updateAllMockNotificationsReadStatus,
} from "../testing/mock-notifications";

// Simulated network delay (remove/adjust as needed)
const MOCK_DELAY_MS = 300;

const simulateDelay = (): Promise<void> =>
  new Promise((resolve) => setTimeout(resolve, MOCK_DELAY_MS));

export const notificationsApi = {
  getNotifications: async (
    userId: string,
    page: number,
    pageSize: number
  ): Promise<NotificationResponse> => {
    /**
     * --- REAL API CALL (Uncomment when backend is ready) ---
     * const response = await api.get<NotificationResponse>("/notifications", {
     *   params: { userId, pageNumber: page, pageSize },
     * });
     * return response.data;
     */

    // --- MOCK IMPLEMENTATION ---
    await simulateDelay();

    const allNotifications = getMockNotifications();
    const totalCount = allNotifications.length;
    const totalPages = Math.ceil(totalCount / pageSize);
    const startIndex = (page - 1) * pageSize;
    const items = allNotifications.slice(startIndex, startIndex + pageSize);

    return {
      items,
      totalCount,
      pageSize,
      pageNumber: page,
      totalPages,
    };
  },

  markAsRead: async (id: string): Promise<void> => {
    /**
     * --- REAL API CALL (Uncomment when backend is ready) ---
     * await api.patch(`/notifications/${id}/read`);
     */

    // --- MOCK IMPLEMENTATION ---
    await simulateDelay();
    updateMockNotificationReadStatus(id, true);
  },

  markAllAsRead: async (): Promise<void> => {
    /**
     * --- REAL API CALL (Uncomment when backend is ready) ---
     * await api.post("/notifications/read-all");
     */

    // --- MOCK IMPLEMENTATION ---
    await simulateDelay();
    updateAllMockNotificationsReadStatus();
  },

  getUnreadCount: async (userId: string): Promise<number> => {
    /**
     * --- REAL API CALL (Uncomment when backend is ready) ---
     * const response = await api.get<{ count: number }>("/notifications/unread-count", {
     *   params: { userId },
     * });
     * return response.data.count;
     */

    // --- MOCK IMPLEMENTATION ---
    await simulateDelay();
    const unreadCount = getMockNotifications().filter((n) => !n.isRead).length;
    return unreadCount;
  },
};

