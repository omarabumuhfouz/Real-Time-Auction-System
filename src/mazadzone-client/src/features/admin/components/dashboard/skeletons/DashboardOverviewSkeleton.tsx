import React from "react";
import { MetricCardSkeleton } from "./MetricCardSkeleton";
import { ActivityChartSkeleton } from "./ActivityChartSkeleton";
import { OpenDisputesBreakdownSkeleton } from "./OpenDisputesBreakdownSkeleton";
import { UserGrowthChartSkeleton } from "./UserGrowthChartSkeleton";
import { UserTrustSkeleton } from "./UserTrustSkeleton";
import { CategoryHealthSkeleton } from "./CategoryHealthSkeleton";
import { PaymentProviderStatusSkeleton } from "./PaymentProviderStatusSkeleton";

export function DashboardOverviewSkeleton() {
  return (
    <div className="space-y-6 md:space-y-8 animate-pulse">
      {/* 1. Metric Cards Grid - 6 Items */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
        {Array.from({ length: 6 }).map((_, index) => (
          <MetricCardSkeleton key={index} />
        ))}
      </div>

      {/* 2. Charts Row 1: Activity Chart (2/3) & Donut Disputes (1/3) */}
      <div className="grid grid-cols-1 xl:grid-cols-3 gap-6 items-stretch">
        <div className="xl:col-span-2">
          <ActivityChartSkeleton />
        </div>
        <div className="xl:col-span-1">
          <OpenDisputesBreakdownSkeleton />
        </div>
      </div>

      {/* 3. Charts Row 2: User Growth (2/3) & User Trust (1/3) */}
      <div className="grid grid-cols-1 xl:grid-cols-3 gap-6 items-stretch">
        <div className="xl:col-span-2">
          <UserGrowthChartSkeleton />
        </div>
        <div className="xl:col-span-1">
          <UserTrustSkeleton />
        </div>
      </div>

      {/* 4. Row 3: Category Health (1/2) & Payment Status (1/2) */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 items-stretch">
        <CategoryHealthSkeleton />
        <PaymentProviderStatusSkeleton />
      </div>
    </div>
  );
}
