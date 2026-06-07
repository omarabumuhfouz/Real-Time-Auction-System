/**
 * Pure mapping functions for the Orders feature.
 * Decouples raw backend DTO models from presentation ViewModels.
 */

import type { AuctionSummary } from "@/features/auctions";
import type { CheckoutAddress } from "../types/checkout.types";
import type { OrderActivity, OrderStatus } from "../types/orders.types";
import type {
  AddressDto,
  OrderDetailsDto,
  OrderSummaryDto,
  WonOrderSummaryDto,
} from "./order.contracts";

/**
 * Helper to normalize backend status strings into stable UI OrderStatus strings.
 */
function mapBackendStatusToOrderStatus(status?: string): OrderStatus {
  if (!status) return "Pending";
  const norm = status.toLowerCase();
  if (norm === "pending") return "Pending";
  if (norm === "confirmed") return "Confirmed";
  if (norm === "processing") return "Processing";
  if (norm === "shipped") return "Shipped";
  if (norm === "delivered") return "Delivered";
  if (norm === "cancelled" || norm === "canceled") return "Cancelled";
  return "Pending";
}

/**
 * Maps lightweight OrderSummaryDto to presentation-safe OrderActivity ViewModels.
 * Leverages safe defaults for missing UI properties.
 */
export function mapOrderSummaryDtoToActivity(
  dto: OrderSummaryDto,
): OrderActivity {
  return {
    id: dto.id,
    orderNumber: `ORD-${dto.id.substring(0, 8).toUpperCase()}`,
    status: mapBackendStatusToOrderStatus(dto.status),
    deliveryStatus: dto.status,
    finalBid: dto.totalAmount,
    date: dto.createdAt,
    auction: {
      id: "unknown",
      title: "Auction Listing",
      imageUrl: "",
    },
  };
}

/**
 * Maps WonOrderSummaryDto from `/orders/won` into the My Orders row model.
 */
export function mapWonOrderSummaryDtoToActivity(
  dto: WonOrderSummaryDto,
  auction?: {
    id?: string;
    title?: string;
    imageUrl?: string;
  },
): OrderActivity {
  return {
    id: dto.orderId,
    orderNumber: `ORD-${dto.orderId.substring(0, 8).toUpperCase()}`,
    status: mapBackendStatusToOrderStatus(dto.status),
    deliveryStatus: dto.status,
    finalBid: dto.finalBidAmount,
    date: dto.orderDate,
    auction: {
      id: auction?.id ?? dto.orderId,
      title: auction?.title ?? dto.itemTitle,
      imageUrl: auction?.imageUrl ?? "",
    },
    sellerId: dto.sellerId,
    sellerName: dto.sellerName,
  };
}

/**
 * Combines OrderDetailsDto and optional AuctionSummary detail info to build OrderActivity.
 */
export function mapOrderDetailsDtoToActivity(
  dto: OrderDetailsDto,
  auction?: AuctionSummary,
): OrderActivity {
  return {
    id: dto.id,
    orderNumber: `ORD-${dto.id.substring(0, 8).toUpperCase()}`,
    status: mapBackendStatusToOrderStatus(dto.status),
    deliveryStatus: dto.status,
    finalBid: dto.totalAmount,
    date: auction?.timing.endDate
      ? new Date(auction.timing.endDate).toISOString()
      : new Date().toISOString(),
    auction: {
      id: dto.auctionId,
      title: auction?.title ?? "Auction Listing",
      imageUrl: auction?.imageUrl ?? "",
    },
    sellerId: auction?.seller?.id,
    sellerName: auction?.seller?.fullName,
  };
}

/**
 * Maps frontend CheckoutAddress to backend AddressDto.
 */
export function mapCheckoutAddressToAddressDto(
  address: CheckoutAddress,
): AddressDto {
  return {
    city: address.city || "Amman",
    street: address.streetAddress || "Main Street",
    building: address.building || "1",
    landmark: address.label || "",
  };
}
