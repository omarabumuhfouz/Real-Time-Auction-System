import { api } from "@/lib/api/client";
import type { AddPaymentMethodRequest, AddPaymentMethodResponse } from "./payment.contracts";

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
