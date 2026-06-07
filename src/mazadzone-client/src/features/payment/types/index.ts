export type PaymentMode = "payout" | "payment";
export type PaymentCardBrand =
  | "VISA"
  | "MASTERCARD"
  | "AMEX"
  | "MADA"
  | "UNKNOWN";

export interface SavedPaymentMethod {
  id: string;
  cardType: PaymentCardBrand;
  lastFourDigits: string;
  expiryDate: string;
  cardholderName: string;
  isDefault: boolean;
}

export interface PayoutDetails {
  type: "card";
  paymentMethodId?: string;
  cardNumber: string;
  lastFourDigits?: string;
  cardType: PaymentCardBrand;
  expiryDate: string;
  cvv: string;
  cardholderName?: string;
}
