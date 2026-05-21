"use client";

import React, { useState } from "react";
import { MetricCard } from "./MetricCard";
import { ActivityChart } from "./ActivityChart";
import { OpenDisputesBreakdown } from "./OpenDisputesBreakdown";
import { UserTrust } from "./UserTrust";
import { UserGrowthChart } from "./UserGrowthChart";
import { CategoryHealth } from "./CategoryHealth";
import { PaymentProviderStatus } from "./PaymentProviderStatus";
import { useGetAuctionActivityTrend, useGetUserGrowthTrend } from "../api/queries";
import { formatCurrency } from "@/utils/currency.utils";
import { METRIC_CARD_DEFINITIONS } from "../constants/dashboard.constants";
import type { AdminDashboardOverviewData } from "../types/admin.types";

interface DashboardOverviewProps {
  stats: AdminDashboardOverviewData;
}

export function DashboardOverview({ stats }: DashboardOverviewProps) {
  const [activityTimeframe, setActivityTimeframe] = useState("week");
  const [growthTimeframe, setGrowthTimeframe] = useState("week");

  const { data: activityData, isLoading: isActivityLoading } = useGetAuctionActivityTrend(activityTimeframe);
  const { data: growthData, isLoading: isGrowthLoading } = useGetUserGrowthTrend(growthTimeframe);
  
  // Helper to format currency values
  const formatCardValue = (val: number | string) => {
    const num = typeof val === "string" ? parseFloat(val) : val;
    return formatCurrency(num);
  };

  const metricCards = METRIC_CARD_DEFINITIONS.map((def) => {
    const metric = stats.metrics[def.metricKey];
    const value = def.isCurrency
      ? formatCardValue(metric.value)
      : metric.value.toLocaleString();

    return {
      title: def.title,
      value,
      changePercent: metric.changePercent,
      isPositive: metric.isPositive,
      icon: def.icon,
      iconClassName: def.iconClassName,
      iconBgClassName: def.iconBgClassName,
    };
  });

  return (
    <div className="space-y-6 md:space-y-8">
      {/* 1. Metric Cards Grid - 6 Items */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
        {metricCards.map((card) => (
          <MetricCard
            key={card.title}
            title={card.title}
            value={card.value}
            changePercent={card.changePercent}
            isPositive={card.isPositive}
            icon={card.icon}
            iconClassName={card.iconClassName}
            iconBgClassName={card.iconBgClassName}
          />
        ))}
      </div>

      {/* 2. Charts Row 1: Composed Trend Chart (2/3) & Donut Disputes (1/3) */}
      <div className="grid grid-cols-1 xl:grid-cols-3 gap-6 items-stretch">
        <div className="xl:col-span-2">
          <ActivityChart
            data={activityData || stats.auctionActivity}
            isLoading={isActivityLoading}
            timeframe={activityTimeframe}
            onTimeframeChange={setActivityTimeframe}
          />
        </div>
        <div className="xl:col-span-1">
          <OpenDisputesBreakdown data={stats.openDisputesBreakdown} />
        </div>
      </div>

      {/* 3. Charts Row 2: User Growth (2/3) & User Trust (1/3) -> Swapped Sides */}
      <div className="grid grid-cols-1 xl:grid-cols-3 gap-6 items-stretch">
        <div className="xl:col-span-2">
          <UserGrowthChart
            data={growthData || stats.userGrowth}
            isLoading={isGrowthLoading}
            timeframe={growthTimeframe}
            onTimeframeChange={setGrowthTimeframe}
          />
        </div>
        <div className="xl:col-span-1">
          <UserTrust data={stats.userTrust} />
        </div>
      </div>

      {/* 4. Row 3: Category Health (1/2) & Payment Status (1/2) */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 items-stretch">
        <CategoryHealth data={stats.categoryHealth} />
        <PaymentProviderStatus data={stats.payments} />
      </div>
    </div>
  );
}
