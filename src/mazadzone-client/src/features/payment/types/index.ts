export type PaymentMode = "payout" | "payment";

export interface PayoutDetails {
  type: "card";
  cardNumber: string;
  cardType: string;
  expiryDate: string;
  cvv: string;
  cardholderName: string;
}
