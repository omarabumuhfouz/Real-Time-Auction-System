"use client";

import { useRouter } from "next/navigation";
import { Gavel, Edit2 } from "lucide-react";
import { cn } from "@/lib/utils";
import { useAuthStore } from "@/stores/auth.store";
import { ROUTES } from "@/config/routes.config";
import { Button } from "@/components/ui/button";

interface PlaceBidButtonProps {
  auctionId: string;
  isOwner: boolean;
  status?: string;
  className?: string;
}

/**
 * Place Bid CTA button with auth-aware routing:
 *
 * - If the user is NOT authenticated → navigate to register page
 * - If the user IS the auction owner (via prop) → show "EDIT AUCTION"
 * - If the auction has ended → show "AUCTION ENDED" (muted)
 * - Otherwise → navigate to auction detail page to place a bid
 */
export function PlaceBidButton({
  auctionId,
  isOwner,
  status,
  className,
}: PlaceBidButtonProps) {
  const router = useRouter();
  const { isAuthenticated } = useAuthStore();
  const isEnded = status === "Ended";
  const isUpcoming = status === "Upcoming";

  const handleClick = (e: React.MouseEvent) => {
    e.preventDefault(); // Prevent link wrapping if any
    e.stopPropagation();

    if (isUpcoming) {
      router.push(ROUTES.AUCTIONS.DETAIL(auctionId));
      return;
    }

    if (!isAuthenticated) {
      router.push(ROUTES.AUTH.REGISTER);
      return;
    }

    if (isOwner) {
      router.push(`${ROUTES.AUCTIONS.DETAIL(auctionId)}/edit`);
      return;
    }

    router.push(ROUTES.AUCTIONS.DETAIL(auctionId));
  };

  const getButtonText = () => {
    if (isEnded) return "AUCTION ENDED";
    if (isOwner) return "EDIT AUCTION";
    if (isUpcoming) return "VIEW DETAILS";
    return "PLACE BID";
  };

  return (
    <Button
      onClick={handleClick}
      disabled={isEnded && !isOwner}
      variant={isUpcoming ? "outline" : "default"}
      className={cn(
        "h-[48px] w-full cursor-pointer rounded-[10px] text-base font-bold tracking-wide transition-all active:scale-[0.98]",
        isUpcoming && "border border-border text-foreground/80 hover:bg-muted hover:text-foreground",
        isEnded && !isOwner && "bg-muted text-muted-foreground hover:bg-muted cursor-not-allowed opacity-100",
        !isUpcoming && !isEnded && "bg-primary text-primary-foreground hover:bg-primary/90",
        className,
      )}
      aria-label={isOwner ? "Edit your auction" : isEnded ? "Auction has ended" : isUpcoming ? "View auction details" : "Place a bid on this auction"}
    >
      {isOwner ? (
        <Edit2 className="mr-2 size-4" aria-hidden="true" />
      ) : !isEnded && (
        <Gavel className="mr-2 size-5" aria-hidden="true" />
      )}
      {getButtonText()}
    </Button>
  );
}
