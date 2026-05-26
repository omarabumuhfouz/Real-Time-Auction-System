import type { OrderActivity } from "./orders.types";

export type CheckoutStep = "details" | "choose-address" | "review" | "success";

export interface CheckoutAddress {
  id: string;
  label: string;
  fullName: string;
  phoneNumber: string;
  streetAddress: string;
  building: string;
  city: string;
  isDefault: boolean;
}

export interface CheckoutPaymentMethod {
  id: string;
  cardType: "VISA" | "MASTERCARD" | "AMEX";
  lastFourDigits: string;
  expiryDate: string;
  cardholderName: string;
  isDefault: boolean;
}

export interface CheckoutState {
  step: CheckoutStep;
  orderId: string;
  orderNumber: string;
  finalBid: number;
  title: string;
  imageUrl: string;
  depositAmount: number;
  amountDue: number;
  selectedAddress: CheckoutAddress | null;
  selectedPayment: CheckoutPaymentMethod | null;
  paymentResponse: CheckoutPaymentResponse | null;
}

export interface CheckoutPaymentResponse {
  orderId: string;
  orderNumber: string;
  status: "Confirmed";
  amountPaid: number;
  deliveryAddress: CheckoutAddress;
  paymentMethod: CheckoutPaymentMethod;
  transactionId: string;
  paidAt: string;
}
