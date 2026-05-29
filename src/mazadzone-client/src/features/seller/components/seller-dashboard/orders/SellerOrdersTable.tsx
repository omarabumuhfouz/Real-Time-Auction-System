"use client";

import { Inbox, Filter, ArrowUpDown, Download } from "lucide-react";
import { AuctionPagination } from "@/features/auctions";
import type { SellerOrderSummaryDto } from "@/features/seller";
import { SellerOrdersTableRow } from "./SellerOrdersTableRow";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

const STATUS_FILTER_OPTIONS = [
  { value: "All", label: "All Statuses" },
  { value: "Pending", label: "Pending" },
  { value: "Shipped", label: "Shipped" },
  { value: "Delivered", label: "Delivered" },
  { value: "Completed", label: "Completed" },
  { value: "Canceled", label: "Canceled/Refunded" },
] as const;

const SORT_OPTIONS = [
  { value: "OrderDate", label: "Order Date" },
  { value: "TotalAmount", label: "Total Amount" },
] as const;

const TABLE_HEADERS = [
  { key: "orderId", label: "Order ID", className: "" },
  { key: "auction", label: "Auction Name & Category", className: "" },
  { key: "bidder", label: "Bidder & Email", className: "" },
  { key: "status", label: "Status", className: "" },
  { key: "orderDate", label: "Order Date", className: "" },
  { key: "totalAmount", label: "Total Amount", className: "" },
] as const;

interface SellerOrdersTableProps {
  orders: SellerOrderSummaryDto[];
  totalCount: number;
  currentPage: number;
  totalPages: number;
  isLoading: boolean;
  activeStatus: string;
  sortBy: string;
  onStatusChange: (status: string) => void;
  onSortChange: (sort: string) => void;
  onPageChange: (page: number) => void;
  // Stats counters to build the capsules row
  allCount: number;
  pendingCount: number;
  shippedCount: number;
  deliveredCount: number;
  completedCount: number;
  canceledCount: number;
}

