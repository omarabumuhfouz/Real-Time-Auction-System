"use client";

import { Inbox, Download, DollarSign, Percent, ShieldCheck, Box } from "lucide-react";
import { AuctionPagination } from "@/features/auctions";
import type { SellerOrderSummaryDto } from "@/features/auctions";
import { SellerFinancialsTableRow } from "./SellerFinancialsTableRow";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
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

  return (
    <div className="bg-white dark:bg-card border border-border/80 rounded-b-2xl rounded-t-none p-6 shadow-xs space-y-8 border-t-0 text-left">
      
      {/* 4 Financial Sub-Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {/* Gross Revenue Card */}
        <div className="border border-border/80 rounded-2xl p-5 flex items-center justify-between shadow-2xs bg-emerald-500/[0.02]">
          <div className="space-y-1 text-left">
            <span className="text-[10px] font-black text-muted-foreground uppercase tracking-wider">Gross Revenue</span>
            <div className="text-2xl font-black tracking-tight text-foreground">{formatCurrency(grossRevenue)}</div>
            <p className="text-[10px] font-bold text-emerald-500">Cumulative sales volume</p>
          </div>
          <div className="h-11 w-11 rounded-xl bg-emerald-50 dark:bg-emerald-950/40 border border-emerald-100 dark:border-emerald-900/30 flex items-center justify-center text-emerald-500">
            <DollarSign className="h-5 w-5" />
          </div>
        </div>

        {/* Platform Fees Card */}
        <div className="border border-border/80 rounded-2xl p-5 flex items-center justify-between shadow-2xs bg-red-500/[0.01]">
          <div className="space-y-1 text-left">
            <span className="text-[10px] font-black text-muted-foreground uppercase tracking-wider">Platform Fees</span>
            <div className="text-2xl font-black tracking-tight text-foreground">{formatCurrency(platformFees)}</div>
            <p className="text-[10px] font-bold text-red-500">5% standard platform cut</p>
          </div>
          <div className="h-11 w-11 rounded-xl bg-red-50 dark:bg-red-950/40 border border-red-100 dark:border-red-900/30 flex items-center justify-center text-red-500">
            <Percent className="h-5 w-5" />
          </div>
        </div>

        {/* Net Profit Card */}
        <div className="border border-border/80 rounded-2xl p-5 flex items-center justify-between shadow-2xs bg-emerald-500/[0.02]">
          <div className="space-y-1 text-left">
            <span className="text-[10px] font-black text-muted-foreground uppercase tracking-wider">Net Profit</span>
            <div className="text-2xl font-black tracking-tight text-foreground">{formatCurrency(netProfit)}</div>
            <p className="text-[10px] font-bold text-emerald-500">Total earnings after fees</p>
          </div>
          <div className="h-11 w-11 rounded-xl bg-emerald-50 dark:bg-emerald-950/40 border border-emerald-100 dark:border-emerald-900/30 flex items-center justify-center text-emerald-500">
            <ShieldCheck className="h-5 w-5" />
          </div>
        </div>

        {/* Completed Transactions Card */}
        <div className="border border-border/80 rounded-2xl p-5 flex items-center justify-between shadow-2xs bg-blue-500/[0.01]">
          <div className="space-y-1 text-left">
            <span className="text-[10px] font-black text-muted-foreground uppercase tracking-wider">Completed Sales</span>
            <div className="text-2xl font-black tracking-tight text-foreground">{completedCount}</div>
            <p className="text-[10px] font-bold text-blue-500">Total fulfilled orders</p>
          </div>
          <div className="h-11 w-11 rounded-xl bg-blue-50 dark:bg-blue-950/40 border border-blue-100 dark:border-blue-900/30 flex items-center justify-center text-blue-500">
            <Box className="h-5 w-5" />
          </div>
        </div>
      </div>

      {/* Breakout Table Section */}
      <div className="space-y-4 pt-2">
        <div className="flex items-center justify-between border-b border-border/40 pb-4">
          <div className="text-left space-y-1">
            <h3 className="text-base font-black tracking-tight text-foreground">Completed Transactions</h3>
            <p className="text-xs text-muted-foreground font-medium">Individual audit ledger of successful payouts</p>
          </div>
          <Button
            variant="outline"
            className="bg-white hover:bg-slate-50 dark:bg-card dark:hover:bg-accent/15 rounded-xl h-10 px-4 text-xs font-bold flex items-center justify-center gap-2 border-border/80 shadow-2xs cursor-pointer"
          >
            <Download className="h-4 w-4 text-muted-foreground" />
            Export Financials
          </Button>
        </div>

        <div className="overflow-x-auto rounded-2xl border border-border/60">
          <table className="w-full text-left border-collapse min-w-[900px]">
            <thead>
              <tr className="bg-[#fafafa] dark:bg-accent/10 border-b border-border/80 text-[10px] font-black uppercase tracking-wider text-muted-foreground h-12">
                {TABLE_HEADERS.map((header) => (
                  <th key={header.key} className={cn("px-6 py-3", header.className)}>
                    {header.label}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-border/50">
              {isLoading ? (
                Array.from({ length: 3 }).map((_, rowIdx) => (
                  <tr key={rowIdx} className="animate-pulse">
                    <td className="px-6 py-4"><div className="h-3.5 bg-muted rounded w-16" /></td>
                    <td className="px-6 py-4"><div className="h-3.5 bg-muted rounded w-32" /></td>
                    <td className="px-6 py-4"><div className="h-3.5 bg-muted rounded w-24" /></td>
                    <td className="px-6 py-4"><div className="h-3.5 bg-muted rounded w-20" /></td>
                    <td className="px-6 py-4"><div className="h-3.5 bg-muted rounded w-20" /></td>
                    <td className="px-6 py-4"><div className="h-3.5 bg-muted rounded w-20" /></td>
                    <td className="px-6 py-4 text-right pr-6"><div className="h-5 bg-muted rounded w-16 ml-auto" /></td>
                  </tr>
                ))
              ) : orders.length === 0 ? (
                <tr>
                  <td colSpan={TABLE_HEADERS.length} className="px-6 py-16 text-center">
                    <div className="flex flex-col items-center justify-center space-y-3 max-w-sm mx-auto">
                      <Inbox className="h-10 w-10 text-muted-foreground/60" />
                      <h3 className="text-base font-black text-foreground">No completed payouts found</h3>
                      <p className="text-xs text-muted-foreground font-semibold">
                        Payout list is currently empty. Complete active auctions and receive buyer orders to trigger payouts.
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
          <div className="border-t border-border/30 pt-6 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
            <p className="text-xs font-semibold text-muted-foreground text-left">
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
