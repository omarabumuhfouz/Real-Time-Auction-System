"use client";

import { useState } from "react";
import { Inbox, Filter, ArrowUpDown, Download } from "lucide-react";
import { AuctionPagination } from "@/features/auctions";
import type { SellerAuctionSummaryDto } from "@/features/auctions";
import { SellerAuctionsTableRow } from "./SellerAuctionsTableRow";
import { DeleteConfirmationModal } from "./DeleteConfirmationModal";
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
  { value: "Active", label: "Active" },
  { value: "Pending", label: "Pending" },
  { value: "Sold", label: "Sold" },
  { value: "Unsold", label: "Unsold" },
] as const;

const SORT_OPTIONS = [
  { value: "EndTime", label: "Ending Soon" },
  { value: "CreationDate", label: "Recently Created" },
  { value: "bidsCount", label: "Most Bids" },
  { value: "lastBidAmount", label: "Highest Bid" },
] as const;

const TABLE_HEADERS = [
  { key: "auction", label: "Auction", className: "" },
  { key: "category", label: "Category", className: "" },
  { key: "status", label: "Status", className: "" },
  { key: "bids", label: "Bids", className: "" },
  { key: "currentBid", label: "Current Bid", className: "" },
  { key: "timeLeft", label: "Time Left", className: "" },
  { key: "endsAt", label: "Ends At", className: "" },
  { key: "actions", label: "Actions", className: "text-right pr-6" },
] as const;

interface SellerAuctionsTableProps {
  auctions: SellerAuctionSummaryDto[];
  totalCount: number;
  currentPage: number;
  totalPages: number;
  isLoading: boolean;
  activeStatus: string;
  sortBy: string;
  onStatusChange: (status: string) => void;
  onSortChange: (sort: string) => void;
  onPageChange: (page: number) => void;
  onDeleteAuction: (id: string) => Promise<void>;
  activeCount: number;
  pendingCount: number;
  soldCount: number;
  unsoldCount: number;
}

