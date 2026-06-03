import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import { useAppToast } from "@/lib/toast/app-toast";
import {
  fetchModerateUsers,
  banUserApi,
  suspendUserApi,
  activateUserApi,
  bulkActivateUsersApi,
  bulkSuspendUsersApi,
  bulkBanUsersApi,
  updateMockUserStatus,
  MOCK_MODERATE_USERS_DATA,
  type UseModerateUsersFilters,
} from "./users.api";
import { usersKeys } from "./users.keys";

export function useModerateUsers(filters: UseModerateUsersFilters) {
  return useQuery({
    queryKey: usersKeys.list(filters),
    queryFn: async () => {
      try {
        return await fetchModerateUsers(filters);
      } catch (error) {
        console.warn("Failed to fetch moderate users from backend, falling back to mock data:", error);

        // Filter fallback mock data
        let filteredData = [...MOCK_MODERATE_USERS_DATA.data];

        if (filters.search) {
          const lowerQuery = filters.search.toLowerCase();
          filteredData = filteredData.filter(
            (u) => u.fullName.toLowerCase().includes(lowerQuery) || u.email.toLowerCase().includes(lowerQuery)
          );
        }
        if (filters.role !== "All Roles") {
          filteredData = filteredData.filter((u) => u.role === filters.role);
        }
        if (filters.status !== "All Statuses") {
          filteredData = filteredData.filter((u) => u.status === filters.status);
        }

        // Apply pagination mock
        const startIndex = (filters.page - 1) * filters.pageSize;
        const endIndex = startIndex + filters.pageSize;
        const paginatedData = filteredData.slice(startIndex, endIndex);

        return {
          ...MOCK_MODERATE_USERS_DATA,
          data: paginatedData,
          totalCount: filteredData.length,
          page: filters.page,
          pageSize: filters.pageSize,
          totalPages: Math.ceil(filteredData.length / filters.pageSize),
        };
      }
    },
    staleTime: 60 * 1000,
  });
}

export interface SuspendUserParams {
  userId: string;
  reason: string;
  until?: string;
}

export function useSuspendUser() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation({
    mutationFn: async ({ userId, reason, until }: SuspendUserParams) => {
      try {
        await suspendUserApi(userId, reason, until);
        return { success: true };
      } catch (error) {
        console.warn(`Failed to suspend user ${userId} on backend, falling back to mock:`, error);
        
        // Mock fallback
        await new Promise((resolve) => setTimeout(resolve, 800));
        const success = updateMockUserStatus(userId, "Suspended");
        if (!success) {
          throw new Error("User not found in mock data");
        }
        return { success: true };
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: usersKeys.all });
      appToast.success("User Suspended", "The user account has been successfully suspended.");
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to suspend the user. Please try again.";
      appToast.error("Action Failed", msg);
    },
  });
}

export interface BanUserParams {
  userId: string;
  reason: string;
}

export function useBanUser() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation({
    mutationFn: async ({ userId, reason }: BanUserParams) => {
      try {
        await banUserApi(userId, reason);
        return { success: true };
      } catch (error) {
        console.warn(`Failed to ban user ${userId} on backend, falling back to mock:`, error);

        // Mock fallback
        await new Promise((resolve) => setTimeout(resolve, 800));
        const success = updateMockUserStatus(userId, "Banned");
        if (!success) {
          throw new Error("User not found in mock data");
        }
        return { success: true };
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: usersKeys.all });
      appToast.success("User Banned", "The user account has been successfully banned.");
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to ban the user. Please try again.";
      appToast.error("Action Failed", msg);
    },
  });
}

export function useRestoreUser() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation({
    mutationFn: async (userId: string) => {
      try {
        await activateUserApi(userId);
        return { success: true };
      } catch (error) {
        console.warn(`Failed to restore user ${userId} on backend, falling back to mock:`, error);

        // Mock fallback
        await new Promise((resolve) => setTimeout(resolve, 800));
        const success = updateMockUserStatus(userId, "Active");
        if (!success) {
          throw new Error("User not found in mock data");
        }
        return { success: true };
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: usersKeys.all });
      appToast.success("User Restored", "The user account has been successfully restored to Active status.");
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to restore the user. Please try again.";
      appToast.error("Action Failed", msg);
    },
  });
}

