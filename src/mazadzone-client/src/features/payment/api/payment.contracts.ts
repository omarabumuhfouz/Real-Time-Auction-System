export interface AddPaymentMethodRequest {
  last4Digits: string;
  expiryMonth: number;
  expiryYear: number;
  cardholderName: string;
  brand: number; // CardBrand enum (1 = Visa, 2 = Mastercard, 3 = AmericanExpress, 4 = Mada, 0 = Unknown)
  gatewayToken: string;
  isDefault: boolean;
}

export interface AddPaymentMethodResponse {
  id: string;
  last4Digits: string;
  expiryMonth: number;
  expiryYear: number;
  cardholderName: string;
  brand: number;
  isDefault: boolean;
  createdOnUtc: string;
}
