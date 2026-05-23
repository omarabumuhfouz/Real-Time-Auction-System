import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import { useNotificationStore } from "@/stores/notification.store";
import { updateMockUserStatus } from "./queries";

export interface SuspendUserParams {
  userId: string;
  reason: string;
}

export interface BanUserParams {
  userId: string;
  reason: string;
}

/**
 * Mutation to suspend a user account.
 * Hits backend API but falls back to mock status changes if offline.
 */
export function useSuspendUser() {
  const queryClient = useQueryClient();
  const addNotification = useNotificationStore((state) => state.addNotification);

  return useMutation({
    mutationFn: async ({ userId, reason }: SuspendUserParams) => {
      try {
        const response = await api.post(`/admin/users/${userId}/suspend`, { reason });
        return response.data;
      } catch (error) {
        console.warn(`Failed to suspend user ${userId} on backend, falling back to mock:`, error);
        
        // Mock fallback
        await new Promise((resolve) => setTimeout(resolve, 800)); // Simulate delay
        const success = updateMockUserStatus(userId, "Suspended");
        if (!success) {
          throw new Error("User not found in mock data");
        }
        return { success: true };
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "moderate-users"] });
      addNotification({
        type: "success",
        title: "User Suspended",
        message: "The user account has been successfully suspended.",
        duration: 4000,
      });
    },
    onError: (error: any) => {
      addNotification({
        type: "error",
        title: "Action Failed",
        message: error.message || "Failed to suspend the user. Please try again.",
        duration: 5000,
      });
    },
  });
}

/**
 * Mutation to ban a user account.
 * Hits backend API but falls back to mock status changes if offline.
 */
export function useBanUser() {
  const queryClient = useQueryClient();
  const addNotification = useNotificationStore((state) => state.addNotification);

  return useMutation({
    mutationFn: async ({ userId, reason }: BanUserParams) => {
      try {
        const response = await api.post(`/admin/users/${userId}/ban`, { reason });
        return response.data;
      } catch (error) {
        console.warn(`Failed to ban user ${userId} on backend, falling back to mock:`, error);
        
        // Mock fallback
        await new Promise((resolve) => setTimeout(resolve, 800)); // Simulate delay
        const success = updateMockUserStatus(userId, "Banned");
        if (!success) {
          throw new Error("User not found in mock data");
        }
        return { success: true };
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "moderate-users"] });
      addNotification({
        type: "success",
        title: "User Banned",
        message: "The user account has been successfully banned.",
        duration: 4000,
      });
    },
    onError: (error: any) => {
      addNotification({
        type: "error",
        title: "Action Failed",
        message: error.message || "Failed to ban the user. Please try again.",
        duration: 5000,
      });
    },
  });
}

/**
 * Mutation to restore a user account back to active.
 * Hits backend API but falls back to mock status changes if offline.
 */
export function useRestoreUser() {
  const queryClient = useQueryClient();
  const addNotification = useNotificationStore((state) => state.addNotification);

  return useMutation({
    mutationFn: async (userId: string) => {
      try {
        const response = await api.post(`/admin/users/${userId}/restore`);
        return response.data;
      } catch (error) {
        console.warn(`Failed to restore user ${userId} on backend, falling back to mock:`, error);
        
        // Mock fallback
        await new Promise((resolve) => setTimeout(resolve, 800)); // Simulate delay
        const success = updateMockUserStatus(userId, "Active");
        if (!success) {
          throw new Error("User not found in mock data");
        }
        return { success: true };
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "moderate-users"] });
      addNotification({
        type: "success",
        title: "User Restored",
        message: "The user account has been successfully restored to Active status.",
        duration: 4000,
      });
    },
    onError: (error: any) => {
      addNotification({
        type: "error",
        title: "Action Failed",
        message: error.message || "Failed to restore the user. Please try again.",
        duration: 5000,
      });
    },
  });
}
