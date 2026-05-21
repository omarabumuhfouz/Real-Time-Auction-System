"use client";

import React from "react";
import { ArrowUpRight, ArrowDownRight } from "lucide-react";
import { cn } from "@/lib/utils";

interface MetricCardProps {
  title: string;
  value: string | number;
  changePercent: number;
  isPositive: boolean;
  icon: React.ComponentType<{ className?: string }>;
  iconClassName?: string;
  iconBgClassName?: string;
}

export function MetricCard({
  title,
  value,
  changePercent,
  isPositive,
  icon: Icon,
  iconClassName,
  iconBgClassName,
}: MetricCardProps) {
  const formattedChange = `${isPositive ? "+" : ""}${changePercent}%`;

  return (
    <div className="bg-card text-card-foreground border border-border rounded-xl p-4 sm:p-5 hover:shadow-md transition-shadow duration-200">
      <div className="flex items-start justify-between gap-3">
        {/* Metric Header */}
        <div className="flex flex-col gap-1 min-w-0">
          <span className="text-xs sm:text-sm font-medium text-muted-foreground truncate" title={title}>
            {title}
          </span>
          <span className="text-xl sm:text-2xl font-extrabold tracking-tight text-foreground mt-0.5 break-all">
            {value}
          </span>
        </div>

        {/* Icon Wrapper */}
        <div className={cn("p-2 sm:p-3 rounded-lg flex items-center justify-center shrink-0", iconBgClassName)}>
          <Icon className={cn("h-5 w-5 sm:h-6 sm:w-6", iconClassName)} />
        </div>
      </div>

      {/* Metric Footer Trend */}
      <div className="flex flex-wrap items-center gap-1.5 mt-4 text-xs font-medium">
        <span
          className={cn(
            "flex items-center px-1.5 py-0.5 rounded-md shrink-0",
            isPositive
              ? "bg-success-foreground/10 text-success-foreground"
              : "bg-destructive/10 text-destructive"
          )}
        >
          {isPositive ? (
            <ArrowUpRight className="h-3.5 w-3.5 mr-0.5" />
          ) : (
            <ArrowDownRight className="h-3.5 w-3.5 mr-0.5" />
          )}
          {formattedChange}
        </span>
        <span className="text-muted-foreground text-[10px] sm:text-xs">vs prev 30d</span>
      </div>
    </div>
  );
}
