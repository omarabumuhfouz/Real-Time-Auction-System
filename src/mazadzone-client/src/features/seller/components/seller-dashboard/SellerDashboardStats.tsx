"use client";

import { Gavel, Clock, ShoppingBag, Box, DollarSign, Percent } from "lucide-react";
import { cn } from "@/lib/utils";

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

  const stats = [
    {
      title: "Active Auctions",
      value: formatValue(activeAuctions),
      subtext: "Live now",
      icon: Gavel,
      textColor: "text-emerald-600 dark:text-emerald-400",
      subtextColor: "text-emerald-500",
      iconBg: "bg-emerald-50 dark:bg-emerald-950/40 border-emerald-100 dark:border-emerald-900/30",
      iconColor: "text-emerald-500 dark:text-emerald-400",
      hoverClass: "hover:bg-emerald-50/20 dark:hover:bg-emerald-950/10 hover:border-emerald-300 dark:hover:border-emerald-800/40",
    },
    {
      title: "Pending Auctions",
      value: formatValue(pending),
      subtext: "Awaiting start",
      icon: Clock,
      textColor: "text-amber-600 dark:text-amber-400",
      subtextColor: "text-amber-500",
      iconBg: "bg-amber-50 dark:bg-amber-950/40 border-amber-100 dark:border-amber-900/30",
      iconColor: "text-amber-500 dark:text-amber-400",
      hoverClass: "hover:bg-amber-50/20 dark:hover:bg-amber-950/10 hover:border-amber-300 dark:hover:border-amber-800/40",
    },
    {
      title: "Sold Items",
      value: formatValue(soldItems),
      subtext: "Total sold",
      icon: ShoppingBag,
      textColor: "text-blue-600 dark:text-blue-400",
      subtextColor: "text-blue-500",
      iconBg: "bg-blue-50 dark:bg-blue-950/40 border-blue-100 dark:border-blue-900/30",
      iconColor: "text-blue-500 dark:text-blue-400",
      hoverClass: "hover:bg-blue-50/20 dark:hover:bg-blue-950/10 hover:border-blue-300 dark:hover:border-blue-800/40",
    },
    {
      title: "Total Orders",
      value: formatValue(totalOrders),
      subtext: "All orders",
      icon: Box,
      textColor: "text-purple-600 dark:text-purple-400",
      subtextColor: "text-purple-500",
      iconBg: "bg-purple-50 dark:bg-purple-950/40 border-purple-100 dark:border-purple-900/30",
      iconColor: "text-purple-500 dark:text-purple-400",
      hoverClass: "hover:bg-purple-50/20 dark:hover:bg-purple-950/10 hover:border-purple-300 dark:hover:border-purple-800/40",
    },
    {
      title: "Gross Revenue",
      value: formatValue(grossRevenue, true),
      subtext: "All time",
      icon: DollarSign,
      textColor: "text-emerald-600 dark:text-emerald-400",
      subtextColor: "text-emerald-500",
      iconBg: "bg-emerald-50 dark:bg-emerald-950/40 border-emerald-100 dark:border-emerald-900/30",
      iconColor: "text-emerald-500 dark:text-emerald-400",
      hoverClass: "hover:bg-emerald-50/20 dark:hover:bg-emerald-950/10 hover:border-emerald-300 dark:hover:border-emerald-800/40",
    },
    {
      title: "Net Profit",
      value: formatValue(netProfit, true),
      subtext: "All time",
      icon: Percent,
      textColor: "text-orange-600 dark:text-orange-400",
      subtextColor: "text-orange-500",
      iconBg: "bg-orange-50 dark:bg-orange-950/40 border-orange-100 dark:border-orange-900/30",
      iconColor: "text-orange-500 dark:text-orange-400",
      hoverClass: "hover:bg-orange-50/20 dark:hover:bg-orange-950/10 hover:border-orange-300 dark:hover:border-orange-800/40",
    },
  ];

  if (isLoading) {
    return (
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6">
        {Array.from({ length: 6 }).map((_, idx) => (
          <div
            key={idx}
            className="bg-card text-card-foreground border border-border/80 rounded-2xl p-5 shadow-sm h-[100px] w-full animate-pulse flex items-center justify-between"
          >
            <div className="space-y-2">
              <div className="h-3 bg-muted rounded w-20" />
              <div className="h-6 bg-muted rounded w-16" />
              <div className="h-3 bg-muted rounded w-14" />
            </div>
            <div className="h-11 w-11 bg-muted rounded-xl shrink-0" />
          </div>
        ))}
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6">
      {stats.map((stat, i) => {
        const Icon = stat.icon;
        return (
          <div
            key={i}
            className={cn(
              "bg-card text-card-foreground border border-border/80 rounded-2xl p-5 shadow-xs",
              "hover:shadow-sm hover:scale-[1.01] active:scale-[0.99] transition-all duration-200",
              "flex items-center justify-between h-[102px] w-full relative overflow-hidden cursor-pointer",
              stat.hoverClass
            )}
          >
            <div className="flex flex-col justify-between h-full text-left">
              <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">
                {stat.title}
              </span>
              <div className="text-xl font-black tracking-tight text-foreground my-0.5">
                {stat.value}
              </div>
              <span className={cn("text-[11px] font-bold", stat.subtextColor)}>
                {stat.subtext}
              </span>
            </div>
            
            <div className={cn("flex h-11 w-11 items-center justify-center rounded-xl border shadow-2xs shrink-0 transition-transform hover:scale-105", stat.iconBg)}>
              <Icon className={cn("h-5 w-5", stat.iconColor)} />
            </div>
          </div>
        );
      })}
    </div>
  );
}
