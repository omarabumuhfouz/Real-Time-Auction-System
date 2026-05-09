"use client";

import { useState, useMemo } from "react";
import { PageWrapper } from "@/components/layout/page-wrapper";
import { AuctionCard } from "./AuctionCard";

/**
 * Auctions page-level component.
 *
 * This is the main entry point rendered by `app/(main)/auctions/page.tsx`.
 * It owns the page layout, data fetching orchestration, and feature composition.
 *
 * TODO: Wire up useGetAuctions, filters, and AuctionCard grid.
 */
export function AuctionsPage() {
  const [isFavorite, setIsFavorite] = useState(false);

  // Stable demo date: 2 days from May 9th, 2026
  const demoEndDate = "2026-05-11T12:00:00Z";

  return (
    <PageWrapper>
      <div className="space-y-6">
        {/* Page header */}
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Auctions</h1>
          <p className="text-muted-foreground">
            Browse live and upcoming auctions
          </p>
        </div>

        {/* TODO: Filter bar */}

        {/* Demo: AuctionCard with Luxury watch 2020 data */}
        <div className="flex flex-wrap gap-6">
          <AuctionCard
            id="auction-001"
            sellerId="seller-xyz"
            title="Luxury watch 2020"
            imageUrl="/mock-images/auctions/auction_num_1.jpg"
            currentBid={38500}
            bidCount={25}
            endDate={demoEndDate}
            isFavorite={isFavorite}
            onFavoriteClick={() => setIsFavorite((prev) => !prev)}
          />
        </div>
      </div>
    </PageWrapper>
  );
}
