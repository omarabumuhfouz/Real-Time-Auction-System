"use client";

import React from "react";
import { Info, ArrowUpRight, ChevronDown } from "lucide-react";
import { Line, LineChart, XAxis, YAxis, CartesianGrid } from "recharts";
import { ChartContainer, ChartTooltip, ChartTooltipContent, type ChartConfig } from "@/components/ui/chart";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import type { UserGrowthTrend } from "../types/admin.types";

interface UserGrowthChartProps {
  data?: UserGrowthTrend;
  isLoading?: boolean;
  timeframe: string;
  onTimeframeChange: (value: string) => void;
}

const chartConfig: ChartConfig = {
  newUsers: {
    label: "New Users",
    color: "var(--color-info-foreground)",
  },
  newSellers: {
    label: "New Sellers",
    color: "var(--primary)",
  },
};

import { UserGrowthChartSkeleton } from "./skeletons";

export function UserGrowthChart({ data, isLoading, timeframe, onTimeframeChange }: UserGrowthChartProps) {
  if (isLoading && !data) {
    return <UserGrowthChartSkeleton />;
  }

  return (
    <div className="bg-card text-card-foreground border border-border rounded-xl p-5 md:p-6 flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-1.5">
          <h3 className="text-base font-bold text-foreground">User Growth Trends</h3>
          <span title="Weekly tracking of user growth across different user stages">
            <Info className="h-4 w-4 text-muted-foreground cursor-help" />
          </span>
        </div>

        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="outline" size="sm" className="h-8 border-border text-xs gap-1.5">
              <span>
                {timeframe === "week" ? "By Week" : timeframe === "month" ? "By Month" : "By Year"}
              </span>
              <ChevronDown className="h-3.5 w-3.5 text-muted-foreground" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="bg-card text-foreground border-border w-32">
            <DropdownMenuItem onClick={() => onTimeframeChange("week")}>By Week</DropdownMenuItem>
            <DropdownMenuItem onClick={() => onTimeframeChange("month")}>By Month</DropdownMenuItem>
            <DropdownMenuItem onClick={() => onTimeframeChange("year")}>By Year</DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>

      {/* Content Wrapper */}
      {!data ? (
        <div className="flex-1 flex items-center justify-center min-h-[300px]">
          <p className="text-xs text-muted-foreground">No data available</p>
        </div>
      ) : (
        <>
          {/* Chart Wrapper */}
          <div className="flex-1 min-h-[300px] relative">
            {isLoading && (
              <div className="absolute inset-0 bg-background/50 flex items-center justify-center z-10 backdrop-blur-[2px] rounded-xl">
                <div className="h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
              </div>
            )}
            <ChartContainer config={chartConfig} className="w-full h-full aspect-auto">
              <LineChart data={data.dataPoints} margin={{ top: 10, right: 5, left: -10, bottom: 0 }}>
                <CartesianGrid strokeDasharray="3 3" vertical={false} />
                <XAxis
                  dataKey="label"
                  tickLine={false}
                  axisLine={false}
                  tickMargin={10}
                  className="text-[11px] font-medium"
                />
                <YAxis
                  tickLine={false}
                  axisLine={false}
                  tickMargin={5}
                  className="text-[11px] font-medium"
                  tickFormatter={(val) => (val >= 1000 ? `${(val / 1000).toFixed(1)}K` : val)}
                />

                <ChartTooltip
                  content={
                    <ChartTooltipContent
                      indicator="dot"
                      labelClassName="font-semibold text-xs"
                    />
                  }
                />

                {/* Blue line: New Users */}
                <Line
                  type="monotone"
                  dataKey="newUsers"
                  stroke="var(--color-info-foreground)"
                  strokeWidth={2.5}
                  dot={{ fill: "var(--color-info-foreground)", strokeWidth: 1, r: 4 }}
                  activeDot={{ r: 6 }}
                />

                {/* Orange line: New Sellers */}
                <Line
                  type="monotone"
                  dataKey="newSellers"
                  stroke="var(--primary)"
                  strokeWidth={2.5}
                  dot={{ fill: "var(--primary)", strokeWidth: 1, r: 4 }}
                  activeDot={{ r: 6 }}
                />
              </LineChart>
            </ChartContainer>
          </div>

          {/* Footer Growth Summaries - 2 Columns */}
          <div className="grid grid-cols-2 gap-4 mt-6 pt-6 border-t border-border">
            {/* New Users */}
            <div className="flex flex-col gap-1">
              <div className="flex items-center gap-1.5 text-[10px] sm:text-xs text-muted-foreground whitespace-nowrap">
                <span className="h-2.5 w-2.5 rounded-full bg-info-foreground shrink-0" />
                <span>Total New Users</span>
              </div>
              <div className="flex items-baseline gap-1 sm:gap-2 mt-1">
                <span className="text-sm sm:text-base font-extrabold text-foreground font-mono">
                  {data.totalNewUsers.value.toLocaleString()}
                </span>
                <span className="flex items-center text-[10px] font-bold text-success-foreground">
                  <ArrowUpRight className="h-3 w-3" />
                  {data.totalNewUsers.changePercent}%
                </span>
              </div>
            </div>

            {/* New Sellers */}
            <div className="flex flex-col gap-1">
              <div className="flex items-center gap-1.5 text-[10px] sm:text-xs text-muted-foreground whitespace-nowrap">
                <span className="h-2.5 w-2.5 rounded-full bg-primary shrink-0" />
                <span>Total New Sellers</span>
              </div>
              <div className="flex items-baseline gap-1 sm:gap-2 mt-1">
                <span className="text-sm sm:text-base font-extrabold text-foreground font-mono">
                  {data.totalNewSellers.value.toLocaleString()}
                </span>
                <span className="flex items-center text-[10px] font-bold text-success-foreground">
                  <ArrowUpRight className="h-3 w-3" />
                  {data.totalNewSellers.changePercent}%
                </span>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
