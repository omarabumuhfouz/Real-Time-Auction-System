import { Gavel } from "lucide-react";
import { BidHistoryRow } from "./BidHistoryRow";
import type { BidHistoryEntry } from "../../types/auction.types";

interface BidHistoryProps {
  bidHistory: BidHistoryEntry[];
  totalBids: number;
}

export function BidHistory({ bidHistory, totalBids }: BidHistoryProps) {
  return (
    <div className="rounded-xl border border-border bg-card p-5">
      {/* Header */}
      <div className="flex items-center justify-between mb-3">
        <div className="flex items-center gap-2">
          <Gavel className="size-4 text-primary" aria-hidden="true" />
          <h2 className="text-base font-bold text-foreground">
            Bid History
          </h2>
        </div>
        <span className="text-xs text-muted-foreground">
          {totalBids} bids
        </span>
      </div>

      {/* Bid list */}
      <div className="max-h-[560px] overflow-y-auto pr-1 scrollbar-thin scrollbar-thumb-primary/20 hover:scrollbar-thumb-primary/40">
        {bidHistory.length > 0 ? (
          bidHistory.map((entry) => (
            <BidHistoryRow key={entry.id} entry={entry} />
          ))
        ) : (
          <div className="flex flex-col items-center justify-center py-8 text-center">
            <Gavel className="size-6 text-muted-foreground/30 mb-2" />
            <h3 className="text-sm font-bold text-foreground">
              No Bids Yet
            </h3>
            <p className="mt-1 text-xs text-muted-foreground">
              Be the first bidder and start the action!
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
