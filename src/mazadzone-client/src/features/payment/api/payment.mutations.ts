import { useMutation } from "@tanstack/react-query";
import { addPaymentMethod } from "./payment.api";
import type { AddPaymentMethodRequest, AddPaymentMethodResponse } from "./payment.contracts";

/**
 * Mutation hook to register a secure payment/payout method on the backend user profile.
 */
export function useAddPaymentMethod() {
  return useMutation<AddPaymentMethodResponse, Error, AddPaymentMethodRequest>({
    mutationFn: addPaymentMethod,
  });
}
