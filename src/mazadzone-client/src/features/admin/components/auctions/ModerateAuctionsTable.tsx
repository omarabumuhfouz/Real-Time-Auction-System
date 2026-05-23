import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { ROUTES } from "@/config/routes.config";

import {
  MoreHorizontal,
  ArrowUpDown,
  Eye,
  XCircle,
  Loader2,
  Gavel,
  AlertTriangle,
} from "lucide-react";
import { format } from "date-fns";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Checkbox } from "@/components/ui/checkbox";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { cn } from "@/lib/utils";
import type { ModerateAuction, AuctionStatus } from "../../types/admin.types";
import { MODERATE_AUCTION_COLUMNS, AUCTION_DROPDOWN_ITEMS } from "../../constants/moderate-auctions.constants";
import { ModerateUsersPagination } from "../users/ModerateUsersPagination";
import { CancelAuctionDialog } from "./CancelAuctionDialog";

interface ModerateAuctionsTableProps {
  auctions: ModerateAuction[];
  isLoading: boolean;
  page: number;
  pageSize: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
  selectedIds: string[];
  onSelectAll: (checked: boolean) => void;
  onSelectRow: (id: string, checked: boolean) => void;
}

/** Returns design-token Tailwind classes for each auction status */
function getStatusBadgeClass(status: AuctionStatus): string {
  switch (status) {
    case "Active":
      return "bg-success text-success-foreground border-success";
    case "Pending":
      return "bg-upcoming text-upcoming-foreground border-upcoming";
    case "Ended":
      return "bg-muted text-muted-foreground border-muted";
    case "Cancelled":
      return "bg-warning text-warning-foreground border-warning";
    default:
      return "bg-secondary text-secondary-foreground border-secondary";
  }
}

/** Currency formatter */
function formatBid(amount: number, currency: string): string {
  if (amount === 0) return "$0";
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: currency || "USD",
    maximumFractionDigits: 0,
  }).format(amount);
}

