import { api } from "@/lib/api/client";
import type {
  AddPaymentMethodRequest,
  AddPaymentMethodResponse,
  PaymentMethodResponse,
} from "./payment.contracts";

/**
 * Saves a new payment method for the authenticated user in the ASP.NET Core backend.
 */
export async function addPaymentMethod(
  request: AddPaymentMethodRequest
): Promise<AddPaymentMethodResponse> {
  const response = await api.post<AddPaymentMethodResponse>(
    "/api/v1/users/me/payment-methods",
    request
  );
  return response.data;
}

/**
 * Retrieves the authenticated user's saved payment methods from the backend.
 */
export async function fetchSavedPaymentMethods(): Promise<PaymentMethodResponse[]> {
  const response = await api.get<PaymentMethodResponse[]>(
    "/api/v1/users/me/payment-methods"
  );
  return response.data;
}
