import type { SavedPaymentMethod, PaymentCardBrand } from "../types";
import type { PaymentCardBrandCode, PaymentMethodResponse } from "./payment.contracts";

function mapPaymentBrand(brand: PaymentCardBrandCode): PaymentCardBrand {
  switch (brand) {
    case 1:
      return "VISA";
    case 2:
      return "MASTERCARD";
    case 3:
      return "AMEX";
    case 4:
      return "MADA";
    default:
      return "UNKNOWN";
  }
}

function formatExpiryDate(month: number, year: number): string {
  const formattedMonth = String(month).padStart(2, "0");
  const formattedYear = String(year).slice(-2);

  return `${formattedMonth}/${formattedYear}`;
}

export function mapPaymentMethodResponseToSavedPaymentMethod(
  paymentMethod: PaymentMethodResponse,
): SavedPaymentMethod {
  return {
    id: paymentMethod.id,
    cardType: mapPaymentBrand(paymentMethod.brand),
    lastFourDigits: paymentMethod.last4Digits,
    expiryDate: formatExpiryDate(
      paymentMethod.expiryMonth,
      paymentMethod.expiryYear,
    ),
    cardholderName: paymentMethod.cardholderName,
    isDefault: paymentMethod.isDefault,
  };
}
