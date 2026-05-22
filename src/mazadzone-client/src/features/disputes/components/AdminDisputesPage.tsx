"use client";

import { AdminDisputesFilters } from "./AdminDisputesFilters";
import { AdminDisputesTable } from "./AdminDisputesTable";
import { useGetAdminDisputes } from "../api/use-get-admin-disputes";
import { useUrlFilters } from "@/hooks/use-url-filters";

export function AdminDisputesPage() {
  const { searchParams, setFilters } = useUrlFilters<{
    search: string;
    status: string;
    category: string;
    page: number;
    pageSize: number;
  }>();

  const search = searchParams.get("search") || "";
  const status = searchParams.get("status") || "All Statuses";
  const category = searchParams.get("category") || "All Categories";
  const page = Number(searchParams.get("page")) || 1;
  const pageSize = Number(searchParams.get("pageSize")) || 10;

  const { data, isLoading } = useGetAdminDisputes({ search, status, category, page, pageSize });

  const handlePageChange = (newPage: number) => {
    setFilters({ page: newPage });
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setFilters({ pageSize: newPageSize, page: 1 });
  };

  return (
    <div className="flex flex-col gap-6 w-full max-w-[1400px] mx-auto p-4 md:p-6 lg:p-8">
      {/* Header section */}
      <div className="flex flex-col gap-1">
        <h1 className="text-3xl font-bold tracking-tight text-foreground">
          Resolve Disputes
        </h1>
        <p className="text-muted-foreground text-sm">
          Review and manage dispute cases across orders and auctions.
        </p>
      </div>

      {/* Filters section */}
      <AdminDisputesFilters
        search={search}
        setSearch={(val) => setFilters({ search: val, page: 1 })}
        status={status}
        setStatus={(val) => setFilters({ status: val, page: 1 })}
        category={category}
        setCategory={(val) => setFilters({ category: val, page: 1 })}
      />

      {/* Table section */}
      <AdminDisputesTable
        data={data?.data || []}
        isLoading={isLoading}
        page={page}
        pageSize={pageSize}
        totalPages={data?.totalPages || 1}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
      />
    </div>
  );
}