export function SellerOrdersTable({
  orders,
  totalCount,
  currentPage,
  totalPages,
  isLoading,
  activeStatus,
  sortBy,
  onStatusChange,
  onSortChange,
  onPageChange,
  allCount,
  pendingCount,
  shippedCount,
  deliveredCount,
  completedCount,
  canceledCount,
}: SellerOrdersTableProps) {

  // Click handler for status capsules
  const handleCapsuleClick = (status: string) => {
    if (activeStatus === status) {
      onStatusChange("All");
    } else {
      onStatusChange(status);
    }
  };

  return (
    <div className="bg-white dark:bg-card border border-border/80 rounded-b-2xl rounded-t-none p-6 shadow-xs space-y-6 border-t-0">

      {/* Header Filters & Controls */}
      <div className="flex flex-col lg:flex-row lg:items-center justify-between gap-4 border-b border-border/40 pb-4">
        <div className="flex items-center gap-3 text-left">
          <h2 className="text-xl font-black tracking-tight text-foreground">
            Your Orders
          </h2>
          <div className="flex items-center gap-1.5 text-blue-500 font-bold text-xs bg-blue-500/10 px-2.5 py-1 rounded-full">
            <span className="h-1.5 w-1.5 rounded-full bg-blue-500" />
            {totalCount} total orders
          </div>
        </div>

        <div className="flex flex-wrap items-center gap-3 self-start lg:self-auto">
          {/* Status Select */}
          <Select value={activeStatus} onValueChange={onStatusChange}>
            <SelectTrigger className="w-[180px] rounded-xl text-xs h-10 bg-white dark:bg-card border border-border/80 hover:bg-accent/10 transition-all font-bold px-3 cursor-pointer">
              <span className="flex items-center gap-2">
                <Filter className="h-4 w-4 text-muted-foreground shrink-0" />
                <SelectValue placeholder="All Statuses" />
              </span>
            </SelectTrigger>
            <SelectContent position="popper" sideOffset={5} className="rounded-xl">
              {STATUS_FILTER_OPTIONS.map((opt) => (
                <SelectItem key={opt.value} className="text-xs rounded-lg font-bold" value={opt.value}>
                  {opt.label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>

          {/* Sort Select */}
          <Select value={sortBy} onValueChange={onSortChange}>
            <SelectTrigger className="w-[180px] rounded-xl text-xs h-10 bg-white dark:bg-card border border-border/80 hover:bg-accent/10 transition-all font-bold px-3 cursor-pointer">
              <span className="flex items-center gap-2">
                <ArrowUpDown className="h-4 w-4 text-muted-foreground shrink-0" />
                <SelectValue placeholder="Sort by" />
              </span>
            </SelectTrigger>
            <SelectContent position="popper" sideOffset={5} className="rounded-xl">
              {SORT_OPTIONS.map((opt) => (
                <SelectItem key={opt.value} className="text-xs rounded-lg font-bold" value={opt.value}>
                  {opt.label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>

          {/* Export Button */}
          <Button
            variant="outline"
            className="bg-white hover:bg-slate-50 dark:bg-card dark:hover:bg-accent/15 rounded-xl h-10 px-4 text-xs font-bold flex items-center justify-center gap-2 border-border/80 cursor-pointer shadow-2xs"
          >
            <Download className="h-4 w-4 text-muted-foreground" />
            Export Orders
          </Button>
        </div>
      </div>

      {/* Internal Status Capsules Grid */}
      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-4">
        {/* All Orders Pill */}
        <button
          onClick={() => onStatusChange("All")}
          className={cn(
            "bg-white dark:bg-card border rounded-2xl p-4 flex items-center gap-4 text-left transition-all hover:scale-[1.02] shadow-2xs cursor-pointer",
            activeStatus === "All"
              ? "border-primary bg-primary/5 ring-2 ring-primary/20"
              : "border-border/80 hover:border-primary/40"
          )}
        >
          <div className="h-3 w-3 rounded-full bg-primary shrink-0" />
          <div className="flex flex-col">
            <span className="text-[11px] font-bold text-muted-foreground uppercase tracking-wider">All</span>
            <span className="text-xl font-black text-foreground mt-0.5">{allCount}</span>
          </div>
        </button>

        {/* Pending Pill */}
        <button
          onClick={() => handleCapsuleClick("Pending")}
          className={cn(
            "bg-white dark:bg-card border rounded-2xl p-4 flex items-center gap-4 text-left transition-all hover:scale-[1.02] shadow-2xs cursor-pointer",
            activeStatus === "Pending"
              ? "border-amber-500 bg-amber-50/10 dark:bg-amber-950/10 ring-2 ring-amber-500/20"
              : "border-border/80 hover:border-amber-500/40"
          )}
        >
          <div className="h-3 w-3 rounded-full bg-amber-500 shrink-0" />
          <div className="flex flex-col">
            <span className="text-[11px] font-bold text-muted-foreground uppercase tracking-wider">Pending</span>
            <span className="text-xl font-black text-foreground mt-0.5">{pendingCount}</span>
          </div>
        </button>

        {/* Shipped Pill */}
        <button
          onClick={() => handleCapsuleClick("Shipped")}
          className={cn(
            "bg-white dark:bg-card border rounded-2xl p-4 flex items-center gap-4 text-left transition-all hover:scale-[1.02] shadow-2xs cursor-pointer",
            activeStatus === "Shipped"
              ? "border-blue-500 bg-blue-50/10 dark:bg-blue-950/10 ring-2 ring-blue-500/20"
              : "border-border/80 hover:border-blue-500/40"
          )}
        >
          <div className="h-3 w-3 rounded-full bg-blue-500 shrink-0" />
          <div className="flex flex-col">
            <span className="text-[11px] font-bold text-muted-foreground uppercase tracking-wider">Shipped</span>
            <span className="text-xl font-black text-foreground mt-0.5">{shippedCount}</span>
          </div>
        </button>

        {/* Delivered Pill */}
        <button
          onClick={() => handleCapsuleClick("Delivered")}
          className={cn(
            "bg-white dark:bg-card border rounded-2xl p-4 flex items-center gap-4 text-left transition-all hover:scale-[1.02] shadow-2xs cursor-pointer",
            activeStatus === "Delivered"
              ? "border-cyan-500 bg-cyan-50/10 dark:bg-cyan-950/10 ring-2 ring-cyan-500/20"
              : "border-border/80 hover:border-cyan-500/40"
          )}
        >
          <div className="h-3 w-3 rounded-full bg-cyan-500 shrink-0" />
          <div className="flex flex-col">
            <span className="text-[11px] font-bold text-muted-foreground uppercase tracking-wider">Delivered</span>
            <span className="text-xl font-black text-foreground mt-0.5">{deliveredCount}</span>
          </div>
        </button>

        {/* Completed Pill */}
        <button
          onClick={() => handleCapsuleClick("Completed")}
          className={cn(
            "bg-white dark:bg-card border rounded-2xl p-4 flex items-center gap-4 text-left transition-all hover:scale-[1.02] shadow-2xs cursor-pointer",
            activeStatus === "Completed"
              ? "border-emerald-500 bg-emerald-50/10 dark:bg-emerald-950/10 ring-2 ring-emerald-500/20"
              : "border-border/80 hover:border-emerald-500/40"
          )}
        >
          <div className="h-3 w-3 rounded-full bg-emerald-500 shrink-0" />
          <div className="flex flex-col">
            <span className="text-[11px] font-bold text-muted-foreground uppercase tracking-wider">Completed</span>
            <span className="text-xl font-black text-foreground mt-0.5">{completedCount}</span>
          </div>
        </button>

        {/* Canceled Pill */}
        <button
          onClick={() => handleCapsuleClick("Canceled")}
          className={cn(
            "bg-white dark:bg-card border rounded-2xl p-4 flex items-center gap-4 text-left transition-all hover:scale-[1.02] shadow-2xs cursor-pointer",
            activeStatus === "Canceled"
              ? "border-red-500 bg-red-50/10 dark:bg-red-950/10 ring-2 ring-red-500/20"
              : "border-border/80 hover:border-red-500/40"
          )}
        >
          <div className="h-3 w-3 rounded-full bg-red-500 shrink-0" />
          <div className="flex flex-col">
            <span className="text-[11px] font-bold text-muted-foreground uppercase tracking-wider">Canceled</span>
            <span className="text-xl font-black text-foreground mt-0.5">{canceledCount}</span>
          </div>
        </button>
      </div>

      {/* Main Table */}
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
              Array.from({ length: 5 }).map((_, rowIdx) => (
                <tr key={rowIdx} className="animate-pulse">
                  <td className="px-6 py-4"><div className="h-3.5 bg-muted rounded w-16" /></td>
                  <td className="px-6 py-4">
                    <div className="space-y-1.5 w-24">
                      <div className="h-3.5 bg-muted rounded" />
                      <div className="h-2.5 bg-muted rounded w-16" />
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <div className="space-y-1.5 w-24">
                      <div className="h-3.5 bg-muted rounded" />
                      <div className="h-2.5 bg-muted rounded w-20" />
                    </div>
                  </td>
                  <td className="px-6 py-4"><div className="h-5 bg-muted rounded w-16" /></td>
                  <td className="px-6 py-4"><div className="h-3.5 bg-muted rounded w-24" /></td>
                  <td className="px-6 py-4"><div className="h-3.5 bg-muted rounded w-20" /></td>
                </tr>
              ))
            ) : orders.length === 0 ? (
              <tr>
                <td colSpan={TABLE_HEADERS.length} className="px-6 py-16 text-center">
                  <div className="flex flex-col items-center justify-center space-y-3 max-w-sm mx-auto">
                    <Inbox className="h-10 w-10 text-muted-foreground/60" />
                    <h3 className="text-base font-black text-foreground">No orders found</h3>
                    <p className="text-xs text-muted-foreground font-semibold">
                      We couldn&apos;t find any orders matching the selected status filter.
                    </p>
                  </div>
                </td>
              </tr>
            ) : (
              orders.map((order) => (
                <SellerOrdersTableRow
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
            <span className="font-bold text-foreground">{totalCount}</span> orders
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
  );
}