export function ModerateAuctionsTable({
  auctions,
  isLoading,
  page,
  pageSize,
  totalPages,
  onPageChange,
  onPageSizeChange,
  selectedIds,
  onSelectAll,
  onSelectRow,
}: ModerateAuctionsTableProps) {
  const router = useRouter();
  const [actionAuction, setActionAuction] = useState<ModerateAuction | null>(null);
  const [actionType, setActionType] = useState<"cancel" | null>(null);

  const handleActionClick = (
    type: "view" | "cancel",
    auction: ModerateAuction
  ) => {
    if (type === "cancel") {
      setActionAuction(auction);
      setActionType("cancel");
    } else if (type === "view") {
      router.push(ROUTES.AUCTIONS.DETAIL(auction.id));
    }
  };

  const isAllSelected =
    auctions.length > 0 && selectedIds.length === auctions.length;
  const isSomeSelected =
    selectedIds.length > 0 && selectedIds.length < auctions.length;

  /** Generate context-aware action buttons per row */
  const getActionButtons = (auction: ModerateAuction) => {
    const actions: {
      label: string;
      icon: React.ComponentType<{ className?: string }>;
      variant: "default" | "outline";
      className: string;
      type: "view" | "cancel";
    }[] = [];

    // View is always present
    actions.push({
      label: "View",
      icon: Eye,
      variant: "default",
      className:
        "bg-primary/10 text-primary hover:bg-primary/20 shadow-none border-transparent w-[60px]",
      type: "view",
    });

    if (auction.status === "Active" || auction.status === "Pending") {
      actions.push({
        label: "Cancel",
        icon: XCircle,
        variant: "outline",
        className:
          "text-destructive border-destructive/20 bg-destructive/10 hover:bg-destructive/20 shadow-none w-[65px]",
        type: "cancel",
      });
    }

    return actions;
  };

  return (
    <div className="bg-card border border-border rounded-xl flex flex-col w-full overflow-hidden shadow-xs">
      <div className="overflow-x-auto w-full">
        <Table>
          <TableHeader className="bg-muted/30">
            <TableRow className="hover:bg-transparent border-b-border">
              <TableHead className="w-12 text-center pl-4">
                <Checkbox
                  className="rounded-[4px] border-muted-foreground/30 data-[state=checked]:bg-primary"
                  checked={isAllSelected || (isSomeSelected ? "indeterminate" : false)}
                  onCheckedChange={(checked) => onSelectAll(checked as boolean)}
                />
              </TableHead>
              {MODERATE_AUCTION_COLUMNS.map((col) => (
                <TableHead
                  key={col.key}
                  className={cn(
                    "text-xs font-bold text-muted-foreground uppercase tracking-wider whitespace-nowrap",
                    col.align === "right" && "text-right pr-6"
                  )}
                >
                  {col.sortable ? (
                    <div className="flex items-center gap-1.5 cursor-pointer hover:text-foreground">
                      {col.label}
                      <ArrowUpDown className="h-3 w-3" />
                    </div>
                  ) : (
                    col.label
                  )}
                </TableHead>
              ))}
            </TableRow>
          </TableHeader>

          <TableBody>
            {isLoading ? (
              // Skeleton rows
              Array.from({ length: 8 }).map((_, idx) => (
                <TableRow key={idx} className="border-b-border">
                  <TableCell className="w-12 pl-4">
                    <div className="h-4 w-4 rounded bg-muted/50 animate-pulse" />
                  </TableCell>
                  {MODERATE_AUCTION_COLUMNS.map((col) => (
                    <TableCell key={col.key}>
                      <div className="h-4 rounded bg-muted/50 animate-pulse w-24" />
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : auctions.length === 0 ? (
              <TableRow>
                <TableCell
                  colSpan={MODERATE_AUCTION_COLUMNS.length + 1}
                  className="h-40 text-center"
                >
                  <div className="flex flex-col items-center gap-2 text-muted-foreground">
                    <Gavel className="h-8 w-8 opacity-30" />
                    <p className="text-sm font-medium">No auctions found.</p>
                    <p className="text-xs opacity-70">
                      Try adjusting your filters or search query.
                    </p>
                  </div>
                </TableCell>
              </TableRow>
            ) : (
              auctions.map((auction) => (
                <TableRow
                  key={auction.id}
                  className="hover:bg-muted/10 border-b-border group"
                >
                  {/* Checkbox */}
                  <TableCell className="w-12 text-center pl-4">
                    <Checkbox
                      className="rounded-[4px] border-muted-foreground/30 data-[state=checked]:bg-primary"
                      checked={selectedIds.includes(auction.id)}
                      onCheckedChange={(checked) =>
                        onSelectRow(auction.id, checked as boolean)
                      }
                    />
                  </TableCell>

                  {/* Auction: image + title + ID */}
                  <TableCell>
                    <div className="flex items-center gap-3">
                      <div className="h-11 w-11 rounded-lg bg-muted/50 border border-border flex items-center justify-center overflow-hidden shrink-0">
                        {auction.imageUrl ? (
                          <img
                            src={auction.imageUrl}
                            alt={auction.title}
                            className="h-full w-full object-cover"
                          />
                        ) : (
                          <Gavel className="h-4 w-4 text-muted-foreground/50" />
                        )}
                      </div>
                      <div className="flex flex-col min-w-0">
                        <span className="text-sm font-semibold text-foreground truncate max-w-[180px]">
                          {auction.title}
                        </span>
                        <span className="text-[11px] text-muted-foreground font-medium">
                          ID: {auction.id}
                        </span>
                      </div>
                    </div>
                  </TableCell>

                  {/* Seller */}
                  <TableCell>
                    <div className="flex flex-col min-w-0">
                      <span className="text-[13px] font-semibold text-foreground whitespace-nowrap">
                        {auction.sellerName}
                      </span>
                      <span className="text-[11px] text-muted-foreground whitespace-nowrap">
                        {auction.sellerEmail}
                      </span>
                    </div>
                  </TableCell>

                  {/* Category */}
                  <TableCell>
                    <span className="text-[13px] text-muted-foreground whitespace-nowrap">
                      {auction.category}
                    </span>
                  </TableCell>

                  {/* Status */}
                  <TableCell>
                    <Badge
                      variant="outline"
                      className={cn(
                        "rounded-md px-2.5 py-0.5 text-[11px] font-semibold tracking-wide min-w-[72px] text-center justify-center",
                        getStatusBadgeClass(auction.status)
                      )}
                    >
                      {auction.status}
                    </Badge>
                  </TableCell>

                  {/* Current Bid */}
                  <TableCell>
                    <div className="flex flex-col">
                      <span className="text-[13px] font-semibold text-foreground whitespace-nowrap">
                        {formatBid(auction.currentBid, auction.currency)}
                      </span>
                      <span className="text-[11px] text-muted-foreground">
                        {auction.currency}
                      </span>
                    </div>
                  </TableCell>

                  {/* Bid Count */}
                  <TableCell>
                    <div className="flex flex-col">
                      <span className="text-[13px] font-semibold text-foreground">
                        {auction.bidCount}
                      </span>
                      <span className="text-[11px] text-muted-foreground">
                        Bids
                      </span>
                    </div>
                  </TableCell>

                  {/* Start Date */}
                  <TableCell>
                    <div className="flex flex-col">
                      <span className="text-[13px] font-medium text-foreground whitespace-nowrap">
                        {format(new Date(auction.startDate), "MMM d, yyyy")}
                      </span>
                      <span className="text-[11px] text-muted-foreground whitespace-nowrap">
                        {format(new Date(auction.startDate), "h:mm a")}
                      </span>
                    </div>
                  </TableCell>

                  {/* End Date */}
                  <TableCell>
                    <div className="flex flex-col">
                      <span className="text-[13px] font-medium text-foreground whitespace-nowrap">
                        {format(new Date(auction.endDate), "MMM d, yyyy")}
                      </span>
                      <span className="text-[11px] text-muted-foreground whitespace-nowrap">
                        {format(new Date(auction.endDate), "h:mm a")}
                      </span>
                    </div>
                  </TableCell>

                  {/* Actions */}
                  <TableCell className="text-right pr-6 py-2">
                    <div className="flex items-center justify-end gap-1.5">
                      {getActionButtons(auction).map((action, idx) => {
                        const Icon = action.icon;
                        return (
                          <Button
                            key={idx}
                            variant={action.variant}
                            onClick={() => handleActionClick(action.type, auction)}
                            className={cn(
                              "flex flex-col items-center justify-center gap-1 h-[52px] px-2 shadow-none rounded-lg shrink-0 cursor-pointer",
                              action.className
                            )}
                          >
                            <Icon className="h-4 w-4" />
                            <span className="text-[10px] font-semibold leading-none">
                              {action.label}
                            </span>
                          </Button>
                        );
                      })}

                      {/* Three-dot menu */}
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-[52px] w-8 rounded-lg text-muted-foreground hover:bg-muted/50 shrink-0"
                          >
                            <MoreHorizontal className="h-4 w-4" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent
                          align="end"
                          className="w-36 bg-card text-foreground border-border"
                          sideOffset={5}
                        >
                          {AUCTION_DROPDOWN_ITEMS.map((item, idx) => (
                            <DropdownMenuItem
                              key={idx}
                              className={cn("text-xs cursor-pointer", item.className)}
                              onClick={() => {
                                if (item.label === "View Details") {
                                  router.push(ROUTES.AUCTIONS.DETAIL(auction.id));
                                } else if (item.label === "Edit Auction") {
                                  router.push(ROUTES.SELLER.EDIT_AUCTION(auction.id));
                                }
                              }}
                            >
                              {item.label}
                            </DropdownMenuItem>
                          ))}
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </div>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>

      {/* Pagination */}
      <ModerateUsersPagination
        page={page}
        pageSize={pageSize}
        totalPages={totalPages}
        onPageChange={onPageChange}
        onPageSizeChange={onPageSizeChange}
      />

      {/* Cancel Dialog */}
      <CancelAuctionDialog
        isOpen={actionAuction !== null && actionType === "cancel"}
        onClose={() => {
          setActionAuction(null);
          setActionType(null);
        }}
        auction={actionAuction}
      />
    </div>
  );
}
