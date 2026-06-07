import { parseUtcDate } from "@/utils/date.utils";
import type { BidActivity, BidStatus } from "../types/bidding.types";
import type { MyBidAuctionDto } from "./bidding.contracts";
import { AuctionStatus, type AuctionStatus as AuctionStatusValue } from "@/features/auctions";

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

function mapDtoToAuctionStatus(dto: MyBidAuctionDto): AuctionStatusValue {
  const startDate = parseUtcDate(dto.startTime);
  const endDate = parseUtcDate(dto.endTime);
  const now = Date.now();

  if (!Number.isNaN(endDate.getTime()) && now >= endDate.getTime()) {
    return AuctionStatus.ENDED;
  }

  if (!Number.isNaN(startDate.getTime()) && now < startDate.getTime()) {
    return AuctionStatus.UPCOMING;
  }

  if (dto.auctionStatus === 1) {
    return AuctionStatus.UPCOMING;
  }

  if (dto.auctionStatus === 3 || dto.auctionStatus === 4) {
    return AuctionStatus.ENDED;
  }

  return AuctionStatus.ACTIVE;
}

function normalizeBidStatusForAuction(
  bidStatus: BidStatus,
  auctionStatus: AuctionStatusValue,
): BidStatus {
  if (auctionStatus === AuctionStatus.ENDED) {
    if (bidStatus === "Leading") {
      return "Won";
    }

    if (bidStatus === "Outbid") {
      return "Lost";
    }
  }

  return bidStatus;
}

/**
 * Maps raw backend MyBidAuctionDto to presentation-safe BidActivity ViewModels.
 */
export function mapMyBidAuctionDtoToBidActivity(
  dto: MyBidAuctionDto,
): BidActivity {
  const auctionStatus = mapDtoToAuctionStatus(dto);
  const bidStatus = normalizeBidStatusForAuction(
    mapIntegerToBidStatus(dto.yourBidStatus),
    auctionStatus,
  );

  return {
    id: `bid-${dto.auctionId}-${dto.yourBidAmount}`,
    yourBid: dto.yourBidAmount,
    status: bidStatus,
    auction: {
      id: dto.auctionId,
      title: dto.itemTitle,
      imageUrl: dto.imageUrl,
      category: "Tech and Electronics",
      subcategory: "Others",
      condition: "New",
      description: "",
      conditionDescription: "",
      status: auctionStatus,
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
