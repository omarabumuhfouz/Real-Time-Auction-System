"use client";

import Link from "next/link";
import { MoveRight } from "lucide-react";
import { AuctionCard, AuctionCardSkeleton } from "../auction-card";
import { useGetUpcomingAuctions } from "../../api";
import { ROUTES } from "@/config/routes.config";

export function UpcomingAuctionsSection() {
  const { data: auctions, isLoading, isError } = useGetUpcomingAuctions(4);

  if (isError) return null;

  return (
    <section className="w-full py-8 mt-6">
      <div className="flex items-center justify-between mb-6 pb-4 border-b border-border/40">
        <h2 className="text-2xl md:text-3xl font-bold tracking-tight text-foreground/80">
          Upcoming Auctions
        </h2>
        <Link
          href={`${ROUTES.AUCTIONS.LIST}?status=Upcoming`}
          className="flex items-center gap-2 text-primary/80 font-semibold hover:gap-3 transition-all text-sm md:text-base"
        >
          View All <MoveRight className="size-5" />
        </Link>
      </div>

      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
        {isLoading
          ? Array.from({ length: 4 }).map((_, i) => (
              <AuctionCardSkeleton key={i} />
            ))
          : auctions?.map((auction) => (
              <AuctionCard
                key={auction.id}
                auction={auction}
              />
            ))}
      </div>
    </section>
  );
}
