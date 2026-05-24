"use client";

import React, { useState, useMemo } from "react";
import { useUrlFilters } from "@/hooks/use-url-filters";
import { Info } from "lucide-react";
import { ModerateUsersFilters } from "./ModerateUsersFilters";
import { ModerateUsersTable } from "./ModerateUsersTable";
import { useModerateUsers, type UseModerateUsersFilters, exportUsersApi } from "../../api/queries";
import { useAppToast } from "@/lib/toast/app-toast";
import type { ModerateUserRole, ModerateUserStatus } from "../../types/admin.types";

export function ModerateUsersPage() {
  const { searchParams, setFilters } = useUrlFilters<UseModerateUsersFilters>();

  const filters = useMemo<UseModerateUsersFilters>(() => {
    return {
      search: searchParams.get("search") || "",
      role: (searchParams.get("role") as ModerateUserRole | "All Roles") || "All Roles",
      status: (searchParams.get("status") as ModerateUserStatus | "All Statuses") || "All Statuses",
      sortBy: searchParams.get("sortBy") || "dateJoined",
      page: Number(searchParams.get("page")) || 1,
      pageSize: Number(searchParams.get("pageSize")) || 15,
    };
  }, [searchParams]);

  const [selectedIds, setSelectedIds] = useState<string[]>([]);
  const [isExporting, setIsExporting] = useState(false);
  const appToast = useAppToast();

  const { data, isLoading } = useModerateUsers(filters);

  const handleFilterChange = (newFilters: Partial<UseModerateUsersFilters>) => {
    setFilters(newFilters);
    setSelectedIds([]); // Clear selection when filters change
  };

  const handlePageChange = (page: number) => {
    setFilters({ page } as Partial<UseModerateUsersFilters>);
    setSelectedIds([]); // Clear selection when page changes
  };

  const handlePageSizeChange = (pageSize: number) => {
    setFilters({ pageSize, page: 1 } as Partial<UseModerateUsersFilters>);
    setSelectedIds([]); 
  };

  const handleSelectAll = (checked: boolean) => {
    if (checked && data) {
      setSelectedIds(data.data.map((user) => user.id));
    } else {
      setSelectedIds([]);
    }
  };

  const handleSelectRow = (id: string, checked: boolean) => {
    if (checked) {
      setSelectedIds((prev) => [...prev, id]);
    } else {
      setSelectedIds((prev) => prev.filter((prevId) => prevId !== id));
    }
  };

  const handleExport = async () => {
    setIsExporting(true);
    try {
      const blob = await exportUsersApi(filters, selectedIds);
      
      // Client-side download trigger
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      
      const dateStr = new Date().toISOString().split("T")[0];
      link.setAttribute("download", `mazadzone_users_export_${dateStr}.csv`);
      
      document.body.appendChild(link);
      link.click();
      
      // Cleanup
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
      
      appToast.success("Export Successful", "Your user data export has been downloaded.");
    } catch (error) {
      console.error("Export failed", error);
      appToast.error("Export Failed", "There was an error generating your export. Please try again.");
    } finally {
      setIsExporting(false);
    }
  };

  return (
    <div className="space-y-6 md:space-y-8 p-4 md:p-6 lg:p-8 max-w-[1600px] mx-auto w-full">
      {/* Page Header */}
      <div className="flex flex-col gap-1.5">
        <div className="flex items-center gap-2">
          <h1 className="text-2xl md:text-3xl font-bold tracking-tight text-foreground">
            Moderate Users
          </h1>
          <span title="Review and manage user accounts and account statuses">
            <Info className="h-5 w-5 text-muted-foreground cursor-help opacity-70 hover:opacity-100 transition-opacity" />
          </span>
        </div>
        <p className="text-sm md:text-base text-muted-foreground">
          Review and manage user accounts and account statuses.
        </p>
      </div>

      {/* Filters Bar */}
      <ModerateUsersFilters 
        filters={filters} 
        onFilterChange={handleFilterChange}
        isExporting={isExporting}
        onExport={handleExport}
        selectedIds={selectedIds}
      />

      {/* Data Table */}
      <ModerateUsersTable 
        users={data?.data || []} 
        isLoading={isLoading}
        page={filters.page}
        pageSize={filters.pageSize}
        totalPages={data?.totalPages || 1}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        selectedIds={selectedIds}
        onSelectAll={handleSelectAll}
        onSelectRow={handleSelectRow}
      />
    </div>
  );
}

