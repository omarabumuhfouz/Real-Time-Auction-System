import { useMutation, useQueryClient } from "@tanstack/react-query";
import { addPaymentMethod } from "./payment.api";
import type { AddPaymentMethodRequest, AddPaymentMethodResponse } from "./payment.contracts";
import { paymentKeys } from "./payment.keys";
import { mapPaymentMethodResponseToSavedPaymentMethod } from "./payment.mappers";
import type { SavedPaymentMethod } from "../types";

/**
 * Mutation hook to register a secure payment/payout method on the backend user profile.
 */
export function useAddPaymentMethod() {
  const queryClient = useQueryClient();

  return useMutation<AddPaymentMethodResponse, Error, AddPaymentMethodRequest>({
    mutationFn: addPaymentMethod,
    onSuccess: (createdPaymentMethod) => {
      const mappedPaymentMethod =
        mapPaymentMethodResponseToSavedPaymentMethod(createdPaymentMethod);

      queryClient.setQueryData<SavedPaymentMethod[]>(
        paymentKeys.savedMethods(),
        (currentPaymentMethods = []) => {
          const nextPaymentMethods = currentPaymentMethods
            .filter((paymentMethod) => paymentMethod.id !== mappedPaymentMethod.id)
            .map((paymentMethod) => ({
              ...paymentMethod,
              isDefault: mappedPaymentMethod.isDefault ? false : paymentMethod.isDefault,
            }));

          return [mappedPaymentMethod, ...nextPaymentMethods];
        },
      );

      void queryClient.invalidateQueries({ queryKey: paymentKeys.savedMethods() });
    },
  });
}
