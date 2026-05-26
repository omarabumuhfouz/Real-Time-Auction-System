"use client";

import { useState, useEffect } from "react";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { VisuallyHidden } from "radix-ui";
import { useGetAddresses, useGetProfile } from "@/features/profile";
import { useGetSavedPaymentMethods } from "@/features/bidding";
import { cn } from "@/lib/utils";
import { useCompleteOrderPayment } from "../../api";
import type { CheckoutState, CheckoutAddress, CheckoutPaymentMethod } from "../../types/checkout.types";

import { AddressSelectStep } from "@/features/profile";
import { PaymentMethodDrawer } from "@/features/payment";

// Step components
import { OrderDetailsStep } from "./OrderDetailsStep";
import { ReviewPaymentStep } from "./ReviewPaymentStep";
import { PaymentSuccessStep } from "./PaymentSuccessStep";

export interface CompletePaymentModalProps {
  orderId: string;
  orderNumber: string;
  finalBid: number;
  title: string;
  imageUrl: string;
  isOpen: boolean;
  onClose: () => void;
}

export function CompletePaymentModal({
  orderId,
  orderNumber,
  finalBid,
  title,
  imageUrl,
  isOpen,
  onClose,
}: CompletePaymentModalProps) {
  const { data: profileAddresses = [] } = useGetAddresses();
  const { data: savedPaymentMethods = [] } = useGetSavedPaymentMethods();
  const completePaymentMutation = useCompleteOrderPayment();
  const { data: profile } = useGetProfile();

  const depositAmount = finalBid * 0.1;
  const amountDue = finalBid * 0.9;

  // Orchestrated flow state
  const [flowState, setFlowState] = useState<CheckoutState>({
    step: "details",
    orderId,
    orderNumber,
    finalBid,
    title,
    imageUrl,
    depositAmount,
    amountDue,
    selectedAddress: null,
    selectedPayment: null,
    paymentResponse: null,
  });

  const [isPaymentSheetOpen, setIsPaymentSheetOpen] = useState(false);

  // Sync defaults when modal opens
  useEffect(() => {
    if (isOpen) {
      const defaultAddr = profileAddresses.find((a) => a.isDefault) || profileAddresses[0] || null;
      const mappedAddr = defaultAddr
        ? {
            id: defaultAddr.id,
            label: defaultAddr.title,
            fullName: profile?.fullName || "",
            phoneNumber: profile?.phoneNumber || "",
            streetAddress: defaultAddr.streetAddress,
            building: defaultAddr.building,
            city: defaultAddr.city,
            isDefault: defaultAddr.isDefault,
          }
        : null;

      const defaultPayment = savedPaymentMethods.find((p) => p.isDefault) || savedPaymentMethods[0] || null;
      const mappedPayment = defaultPayment
        ? {
            id: defaultPayment.id,
            cardType: defaultPayment.cardType,
            lastFourDigits: defaultPayment.lastFourDigits || "4242",
            expiryDate: defaultPayment.expiryDate,
            cardholderName: defaultPayment.cardholderName,
            isDefault: defaultPayment.isDefault,
          }
        : null;

      setFlowState({
        step: "details",
        orderId,
        orderNumber,
        finalBid,
        title,
        imageUrl,
        depositAmount,
        amountDue,
        selectedAddress: mappedAddr,
        selectedPayment: mappedPayment,
        paymentResponse: null,
      });
    }
  }, [isOpen, orderId, orderNumber, finalBid, title, imageUrl, profileAddresses, savedPaymentMethods]);

  const handleClose = () => {
    onClose();
  };

  const handleSelectAddress = (address: CheckoutAddress) => {
    setFlowState((prev) => ({
      ...prev,
      selectedAddress: address,
      step: "details",
    }));
  };

  const handleSavePaymentMethod = (paymentMethod: CheckoutPaymentMethod) => {
    setFlowState((prev) => ({
      ...prev,
      selectedPayment: paymentMethod,
    }));
    setIsPaymentSheetOpen(false);
  };

  const handlePaymentSubmit = async () => {
    if (!flowState.selectedAddress || !flowState.selectedPayment) return;

    try {
      const response = await completePaymentMutation.mutateAsync({
        orderId: flowState.orderId,
        addressId: flowState.selectedAddress.id,
        paymentMethodId: flowState.selectedPayment.id,
      });

      setFlowState((prev) => ({
        ...prev,
        paymentResponse: {
          ...response,
          // Map to verify types align
          deliveryAddress: prev.selectedAddress!,
          paymentMethod: prev.selectedPayment!,
        },
        step: "success",
      }));
    } catch (err) {
      console.error("Failed to complete order payment:", err);
    }
  };

  const currentStep = flowState.step;

  return (
    <>
      <Dialog open={isOpen} onOpenChange={(open) => !open && handleClose()}>
        <DialogContent
          className={cn(
            "w-full bg-card border-border p-6 shadow-xl rounded-xl gap-0 z-50 focus-visible:outline-none transition-all duration-200 text-left",
            currentStep === "details" ? "max-w-lg" : "max-w-md"
          )}
          showCloseButton={currentStep !== "success"}
        >
          <VisuallyHidden.Root>
            <DialogTitle>Complete Order Payment Dialog</DialogTitle>
          </VisuallyHidden.Root>

          {/* Step 1: Details summary */}
          {currentStep === "details" && (
            <OrderDetailsStep
              orderNumber={flowState.orderNumber}
              title={flowState.title}
              imageUrl={flowState.imageUrl}
              finalBid={flowState.finalBid}
              depositAmount={flowState.depositAmount}
              amountDue={flowState.amountDue}
              selectedAddress={flowState.selectedAddress}
              selectedPayment={flowState.selectedPayment}
              onChangeAddress={() => setFlowState((prev) => ({ ...prev, step: "choose-address" }))}
              onAddPayment={() => setIsPaymentSheetOpen(true)}
              onContinue={() => setFlowState((prev) => ({ ...prev, step: "review" }))}
              onCancel={handleClose}
            />
          )}

          {/* Step 2: Choose Address list */}
          {currentStep === "choose-address" && (
            <AddressSelectStep
              selectedAddressId={flowState.selectedAddress?.id}
              onSelectAddress={handleSelectAddress}
              onCancel={() => setFlowState((prev) => ({ ...prev, step: "details" }))}
              title="Choose Shipping Address"
              subtitle="Select where you want your item to be shipped."
            />
          )}

          {/* Step 4: Review and Pay */}
          {currentStep === "review" && (
            <ReviewPaymentStep
              orderNumber={flowState.orderNumber}
              title={flowState.title}
              finalBid={flowState.finalBid}
              depositAmount={flowState.depositAmount}
              amountDue={flowState.amountDue}
              selectedAddress={flowState.selectedAddress}
              selectedPayment={flowState.selectedPayment}
              onConfirm={handlePaymentSubmit}
              onCancel={() => setFlowState((prev) => ({ ...prev, step: "details" }))}
              isSubmitting={completePaymentMutation.isPending}
            />
          )}

          {/* Step 5: Success Receipt */}
          {currentStep === "success" && (
            <PaymentSuccessStep
              paymentResponse={flowState.paymentResponse}
              onClose={handleClose}
            />
          )}
        </DialogContent>
      </Dialog>

      {/* Step 3: Card Setup side panel */}
      <PaymentMethodDrawer
        isOpen={isPaymentSheetOpen}
        onClose={() => setIsPaymentSheetOpen(false)}
        onSaveCard={handleSavePaymentMethod}
        mode="payment"
        amount={flowState.amountDue}
        deliveryAddress={flowState.selectedAddress}
      />
    </>
  );
}
