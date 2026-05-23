/**
 * Type definitions for the Place Bid multi-step flow.
 *
 * Steps: place-bid → choose-address → payment → review → success
 */

// ---------------------------------------------------------------------------
// Step tracking
// ---------------------------------------------------------------------------

export const PLACE_BID_STEPS = {
  PLACE_BID: "place-bid",
  CHOOSE_ADDRESS: "choose-address",
  PAYMENT: "payment",
  REVIEW: "review",
  SUCCESS: "success",
} as const;

export type PlaceBidStep =
  (typeof PLACE_BID_STEPS)[keyof typeof PLACE_BID_STEPS];

// ---------------------------------------------------------------------------
// Delivery address (mirrors profile Address for bid flow context)
// ---------------------------------------------------------------------------

export interface DeliveryAddress {
  id: string;
  label: string;
  fullName: string;
  phoneNumber: string;
  streetAddress: string;
  building: string;
  city: string;
  isDefault: boolean;
}

// ---------------------------------------------------------------------------
// Saved payment method
// ---------------------------------------------------------------------------

export type CardBrand = "VISA" | "MASTERCARD" | "AMEX";

export interface SavedPaymentMethod {
  id: string;
  cardType: CardBrand;
  lastFourDigits: string;
  expiryDate: string;
  cardholderName: string;
  isDefault: boolean;
}

// ---------------------------------------------------------------------------
// API request / response
// ---------------------------------------------------------------------------

export interface PlaceBidRequest {
  auctionId: string;
  bidAmount: number;
  addressId: string;
  paymentMethodId?: string;
  paymentDetails?: {
    cardNumber: string;
    expiryDate: string;
    cvv: string;
    cardholderName: string;
  };
}

export interface PlaceBidResponse {
  bidId: string;
  auctionId: string;
  auctionTitle: string;
  bidAmount: number;
  authorizationHold: number;
  deliveryAddress: DeliveryAddress;
  paymentMethod: SavedPaymentMethod;
  placedAt: string;
}

// ---------------------------------------------------------------------------
// Aggregate state for the multi-step flow
// ---------------------------------------------------------------------------

export interface PlaceBidFlowState {
  step: PlaceBidStep;
  auctionId: string;
  auctionTitle: string;
  currentBid: number;
  minIncrement: number;
  bidAmount: number;
  selectedAddress: DeliveryAddress | null;
  selectedPayment: SavedPaymentMethod | null;
  bidResponse: PlaceBidResponse | null;
}

// ---------------------------------------------------------------------------
// Props shared across step components
// ---------------------------------------------------------------------------

export interface PlaceBidModalProps {
  auctionId: string;
  auctionTitle: string;
  currentBid: number;
  minIncrement: number;
  isOpen: boolean;
  onClose: () => void;
}
