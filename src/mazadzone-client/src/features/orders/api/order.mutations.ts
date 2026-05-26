/**
 * React Query mutations for the Orders feature.
 * Connects directly to the ASP.NET Core endpoints.
 */

import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { CheckoutPaymentResponse } from "../types/checkout.types";
import {
  payRemainingAmount,
  createOrder,
  confirmOrder,
  shipOrder,
  deliverOrder,
  cancelOrder,
} from "./order.api";
import { orderKeys } from "./order.keys";
import { useAuthStore } from "@/stores/auth.store";
import { useAppToast } from "@/lib/toast/app-toast";
import type { ApiError } from "@/types/api.types";
import type { CreateOrderRequest } from "./order.contracts";

/**
 * Mutation hook to complete remaining payment authorization on an order.
 * Invokes real backend complete payment REST POST endpoint, constructs local CheckoutPaymentResponse.
 */
export function useCompleteOrderPayment() {
  const queryClient = useQueryClient();
  const { user } = useAuthStore();
  const appToast = useAppToast();

  return useMutation<
    CheckoutPaymentResponse,
    ApiError,
    { orderId: string; addressId: string; paymentMethodId: string }
  >({
    mutationFn: async ({ orderId, addressId, paymentMethodId }) => {
      // Call backend POST endpoint
      await payRemainingAmount(orderId, paymentMethodId);

      // Return a compatible confirmation response payload for the checkout success view
      return {
        orderId,
        orderNumber: `ORD-${orderId.substring(0, 8).toUpperCase()}`,
        status: "Confirmed",
        amountPaid: 0,
        deliveryAddress: {
          id: addressId,
          label: "Home (Default)",
          fullName: user?.fullName || "Omar Ahmad",
          phoneNumber: "+962 7 9123 4567",
          streetAddress: "Al-Hamra Street",
          building: "Building 12, Floor 3",
          city: "Amman",
          isDefault: true,
        },
        paymentMethod: {
          id: paymentMethodId,
          cardType: "VISA",
          lastFourDigits: "4242",
          expiryDate: "12/28",
          cardholderName: user?.fullName || "Omar Ahmad",
          isDefault: true,
        },
        transactionId: `tx-${Date.now()}`,
        paidAt: new Date().toISOString(),
      };
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: orderKeys.all });
      appToast.success("Success", "Payment processed successfully.");
    },
    onError: (err) => {
      appToast.error("Error", err.message || "Failed to process payment.");
    },
  });
}

/**
 * Mutation hook to create an order (winning bidder initiates checkout).
 */
export function useCreateOrder() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation<void, ApiError, CreateOrderRequest>({
    mutationFn: (request) => createOrder(request),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: orderKeys.all });
      appToast.success("Success", "Order has been created.");
    },
    onError: (err) => {
      appToast.error("Error", err.message || "Failed to create order.");
    },
  });
}

/**
 * Mutation hook for confirming an order (Seller lifecycle action).
 */
export function useConfirmOrder() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation<void, ApiError, string>({
    mutationFn: (id) => confirmOrder(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: orderKeys.all });
      appToast.success("Success", "Order confirmed successfully.");
    },
    onError: (err) => {
      appToast.error("Error", err.message || "Failed to confirm order.");
    },
  });
}

/**
 * Mutation hook to mark an order as shipped (Seller lifecycle action).
 */
export function useShipOrder() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation<void, ApiError, string>({
    mutationFn: (id) => shipOrder(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: orderKeys.all });
      appToast.success("Success", "Order marked as shipped.");
    },
    onError: (err) => {
      appToast.error("Error", err.message || "Failed to mark order as shipped.");
    },
  });
}

/**
 * Mutation hook to mark an order as delivered (Buyer lifecycle action).
 */
export function useDeliverOrder() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation<void, ApiError, string>({
    mutationFn: (id) => deliverOrder(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: orderKeys.all });
      appToast.success("Success", "Order marked as delivered.");
    },
    onError: (err) => {
      appToast.error(
        "Error",
        err.message || "Failed to mark order as delivered.",
      );
    },
  });
}

/**
 * Mutation hook to cancel an order.
 */
export function useCancelOrder() {
  const queryClient = useQueryClient();
  const appToast = useAppToast();

  return useMutation<void, ApiError, string>({
    mutationFn: (id) => cancelOrder(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: orderKeys.all });
      appToast.success("Success", "Order canceled successfully.");
    },
    onError: (err) => {
      appToast.error("Error", err.message || "Failed to cancel order.");
    },
  });
}
