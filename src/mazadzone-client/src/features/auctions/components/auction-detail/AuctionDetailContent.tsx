"use client";

import dynamic from "next/dynamic";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { AuctionImageGallery } from "./image-gallery";
import { AuctionMainInfo } from "./AuctionMainInfo";
import { SellerInfo } from "./SellerInfo";
import { BidHistory } from "./BidHistory";
import { ItemDetailsTab } from "./ItemDetailsTab";
import { SimilarItems } from "./SimilarItems";
import { ROUTES } from "@/config/routes.config";
import type { AuctionSummary, Seller } from "../../types/auction.types";

const PlaceBidModal = dynamic(
  () => import("@/features/bidding").then((module) => module.PlaceBidModal),
);

interface AuctionDetailContentProps {
  auction: AuctionSummary;
}

/**
 * Orchestrator component for the Auction Detail view.
 * Responsible for state management and layout organization.
 */
export function AuctionDetailContent({
  auction,
}: AuctionDetailContentProps) {
  const router = useRouter();
  const [isFavorite, setIsFavorite] = useState(auction.isFavorite);
  const [isBidModalOpen, setIsBidModalOpen] = useState(false);

  // ---------------------------------------------------------------------------
  // Mock Data & Derivatives
  // ---------------------------------------------------------------------------
  
  // Prefer the seller data from the API response, fallback to mock details if unavailable
  const seller: Seller = auction.seller || {
    id: "seller-123",
    fullName: "Ahmad Al-Rashid",
    email: "ahmad@mazadzone.com",
    role: "seller",
    isVerified: true,
    avatarInitial: "A",
    reviews: 277,
    rating: 4.6,
  };

  const fullDescription = `This exceptional ${auction.title} represents the pinnacle of craftsmanship. Featuring premium-grade materials, meticulously hand-finished details, and a design that seamlessly blends timeless elegance with modern sensibility. The item has been carefully maintained and is in ${auction.condition.toLowerCase()} condition. All original accessories and documentation are included. This is a rare opportunity to acquire a sought-after piece in the ${auction.category} category.`;

  const conditionDescription = `The item is in ${auction.condition.toLowerCase()} condition with minimal signs of wear. All functional components have been tested and verified to be working perfectly. There are no major scratches, dents, or structural issues. The original packaging is included and in good shape.`;

  // ---------------------------------------------------------------------------
  // Handlers
  // ---------------------------------------------------------------------------

  const handlePlaceBid = () => {
    // Temporarily disabled check for testing the Place Bid Modal as a guest
    // if (!isAuthenticated) {
    //   router.push(ROUTES.AUTH.REGISTER);
    //   return;
    // }

    if (auction.isOwner) {
      router.push(`${ROUTES.AUCTIONS.DETAIL(auction.id)}/edit`);
      return;
    }

    setIsBidModalOpen(true);
  };

  const handleShare = async () => {
    try {
      if (navigator.share) {
        await navigator.share({ title: auction.title, url: window.location.href });
      } else {
        await navigator.clipboard.writeText(window.location.href);
      }
    } catch (err) {
      console.error("Failed to share:", err);
    }
  };

  const handleFavoriteToggle = () => {
    setIsFavorite((prev) => !prev);
    // TODO: Persistence call
  };

  return (
    <>
      <div className="grid grid-cols-1 gap-8 lg:grid-cols-[1fr_520px]">
        {/* --- Left Column: Images & Technical Details --- */}
        <div className="flex flex-col gap-8">
          <AuctionImageGallery images={auction.images} title={auction.title} />

          <ItemDetailsTab
            category={auction.category}
            subcategory={auction.subcategory}
            description={fullDescription}
            condition={auction.condition}
            conditionDescription={conditionDescription}
          />
        </div>

        {/* --- Right Column: Core Info & Bidding --- */}
        <div className="flex flex-col gap-[26px]">
          <AuctionMainInfo
            auction={auction}
            isFavorite={isFavorite}
            onFavoriteToggle={handleFavoriteToggle}
            onPlaceBid={handlePlaceBid}
            onShare={handleShare}
          />

          <SellerInfo seller={seller} />

          <BidHistory
            bidHistory={auction.bidHistory}
            totalBids={auction.pricing.bidCount}
          />
        </div>
      </div>

      {/* --- Similar Items Section --- */}
      <SimilarItems 
        auctionId={auction.id}
        category={auction.category}
        subcategory={auction.subcategory}
      />

      <PlaceBidModal
        auctionId={auction.id}
        auctionTitle={auction.title}
        currentBid={auction.pricing.currentBid ?? auction.pricing.startingPrice}
        minIncrement={
          (auction.pricing.currentBid ?? auction.pricing.startingPrice) < 100 ? 5 :
          (auction.pricing.currentBid ?? auction.pricing.startingPrice) < 1000 ? 50 :
          (auction.pricing.currentBid ?? auction.pricing.startingPrice) < 10000 ? 100 : 250
        }
        isOpen={isBidModalOpen}
        onClose={() => setIsBidModalOpen(false)}
      />
    </>
  );
}
