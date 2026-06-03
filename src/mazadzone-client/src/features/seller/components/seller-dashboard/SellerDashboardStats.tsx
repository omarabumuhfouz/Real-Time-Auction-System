"use client";

import { Gavel, Clock, ShoppingBag, Box, DollarSign, Percent } from "lucide-react";
import { MetricStrip, type MetricStripItem } from "@/components/ui/metric-strip";

interface SellerDashboardStatsProps {
  activeAuctions: number;
  pending: number;
  soldItems: number;
  totalOrders: number;
  grossRevenue: number;
  netProfit: number;
  isLoading?: boolean;
}

export function SellerDashboardStats({
  activeAuctions,
  pending,
  soldItems,
  totalOrders,
  grossRevenue,
  netProfit,
  isLoading = false,
}: SellerDashboardStatsProps) {

  const formatValue = (val: number, isCurrency: boolean = false) => {
    const formatted = val.toLocaleString("en-US", {
      maximumFractionDigits: 0,
    });
    return isCurrency ? `${formatted} JD` : formatted;
  };

  const items: MetricStripItem[] = [
    {
      label: "Active Auctions",
      value: formatValue(activeAuctions),
      subtext: "Live now",
      icon: Gavel,
      iconClassName: "text-emerald-500",
    },
    {
      label: "Pending Auctions",
      value: formatValue(pending),
      subtext: "Awaiting start",
      icon: Clock,
      iconClassName: "text-amber-500",
    },
    {
      label: "Sold Items",
      value: formatValue(soldItems),
      subtext: "Total sold",
      icon: ShoppingBag,
      iconClassName: "text-blue-500",
    },
    {
      label: "Total Orders",
      value: formatValue(totalOrders),
      subtext: "All orders",
      icon: Box,
      iconClassName: "text-purple-500",
    },
    {
      label: "Gross Revenue",
      value: formatValue(grossRevenue, true),
      subtext: "All time",
      icon: DollarSign,
      iconClassName: "text-emerald-500",
    },
    {
      label: "Net Profit",
      value: formatValue(netProfit, true),
      subtext: "All time",
      icon: Percent,
      iconClassName: "text-orange-500",
    },
  ];

  return <MetricStrip items={items} isLoading={isLoading} />;
}
