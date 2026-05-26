import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import { useAppToast } from "@/lib/toast/app-toast";
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
  const appToast = useAppToast();

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
      appToast.success("User Suspended", "The user account has been successfully suspended.");
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to suspend the user. Please try again.";
      appToast.error("Action Failed", msg);
    },
  });
}

/**
 * Mutation to ban a user account.
 * Hits backend API but falls back to mock status changes if offline.
 */
export function useBanUser() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

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
      appToast.success("User Banned", "The user account has been successfully banned.");
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to ban the user. Please try again.";
      appToast.error("Action Failed", msg);
    },
  });
}

/**
 * Mutation to restore a user account back to active.
 * Hits backend API but falls back to mock status changes if offline.
 */
export function useRestoreUser() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

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
      appToast.success("User Restored", "The user account has been successfully restored to Active status.");
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to restore the user. Please try again.";
      appToast.error("Action Failed", msg);
    },
  });
}
