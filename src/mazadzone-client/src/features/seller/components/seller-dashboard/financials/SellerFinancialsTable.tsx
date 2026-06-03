"use client";

import { Inbox, Download, DollarSign, Percent, ShieldCheck, Box } from "lucide-react";
import { AuctionPagination } from "@/features/auctions";
import type { SellerOrderSummaryDto } from "@/features/seller";
import { SellerFinancialsTableRow } from "./SellerFinancialsTableRow";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { MetricStrip, type MetricStripItem } from "@/components/ui/metric-strip";
import { formatCurrency } from "@/utils/currency.utils";

const TABLE_HEADERS = [
  { key: "transactionId", label: "Transaction ID", className: "" },
  { key: "auction", label: "Auction Name", className: "" },
  { key: "completionDate", label: "Completion Date", className: "" },
  { key: "grossRevenue", label: "Gross Revenue", className: "" },
  { key: "platformFee", label: "Platform Fee (10%)", className: "" },
  { key: "netProfit", label: "Net Profit", className: "" },
  { key: "status", label: "Status", className: "text-right pr-6" },
] as const;

interface SellerFinancialsTableProps {
  orders: SellerOrderSummaryDto[];
  totalCount: number;
  currentPage: number;
  totalPages: number;
  isLoading: boolean;
  onPageChange: (page: number) => void;
  // Financial metrics from API response
  grossRevenue: number;
  platformFees: number;
  netProfit: number;
  completedCount: number;
}

export function SellerFinancialsTable({
  orders,
  totalCount,
  currentPage,
  totalPages,
  isLoading,
  onPageChange,
  grossRevenue,
  platformFees,
  netProfit,
  completedCount,
}: SellerFinancialsTableProps) {

  const financialMetrics: MetricStripItem[] = [
    {
      label: "Gross Revenue",
      value: formatCurrency(grossRevenue),
      subtext: "Cumulative sales volume",
      icon: DollarSign,
      iconClassName: "text-emerald-500",
    },
    {
      label: "Platform Fees",
      value: formatCurrency(platformFees),
      subtext: "5% standard platform cut",
      icon: Percent,
      iconClassName: "text-red-500",
    },
    {
      label: "Net Profit",
      value: formatCurrency(netProfit),
      subtext: "Total earnings after fees",
      icon: ShieldCheck,
      iconClassName: "text-emerald-500",
    },
    {
      label: "Completed Sales",
      value: String(completedCount),
      subtext: "Total fulfilled orders",
      icon: Box,
      iconClassName: "text-blue-500",
    },
  ];

  return (
    <div className="space-y-6 text-left">
      
      {/* Financial Overview Strip */}
      <MetricStrip items={financialMetrics} isLoading={isLoading} />

      {/* Transactions Table Section */}
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <div className="text-left space-y-0.5">
            <h3 className="text-base font-bold tracking-tight text-foreground">Completed Transactions</h3>
            <p className="text-xs text-muted-foreground">Individual audit ledger of successful payouts</p>
          </div>
          <Button
            variant="outline"
            className="rounded-lg h-9 px-3.5 text-xs font-semibold flex items-center gap-2 cursor-pointer"
          >
            <Download className="h-3.5 w-3.5 text-muted-foreground" />
            Export
          </Button>
        </div>

        <div className="overflow-x-auto rounded-xl border border-border bg-card">
          <table className="w-full text-left border-collapse min-w-[900px]">
            <thead>
              <tr className="bg-muted/30 border-b border-border text-[10px] font-bold uppercase tracking-wider text-muted-foreground h-11">
                {TABLE_HEADERS.map((header) => (
                  <th key={header.key} className={cn("px-5 py-3", header.className)}>
                    {header.label}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-border/50">
              {isLoading ? (
                Array.from({ length: 3 }).map((_, rowIdx) => (
                  <tr key={rowIdx} className="animate-pulse">
                    <td className="px-5 py-4"><div className="h-3.5 bg-muted rounded w-16" /></td>
                    <td className="px-5 py-4"><div className="h-3.5 bg-muted rounded w-32" /></td>
                    <td className="px-5 py-4"><div className="h-3.5 bg-muted rounded w-24" /></td>
                    <td className="px-5 py-4"><div className="h-3.5 bg-muted rounded w-20" /></td>
                    <td className="px-5 py-4"><div className="h-3.5 bg-muted rounded w-20" /></td>
                    <td className="px-5 py-4"><div className="h-3.5 bg-muted rounded w-20" /></td>
                    <td className="px-5 py-4 text-right pr-6"><div className="h-5 bg-muted rounded w-16 ml-auto" /></td>
                  </tr>
                ))
              ) : orders.length === 0 ? (
                <tr>
                  <td colSpan={TABLE_HEADERS.length} className="px-5 py-14 text-center">
                    <div className="flex flex-col items-center justify-center space-y-2 max-w-xs mx-auto">
                      <Inbox className="h-8 w-8 text-muted-foreground/40" />
                      <h3 className="text-sm font-bold text-foreground">No completed payouts found</h3>
                      <p className="text-xs text-muted-foreground">
                        Complete active auctions and receive buyer orders to trigger payouts.
                      </p>
                    </div>
                  </td>
                </tr>
              ) : (
                orders.map((order) => (
                  <SellerFinancialsTableRow
                    key={order.orderId}
                    order={order}
                  />
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination Controls */}
        {totalPages > 1 && (
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 pt-2">
            <p className="text-xs font-medium text-muted-foreground text-left">
              Showing <span className="font-bold text-foreground">{Math.min(totalCount, (currentPage - 1) * 5 + 1)}</span> to{" "}
              <span className="font-bold text-foreground">{Math.min(totalCount, currentPage * 5)}</span> of{" "}
              <span className="font-bold text-foreground">{totalCount}</span> payout items
            </p>
            <AuctionPagination
              currentPage={currentPage}
              totalPages={totalPages}
              onPageChange={onPageChange}
              hasPreviousPage={currentPage > 1}
              hasNextPage={currentPage < totalPages}
              className="w-full sm:w-auto"
            />
          </div>
        )}
      </div>

    </div>
  );
}
