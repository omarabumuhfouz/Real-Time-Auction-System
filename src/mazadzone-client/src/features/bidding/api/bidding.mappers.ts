import { parseUtcDate } from "@/utils/date.utils";
import type { BidActivity, BidStatus } from "../types/bidding.types";
import type { MyBidAuctionDto } from "./bidding.contracts";

/**
 * Maps backend enum integers to standard frontend BidStatus strings.
 */
function mapIntegerToBidStatus(statusNum: number): BidStatus {
  switch (statusNum) {
    case 0:
      return "Leading";
    case 1:
      return "Outbid";
    case 2:
      return "Won";
    case 3:
      return "Lost";
    default:
      return "Leading";
  }
}

/**
 * Maps raw backend MyBidAuctionDto to presentation-safe BidActivity ViewModels.
 */
export function mapMyBidAuctionDtoToBidActivity(
  dto: MyBidAuctionDto,
): BidActivity {
  return {
    id: `bid-${dto.auctionId}-${dto.yourBidAmount}`,
    yourBid: dto.yourBidAmount,
    status: mapIntegerToBidStatus(dto.yourBidStatus),
    auction: {
      id: dto.auctionId,
      title: dto.itemTitle,
      imageUrl: dto.imageUrl,
      category: "Tech and Electronics",
      subcategory: "Others",
      condition: "New",
      status:
        dto.auctionStatus === 1
          ? "Upcoming"
          : dto.auctionStatus === 2
            ? "Active"
            : dto.auctionStatus === 3 || dto.auctionStatus === 4
              ? "Ended"
              : "Active",
      pricing: {
        startingPrice: dto.currentBidAmount,
        currentBid: dto.currentBidAmount > 0 ? dto.currentBidAmount : null,
        bidCount: dto.bidsCount,
      },
      timing: {
        startDate: parseUtcDate(dto.startTime),
        endDate: parseUtcDate(dto.endTime),
        creationDate: dto.startTime,
      },
      isFavorite: false,
      isOwner: false,
      images: dto.imageUrl ? [dto.imageUrl] : [],
      bidHistory: [],
    },
  };
}
