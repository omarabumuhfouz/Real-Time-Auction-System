"use client";

import React, { useState } from "react";
import { useGetAdminOverviewStats } from "../../api/queries";
import { DashboardOverview } from "./DashboardOverview";
import { AdminHeader } from "../layout/AdminHeader";
import { Button } from "@/components/ui/button";
import { DashboardOverviewSkeleton } from "./skeletons";

export function AdminDashboardPage() {
  const { data: stats, isLoading, isError, refetch } = useGetAdminOverviewStats();
  const [timeRange, setTimeRange] = useState("30");

  return (
    <div className="flex flex-col">
      <AdminHeader
        title="Admin Dashboard Overview"
        timeRange={timeRange}
        onTimeRangeChange={setTimeRange}
        onRefresh={refetch}
        showTimeRange
        showRefresh
      />

      {/* Main dashboard contents */}
      <div className="flex-1 p-6 md:p-8 space-y-8 max-w-[1600px] mx-auto w-full">
        {isLoading ? (
          <DashboardOverviewSkeleton />
        ) : isError || !stats ? (
          <div className="flex flex-col items-center justify-center min-h-[400px] gap-4 text-center">
            <p className="text-destructive font-semibold">Failed to load dashboard data</p>
            <Button onClick={() => refetch()} variant="outline">
              Retry
            </Button>
          </div>
        ) : (
          <DashboardOverview stats={stats} />
        )}
      </div>
    </div>
  );
}
