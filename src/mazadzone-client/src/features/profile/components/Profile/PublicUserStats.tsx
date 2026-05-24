import { TrendingUp, Trophy, CheckSquare, Gavel } from "lucide-react";
import type { PublicUserProfile } from "../../types/user-profile.types";
import { cn } from "@/lib/utils";

interface PublicUserStatsProps {
  profile: PublicUserProfile;
  isCompact?: boolean;
}

export function PublicUserStats({ profile, isCompact = false }: PublicUserStatsProps) {
  const stats = [
    {
      label: "Auctions Participated",
      value: profile.biddingActivityCount,
      icon: TrendingUp,
      description: "Total public bidding participation history",
      hoverBorder: "hover:border-primary/30",
      iconColor: "text-primary",
      bgIcon: "bg-primary/10",
    },
    {
      label: "Total Bids Placed",
      value: profile.bidsPlacedCount,
      icon: Gavel,
      description: "Total number of bids placed across all auctions",
      hoverBorder: "hover:border-purple-500/30",
      iconColor: "text-purple-600 dark:text-purple-400",
      bgIcon: "bg-purple-500/10",
    },
    {
      label: "Auctions Won",
      value: profile.wonAuctionsCount,
      icon: Trophy,
      description: "Highest bidder standing at auction end",
      hoverBorder: "hover:border-amber-500/30",
      iconColor: "text-amber-500",
      bgIcon: "bg-amber-500/10",
    },
    {
      label: "Completed Purchases",
      value: profile.completedPurchasesCount,
      icon: CheckSquare,
      description: "Won listings successfully checked out",
      hoverBorder: "hover:border-sky-500/30",
      iconColor: "text-sky-600 dark:text-sky-400",
      bgIcon: "bg-sky-500/10",
    },
  ];

  return (
    <div className={cn("grid grid-cols-1 gap-4", isCompact ? "sm:grid-cols-2 lg:grid-cols-4" : "md:grid-cols-2 lg:grid-cols-4")}>
      {stats.map((stat, idx) => {
        const Icon = stat.icon;
        return (
          <div
            key={idx}
            className={cn(
              "flex items-center justify-between rounded-2xl border border-border bg-card p-5 shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-md",
              stat.hoverBorder
            )}
          >
            <div className="flex flex-col gap-1">
              <span className="text-2xl font-black text-foreground tracking-tight">
                {stat.value}
              </span>
              <span className="text-sm font-bold text-foreground">
                {stat.label}
              </span>
              {!isCompact && (
                <span className="text-xs text-muted-foreground mt-0.5">
                  {stat.description}
                </span>
              )}
            </div>
            <div className={cn("rounded-xl p-3 shadow-xs border border-border/40 shrink-0", stat.bgIcon, stat.iconColor)}>
              <Icon className="size-6 shrink-0 " />
            </div>
          </div>
        );
      })}
    </div>
  );
}
