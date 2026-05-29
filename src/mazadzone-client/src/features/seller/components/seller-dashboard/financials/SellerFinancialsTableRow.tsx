"use client";

import { format } from "date-fns";
import { Badge } from "@/components/ui/badge";
import { formatCurrency } from "@/utils/currency.utils";
import type { SellerOrderSummaryDto } from "@/features/auctions";

interface SellerFinancialsTableRowProps {
  order: SellerOrderSummaryDto;
}

export function SellerFinancialsTableRow({ order }: SellerFinancialsTableRowProps) {
  
  // Format Order ID for visual elegance
  const shortTransactionId = order.orderId.substring(0, 8).toUpperCase();

  // Compute standard 10% platform fee for local visual breakdown
  const grossAmount = order.totalAmount;
  const platformFee = grossAmount * 0.10; // 10% standard platform fee
  const netProfit = grossAmount - platformFee;

  return (
    <tr className="hover:bg-accent/20 dark:hover:bg-accent/5 transition-colors h-[64px]">
      
      {/* Column 1: Transaction ID */}
      <td className="px-6 py-3 font-mono font-black text-xs text-foreground tracking-wider">
        #{shortTransactionId}
      </td>

      {/* Column 2: Auction Title */}
      <td className="px-6 py-3 min-w-[200px]">
        <div className="text-left font-black text-xs text-foreground truncate max-w-[220px]">
          {order.auctionTitle}
        </div>
      </td>

      {/* Column 3: Completion Date */}
      <td className="px-6 py-3">
        <div className="space-y-0.5 text-left text-[11px] font-semibold">
          <div className="text-foreground">
            {format(new Date(order.orderDateUtc), "MMM d, yyyy")}
          </div>
          <div className="text-[10px] text-muted-foreground">
            {format(new Date(order.orderDateUtc), "h:mm a")}
          </div>
        </div>
      </td>

      {/* Column 4: Gross Revenue */}
      <td className="px-6 py-3 font-black text-xs text-foreground">
        {formatCurrency(grossAmount)}
      </td>

      {/* Column 5: Platform Fee (10%) */}
      <td className="px-6 py-3 font-semibold text-xs text-red-500">
        -{formatCurrency(platformFee)}
      </td>

      {/* Column 6: Net Profit */}
      <td className="px-6 py-3 font-black text-xs text-emerald-500">
        {formatCurrency(netProfit)}
      </td>

      {/* Column 7: Status */}
      <td className="px-6 py-3 pr-6 text-right">
        <Badge className="px-2.5 py-0.5 rounded-full border shadow-none text-[10px] font-black uppercase tracking-wider bg-emerald-50 dark:bg-emerald-950/20 text-emerald-600 dark:text-emerald-400 border-emerald-100 dark:border-emerald-900/30">
          Completed
        </Badge>
      </td>
    </tr>
  );
}
