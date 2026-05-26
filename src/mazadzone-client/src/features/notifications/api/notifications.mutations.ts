import { useMutation, useQueryClient } from "@tanstack/react-query";
import { notificationsApi } from "./notifications.api";
import { NOTIFICATION_KEYS } from "./notifications.queries";
import { useNotificationStore } from "../store/notification.store";

export const useMarkAsRead = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => notificationsApi.markAsRead(id),
    onSuccess: () => {
      // Invalidate both the list and the unread count
      queryClient.invalidateQueries({ queryKey: NOTIFICATION_KEYS.all });
    },
  });
};

export const useMarkAllAsRead = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => {
      // Extract active unread notification IDs from Zustand store to perform parallel API reads
      const unreadIds = useNotificationStore
        .getState()
        .notifications.filter((n) => !n.isRead)
        .map((n) => n.id);

      return notificationsApi.markAllAsRead(unreadIds);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: NOTIFICATION_KEYS.all });
    },
  });
};