export function SellerAuctionsTable({
  auctions,
  totalCount,
  currentPage,
  totalPages,
  isLoading,
  activeStatus,
  sortBy,
  onStatusChange,
  onSortChange,
  onPageChange,
  onDeleteAuction,
  activeCount,
  pendingCount,
  soldCount,
  unsoldCount,
}: SellerAuctionsTableProps) {
  const [deleteTargetId, setDeleteTargetId] = useState<string | null>(null);
  const [isDeletingInProgress, setIsDeletingInProgress] = useState(false);

  const handleDeleteClick = (id: string) => {
    setDeleteTargetId(id);
  };

  const handleConfirmDelete = async () => {
    if (!deleteTargetId) return;
    setIsDeletingInProgress(true);
    try {
      await onDeleteAuction(deleteTargetId);
    } finally {
      setIsDeletingInProgress(false);
      setDeleteTargetId(null);
    }
  };

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
            Your Auctions
          </h2>
          <div className="flex items-center gap-1.5 text-emerald-500 font-bold text-xs bg-emerald-500/10 px-2.5 py-1 rounded-full">
            <span className="h-1.5 w-1.5 rounded-full bg-emerald-500 animate-pulse" />
            {activeCount} active
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
            Export Auctions
          </Button>
        </div>
      </div>

      {/* Internal Status Capsules Grid */}
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
        {/* Active Pill */}
        <button
          onClick={() => handleCapsuleClick("Active")}
          className={cn(
            "bg-white dark:bg-card border rounded-2xl p-4 flex items-center gap-4 text-left transition-all hover:scale-[1.02] shadow-2xs cursor-pointer",
            activeStatus === "Active"
              ? "border-emerald-500 bg-emerald-50/10 dark:bg-emerald-950/10 ring-2 ring-emerald-500/20"
              : "border-border/80 hover:border-emerald-500/40"
          )}
        >
          <div className="h-3 w-3 rounded-full bg-emerald-500 shrink-0" />
          <div className="flex flex-col">
            <span className="text-[11px] font-bold text-muted-foreground uppercase tracking-wider">Active</span>
            <span className="text-xl font-black text-foreground mt-0.5">{activeCount}</span>
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

        {/* Sold Pill */}
        <button
          onClick={() => handleCapsuleClick("Sold")}
          className={cn(
            "bg-white dark:bg-card border rounded-2xl p-4 flex items-center gap-4 text-left transition-all hover:scale-[1.02] shadow-2xs cursor-pointer",
            activeStatus === "Sold"
              ? "border-blue-500 bg-blue-50/10 dark:bg-blue-950/10 ring-2 ring-blue-500/20"
              : "border-border/80 hover:border-blue-500/40"
          )}
        >
          <div className="h-3 w-3 rounded-full bg-blue-500 shrink-0" />
          <div className="flex flex-col">
            <span className="text-[11px] font-bold text-muted-foreground uppercase tracking-wider">Sold</span>
            <span className="text-xl font-black text-foreground mt-0.5">{soldCount}</span>
          </div>
        </button>

        {/* Unsold Pill */}
        <button
          onClick={() => handleCapsuleClick("Unsold")}
          className={cn(
            "bg-white dark:bg-card border rounded-2xl p-4 flex items-center gap-4 text-left transition-all hover:scale-[1.02] shadow-2xs cursor-pointer",
            activeStatus === "Unsold"
              ? "border-slate-500 bg-slate-50/10 dark:bg-slate-950/10 ring-2 ring-slate-500/20"
              : "border-border/80 hover:border-slate-500/40"
          )}
        >
          <div className="h-3 w-3 rounded-full bg-slate-500 shrink-0" />
          <div className="flex flex-col">
            <span className="text-[11px] font-bold text-muted-foreground uppercase tracking-wider">Unsold</span>
            <span className="text-xl font-black text-foreground mt-0.5">{unsoldCount}</span>
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
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-3">
                      <div className="h-10 w-12 bg-muted rounded-lg shrink-0" />
                      <div className="space-y-1.5 w-24">
                        <div className="h-3.5 bg-muted rounded" />
                        <div className="h-2.5 bg-muted rounded w-16" />
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4"><div className="h-3 bg-muted rounded w-24" /></td>
                  <td className="px-6 py-4"><div className="h-5 bg-muted rounded w-16" /></td>
                  <td className="px-6 py-4"><div className="h-3 bg-muted rounded w-6" /></td>
                  <td className="px-6 py-4"><div className="h-3 bg-muted rounded w-20" /></td>
                  <td className="px-6 py-4"><div className="h-3 bg-muted rounded w-12" /></td>
                  <td className="px-6 py-4"><div className="h-3.5 bg-muted rounded w-24" /></td>
                  <td className="px-6 py-4 text-right pr-6"><div className="h-8 bg-muted rounded-lg w-20 ml-auto" /></td>
                </tr>
              ))
            ) : auctions.length === 0 ? (
              <tr>
                <td colSpan={TABLE_HEADERS.length} className="px-6 py-16 text-center">
                  <div className="flex flex-col items-center justify-center space-y-3 max-w-sm mx-auto">
                    <Inbox className="h-10 w-10 text-muted-foreground/60" />
                    <h3 className="text-base font-black text-foreground">No auctions found</h3>
                    <p className="text-xs text-muted-foreground font-semibold">
                      We couldn&apos;t find any auctions matching the selected status filter.
                    </p>
                  </div>
                </td>
              </tr>
            ) : (
              auctions.map((auction) => (
                <SellerAuctionsTableRow
                  key={auction.auctionId}
                  auction={auction}
                  onDelete={handleDeleteClick}
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
            <span className="font-bold text-foreground">{totalCount}</span> auctions
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

      {/* Premium Confirm Delete Modal */}
      <DeleteConfirmationModal
        isOpen={!!deleteTargetId}
        onClose={() => setDeleteTargetId(null)}
        onConfirm={handleConfirmDelete}
        isConfirming={isDeletingInProgress}
      />
    </div>
  );
}
