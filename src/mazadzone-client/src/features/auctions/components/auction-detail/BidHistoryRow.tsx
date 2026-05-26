import Link from "next/link";
import { cn } from "@/lib/utils";
import { formatCurrency } from "@/utils/currency.utils";
import type { BidHistoryEntry } from "../../types/auction.types";

interface BidHistoryRowProps {
  entry: BidHistoryEntry;
}

export function BidHistoryRow({ entry }: BidHistoryRowProps) {
  const bidderId = entry.bidderId || `bidder-${entry.bidderName.toLowerCase().replace(/\s/g, "-").replace(/\./g, "")}`;

  return (
    <div
      className={cn(
        "flex items-center justify-between px-3 py-3 border-b border-border last:border-0 rounded-lg transition-colors",
        entry.isHighest && "bg-primary/5 ring-1 ring-primary/20 shadow-[0_0_15px_rgba(var(--primary),0.05)]"
      )}
    >
      <div className="flex items-center gap-3">
        {/* Avatar */}
        <Link
          href={`/users/${bidderId}`}
          className={cn(
            "flex h-9 w-9 items-center justify-center rounded-full text-sm font-bold select-none cursor-pointer transition-all hover:scale-105 ring-2 ring-transparent hover:ring-primary/50",
            entry.isHighest ? "bg-primary text-primary-foreground" : "bg-muted text-foreground"
          )}
        >
          {entry.bidderInitial}
        </Link>
        <div className="flex flex-col">
          <div className="flex items-center gap-1.5">
            <Link
              href={`/users/${bidderId}`}
              className="text-sm font-semibold text-foreground hover:text-primary transition-colors cursor-pointer"
            >
              {entry.bidderName}
            </Link>
            {entry.isHighest && (
              <span className="text-[10px] font-bold text-primary-foreground bg-primary px-2 py-0.5 rounded-full leading-none shadow-sm">
                Highest Bid
              </span>
            )}
          </div>
          <span className="text-xs text-muted-foreground">{entry.timeAgo}</span>
        </div>
      </div>

      <span
        className={cn(
          "text-sm font-bold",
          entry.isHighest ? "text-primary text-base scale-105 transition-transform" : "text-foreground",
        )}
      >
        {formatCurrency(entry.amount)}
      </span>
    </div>
  );
}
