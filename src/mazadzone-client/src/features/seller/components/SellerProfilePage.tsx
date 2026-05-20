"use client";

import { useState } from "react";
import { PageWrapper } from "@/components/layout/page-wrapper";
import { MessageSquare, ShoppingBag } from "lucide-react";
import {
  useGetSellerProfile,
  useGetSellerReviews,
  useGetSellerProfileAuctions,
} from "../api/seller.queries";
import { SellerProfileHeader } from "./SellerProfileHeader";
import { SellerReviewsTab } from "./SellerReviewsTab";
import { SellerAuctionsTab } from "./SellerAuctionsTab";

interface SellerProfilePageProps {
  sellerId: string;
}

export function SellerProfilePage({ sellerId }: SellerProfilePageProps) {
  const [activeTab, setActiveTab] = useState<"reviews" | "auctions">("reviews");
  const [reviewsPage, setReviewsPage] = useState(1);
  const [auctionsPage, setAuctionsPage] = useState(1);

  const {
    data: profile,
    isLoading: isProfileLoading,
    isError: isProfileError,
    refetch: refetchProfile,
  } = useGetSellerProfile(sellerId);

  const { data: reviewsData, isLoading: isReviewsLoading } =
    useGetSellerReviews(sellerId, reviewsPage, 6);

  const { data: auctionsData, isLoading: isAuctionsLoading } =
    useGetSellerProfileAuctions(sellerId, auctionsPage, 6);

  if (isProfileError) {
    return (
      <PageWrapper>
        <div className="flex flex-col items-center justify-center gap-4 py-24">
          <p className="text-lg font-medium text-destructive">
            Failed to load seller profile
          </p>
          <button
            type="button"
            onClick={() => refetchProfile()}
            className="rounded-md bg-primary px-5 py-2 text-sm font-semibold text-primary-foreground transition-colors hover:bg-primary/90 cursor-pointer"
          >
            Try Again
          </button>
        </div>
      </PageWrapper>
    );
  }

  if (isProfileLoading || isReviewsLoading || isAuctionsLoading) {
    return (
      <PageWrapper>
        <div className="flex flex-col gap-8 px-4 py-6 sm:px-6 lg:px-8 animate-pulse">
          {/* Header Skeleton */}
          <div className="h-[200px] w-full rounded-xl bg-card border border-border" />

          {/* Tabs Skeleton */}
          <div className="flex justify-center gap-4 border-b border-border/60 pb-6">
            <div className="h-11 w-36 rounded-full bg-card border border-border" />
            <div className="h-11 w-44 rounded-full bg-card border border-border" />
          </div>

          {/* List Skeleton */}
          <div className="flex flex-col gap-4">
            <div className="h-32 w-full rounded-xl bg-card border border-border" />
            <div className="h-32 w-full rounded-xl bg-card border border-border" />
            <div className="h-32 w-full rounded-xl bg-card border border-border" />
          </div>
        </div>
      </PageWrapper>
    );
  }

  if (!profile) {
    return (
      <PageWrapper>
        <div className="flex flex-col items-center justify-center gap-2 py-24">
          <p className="text-lg font-medium text-muted-foreground">
            Seller not found
          </p>
          <p className="text-sm text-muted-foreground">
            The requested seller profile does not exist or has been deactivated.
          </p>
        </div>
      </PageWrapper>
    );
  }

  const totalReviews = reviewsData?.totalCount ?? 0;
  const totalAuctions = auctionsData?.totalCount ?? 0;

  return (
    <PageWrapper>
      <div className="flex flex-col gap-8 px-4 py-6 sm:px-6 lg:px-8">
        {/* Profile Card Header */}
        <SellerProfileHeader profile={profile} />

        {/* Tab Buttons Container */}
        <div className="flex justify-center gap-4 border-b border-border/60 pb-6">
          <button
            type="button"
            onClick={() => setActiveTab("reviews")}
            className={
              activeTab === "reviews"
                ? "flex items-center gap-2 bg-primary text-primary-foreground px-6 py-2.5 rounded-full text-sm font-bold shadow-xs hover:bg-primary/95 transition-all cursor-pointer"
                : "flex items-center gap-2 bg-card hover:bg-muted text-foreground px-6 py-2.5 rounded-full text-sm font-bold border border-border transition-all cursor-pointer"
            }
          >
            <MessageSquare className="size-4 shrink-0" />
            <span>Reviews ({totalReviews})</span>
          </button>

          <button
            type="button"
            onClick={() => setActiveTab("auctions")}
            className={
              activeTab === "auctions"
                ? "flex items-center gap-2 bg-primary text-primary-foreground px-6 py-2.5 rounded-full text-sm font-bold shadow-xs hover:bg-primary/95 transition-all cursor-pointer"
                : "flex items-center gap-2 bg-card hover:bg-muted text-foreground px-6 py-2.5 rounded-full text-sm font-bold border border-border transition-all cursor-pointer"
            }
          >
            <ShoppingBag className="size-4 shrink-0" />
            <span>Seller Auctions ({totalAuctions})</span>
          </button>
        </div>

        {/* Active Tab Panel */}
        <div className="transition-all duration-300">
          {activeTab === "reviews" ? (
            <SellerReviewsTab
              sellerId={sellerId}
              reviews={reviewsData?.items ?? []}
              currentPage={reviewsPage}
              totalPages={reviewsData?.totalPages ?? 1}
              onPageChange={setReviewsPage}
            />
          ) : (
            <SellerAuctionsTab
              auctions={auctionsData?.items ?? []}
              currentPage={auctionsPage}
              totalPages={auctionsData?.totalPages ?? 1}
              onPageChange={setAuctionsPage}
            />
          )}
        </div>
      </div>
    </PageWrapper>
  );
}
