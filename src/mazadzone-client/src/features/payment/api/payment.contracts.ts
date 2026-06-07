export type PaymentCardBrandCode = 0 | 1 | 2 | 3 | 4;

export interface AddPaymentMethodRequest {
  last4Digits: string;
  expiryMonth: number;
  expiryYear: number;
  cardholderName: string;
  brand: PaymentCardBrandCode;
  gatewayToken: string;
  isDefault: boolean;
}

export interface PaymentMethodResponse {
  id: string;
  last4Digits: string;
  expiryMonth: number;
  expiryYear: number;
  cardholderName: string;
  brand: PaymentCardBrandCode;
  isDefault: boolean;
  createdOnUtc: string;
}

export type AddPaymentMethodResponse = PaymentMethodResponse;