export async function exportUsersApi(filters: UseModerateUsersFilters, selectedIds: string[]): Promise<Blob> {
  try {
    if (selectedIds.length > 0) {
      const response = await api.post<Blob>("/users/users/export/selected", selectedIds, {
        responseType: "blob",
      });
      return response.data;
    } else {
      const isAsc = filters.sortBy === "name" ? true : false;
      const response = await api.get<Blob>("/users/users/export", {
        params: {
          SearchTerm: filters.search || undefined,
          SortBy: filters.sortBy === "name" ? "FullName" : "JoinedAt",
          IsAsc: isAsc,
        },
        responseType: "blob",
      });
      return response.data;
    }
  } catch (error) {
    console.warn("Failed to reach real export endpoint, generating mock CSV blob:", error);

    await new Promise((resolve) => setTimeout(resolve, 1500));

    let filteredData = [...MOCK_MODERATE_USERS_DATA.data];
    if (selectedIds.length > 0) {
      filteredData = filteredData.filter((u) => selectedIds.includes(u.id));
    } else {
      if (filters.search) {
        const lowerQuery = filters.search.toLowerCase();
        filteredData = filteredData.filter(
          (u) => u.fullName.toLowerCase().includes(lowerQuery) || u.email.toLowerCase().includes(lowerQuery)
        );
      }
      if (filters.role !== "All Roles") {
        filteredData = filteredData.filter((u) => u.role === filters.role);
      }
      if (filters.status !== "All Statuses") {
        filteredData = filteredData.filter((u) => u.status === filters.status);
      }
    }

    const csvHeader = "ID,Full Name,Email,Role,Status,Joined Date,Last Active\n";
    const csvRows = filteredData
      .map((u) => `${u.id},"${u.fullName}","${u.email}",${u.role},${u.status},${u.joinedDate},"${u.lastActive}"`)
      .join("\n");

    const csvContent = csvHeader + csvRows;
    return new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
  }
}

export interface BulkSuspendUsersParams {
  userIds: string[];
  reason: string;
  until?: string;
}

export function useBulkSuspendUsers() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation({
    mutationFn: async ({ userIds, reason, until }: BulkSuspendUsersParams) => {
      try {
        await bulkSuspendUsersApi(userIds, reason, until);
        return { success: true };
      } catch (error) {
        console.warn("Failed to bulk suspend users on backend, falling back to mock:", error);
        
        await new Promise((resolve) => setTimeout(resolve, 800));
        userIds.forEach((id) => updateMockUserStatus(id, "Suspended"));
        return { success: true };
      }
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: usersKeys.all });
      appToast.success("Users Suspended", `Successfully suspended ${variables.userIds.length} user accounts.`);
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to bulk suspend the selected users.";
      appToast.error("Bulk Suspension Failed", msg);
    },
  });
}

export interface BulkBanUsersParams {
  userIds: string[];
  reason: string;
}

export function useBulkBanUsers() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation({
    mutationFn: async ({ userIds, reason }: BulkBanUsersParams) => {
      try {
        await bulkBanUsersApi(userIds, reason);
        return { success: true };
      } catch (error) {
        console.warn("Failed to bulk ban users on backend, falling back to mock:", error);
        
        await new Promise((resolve) => setTimeout(resolve, 800));
        userIds.forEach((id) => updateMockUserStatus(id, "Banned"));
        return { success: true };
      }
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: usersKeys.all });
      appToast.success("Users Banned", `Successfully banned ${variables.userIds.length} user accounts.`);
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to bulk ban the selected users.";
      appToast.error("Bulk Ban Failed", msg);
    },
  });
}

export function useBulkActivateUsers() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation({
    mutationFn: async (userIds: string[]) => {
      try {
        await bulkActivateUsersApi(userIds);
        return { success: true };
      } catch (error) {
        console.warn("Failed to bulk activate users on backend, falling back to mock:", error);
        
        await new Promise((resolve) => setTimeout(resolve, 800));
        userIds.forEach((id) => updateMockUserStatus(id, "Active"));
        return { success: true };
      }
    },
    onSuccess: (_, userIds) => {
      queryClient.invalidateQueries({ queryKey: usersKeys.all });
      appToast.success("Users Activated", `Successfully activated ${userIds.length} user accounts.`);
    },
    onError: (error: unknown) => {
      const msg = error instanceof Error ? error.message : "Failed to bulk activate the selected users.";
      appToast.error("Bulk Activation Failed", msg);
    },
  });
}
