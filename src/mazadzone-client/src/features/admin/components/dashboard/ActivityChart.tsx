"use client";

import React from "react";
import { Info, ArrowUpRight, ChevronDown } from "lucide-react";
import {
  Bar,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  ComposedChart,
} from "recharts";
import {
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
  type ChartConfig,
} from "@/components/ui/chart";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import type { AuctionActivityTrend } from "../../types/admin.types";

interface ActivityChartProps {
  data?: AuctionActivityTrend;
  isLoading?: boolean;
  timeframe: string;
  onTimeframeChange: (value: string) => void;
}

const chartConfig: ChartConfig = {
  newAuctions: {
    label: "New Auctions",
    color: "var(--primary)",
  },
  bidsPlaced: {
    label: "Bids Placed",
    color: "var(--color-dark)",
  },
};

import { ActivityChartSkeleton } from "./skeletons";

export function ActivityChart({ data, isLoading, timeframe, onTimeframeChange }: ActivityChartProps) {
  if (isLoading && !data) {
    return <ActivityChartSkeleton />;
  }

  return (
    <div className="bg-card text-card-foreground border border-border rounded-xl p-5 md:p-6 flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-1.5">
          <h3 className="text-base font-bold text-foreground">
            Auction Activity & Bidding Trend
          </h3>
          <span title="Weekly tracking of launched auctions versus total bids submitted">
            <Info className="h-4 w-4 text-muted-foreground cursor-help" />
          </span>
        </div>

        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="outline" size="sm" className="h-8 border-border text-xs gap-1.5">
              <span>By {timeframe}</span>
              <ChevronDown className="h-3.5 w-3.5 text-muted-foreground" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="bg-card text-foreground border-border w-32">
            <DropdownMenuItem onClick={() => onTimeframeChange("Day")}>By Day</DropdownMenuItem>
            <DropdownMenuItem onClick={() => onTimeframeChange("Week")}>By Week</DropdownMenuItem>
            <DropdownMenuItem onClick={() => onTimeframeChange("Month")}>By Month</DropdownMenuItem>
            <DropdownMenuItem onClick={() => onTimeframeChange("Quarter")}>By Quarter</DropdownMenuItem>
            <DropdownMenuItem onClick={() => onTimeframeChange("Year")}>By Year</DropdownMenuItem>
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
              <ComposedChart data={data.dataPoints} margin={{ top: 10, right: -5, left: -10, bottom: 0 }}>
                <CartesianGrid strokeDasharray="3 3" vertical={false} />
                <XAxis
                  dataKey="label"
                  tickLine={false}
                  axisLine={false}
                  tickMargin={10}
                  className="text-[11px] font-medium"
                />
                {/* Left Y Axis for Bar (New Auctions) */}
                <YAxis
                  yAxisId="left"
                  tickLine={false}
                  axisLine={false}
                  tickMargin={5}
                  tick={{ fill: "var(--primary)", fontSize: 11, fontWeight: 500 }}
                  tickFormatter={(val) => (val >= 1000 ? `${(val / 1000).toFixed(1)}K` : val)}
                />
                {/* Right Y Axis for Line (Bids Placed) */}
                <YAxis
                  yAxisId="right"
                  orientation="right"
                  tickLine={false}
                  axisLine={false}
                  tickMargin={5}
                  tick={{ fill: "var(--color-dark)", fontSize: 11, fontWeight: 500 }}
                  tickFormatter={(val) => (val >= 1000 ? `${(val / 1000).toFixed(0)}K` : val)}
                />

                <ChartTooltip
                  content={
                    <ChartTooltipContent
                      indicator="dot"
                      labelClassName="font-semibold text-xs"
                    />
                  }
                />

                {/* Orange Bars for New Auctions */}
                <Bar
                  yAxisId="left"
                  dataKey="newAuctions"
                  fill="var(--primary)"
                  radius={[4, 4, 0, 0]}
                  maxBarSize={45}
                />

                {/* Dark Navy Line for Bids Placed */}
                <Line
                  yAxisId="right"
                  type="monotone"
                  dataKey="bidsPlaced"
                  stroke="var(--color-dark)"
                  strokeWidth={2.5}
                  dot={{ fill: "var(--color-dark)", strokeWidth: 1, r: 4 }}
                  activeDot={{ r: 6 }}
                />
              </ComposedChart>
            </ChartContainer>
          </div>

          {/* Footer Metrics */}
          <div className="grid grid-cols-2 gap-4 mt-6 pt-6 border-t border-border">
            {/* New Auctions Box */}
            <div className="flex flex-col gap-1">
              <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
                <span className="h-2.5 w-2.5 rounded-sm bg-primary shrink-0" />
                <span>Total New Auctions</span>
              </div>
              <div className="flex items-baseline gap-2 mt-1">
                <span className="text-xl font-bold text-foreground">
                  {data.totalNewAuctions.value.toLocaleString()}
                </span>
                <span className="flex items-center text-xs font-semibold text-success-foreground">
                  <ArrowUpRight className="h-3 w-3 mr-0.5" />
                  {data.totalNewAuctions.changePercent}%
                </span>
              </div>
            </div>

            {/* Bids Placed Box */}
            <div className="flex flex-col gap-1">
              <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
                <span className="h-2.5 w-2.5 rounded-sm bg-dark shrink-0" />
                <span>Total Bids Placed</span>
              </div>
              <div className="flex items-baseline gap-2 mt-1">
                <span className="text-xl font-bold text-foreground">
                  {data.totalBidsPlaced.value.toLocaleString()}
                </span>
                <span className="flex items-center text-xs font-semibold text-success-foreground">
                  <ArrowUpRight className="h-3 w-3 mr-0.5" />
                  {data.totalBidsPlaced.changePercent}%
                </span>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
