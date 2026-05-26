import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";

interface BidStatusBadgeProps {
  status: string;
  className?: string;
}

export function BidStatusBadge({ status, className }: BidStatusBadgeProps) {
  const isLeading = status === "Leading";
  const isOutbid = status === "Outbid";
  const isWon = status === "Won";
  const isLost = status === "Lost" || status === "Ended";

  return (
    <Badge
      variant="outline"
      className={cn(
        "rounded-full font-medium border-none w-24 h-8 text-lg flex items-center justify-center shrink-0",
        isLeading && "bg-success text-success-foreground",
        isWon && "bg-success text-success-foreground",
        isOutbid && "bg-warning text-warning-foreground",
        isLost && "bg-muted text-muted-foreground",
        className
      )}
    >
      {status}
    </Badge>
  );
}
