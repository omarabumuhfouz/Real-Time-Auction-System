"use client";

import { useState, useEffect } from "react";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { VisuallyHidden } from "radix-ui";
import { useGetAddresses } from "@/features/profile";
import { cn } from "@/lib/utils";
import { usePlaceBid, useGetSavedPaymentMethods } from "../../api/bidding.queries";
import type { PlaceBidFlowState, PlaceBidModalProps, DeliveryAddress, SavedPaymentMethod } from "../../types/place-bid.types";

// Step components
import { PlaceBidStep } from "./PlaceBidStep";
import { ChooseAddressStep } from "./ChooseAddressStep";
import { PaymentProviderSheet } from "./PaymentProviderSheet";
import { ReviewConfirmStep } from "./ReviewConfirmStep";
import { BidSuccessStep } from "./BidSuccessStep";

export function PlaceBidModal({
  auctionId,
  auctionTitle,
  currentBid,
  minIncrement,
  isOpen,
  onClose,
}: PlaceBidModalProps) {
  const { data: profileAddresses = [] } = useGetAddresses();
  const { data: savedPaymentMethods = [] } = useGetSavedPaymentMethods();
  const placeBidMutation = usePlaceBid();

  // State orchestrator
  const [flowState, setFlowState] = useState<PlaceBidFlowState>({
    step: "place-bid",
    auctionId,
    auctionTitle,
    currentBid,
    minIncrement,
    bidAmount: currentBid + minIncrement,
    selectedAddress: null,
    selectedPayment: null,
    bidResponse: null,
  });

  const [isPaymentSheetOpen, setIsPaymentSheetOpen] = useState(false);

  // Reset or initialize state when modal opens
  useEffect(() => {
    if (isOpen) {
      // Fetch default address
      const defaultAddr = profileAddresses.find((a) => a.isDefault) || profileAddresses[0] || null;
      const mappedAddr = defaultAddr
        ? {
            id: defaultAddr.id,
            label: defaultAddr.isDefault ? "Home (Default)" : "Address",
            fullName: defaultAddr.fullName,
            phoneNumber: defaultAddr.phoneNumber,
            streetAddress: defaultAddr.streetAddress,
            building: defaultAddr.building,
            city: defaultAddr.city,
            isDefault: defaultAddr.isDefault,
          }
        : null;

      // Fetch default payment method
      const defaultPayment = savedPaymentMethods.find((p) => p.isDefault) || savedPaymentMethods[0] || null;

      setFlowState({
        step: "place-bid",
        auctionId,
        auctionTitle,
        currentBid,
        minIncrement,
        bidAmount: currentBid + minIncrement,
        selectedAddress: mappedAddr,
        selectedPayment: defaultPayment,
        bidResponse: null,
      });
    }
  }, [isOpen, auctionId, auctionTitle, currentBid, minIncrement, profileAddresses, savedPaymentMethods]);

  const handleClose = () => {
    // When the dialog closes, reset and close
    onClose();
  };

  const handleSelectAddress = (address: DeliveryAddress) => {
    setFlowState((prev) => ({
      ...prev,
      selectedAddress: address,
      step: "place-bid", // Go back to place bid
    }));
  };

  const handleSavePaymentMethod = (paymentMethod: SavedPaymentMethod) => {
    setFlowState((prev) => ({
      ...prev,
      selectedPayment: paymentMethod,
    }));
    setIsPaymentSheetOpen(false);
  };

  const handlePlaceBidSubmit = async () => {
    if (!flowState.selectedAddress) return;

    try {
      const response = await placeBidMutation.mutateAsync({
        auctionId: flowState.auctionId,
        bidAmount: flowState.bidAmount,
        addressId: flowState.selectedAddress.id,
        paymentMethodId: flowState.selectedPayment?.id,
      });

      setFlowState((prev) => ({
        ...prev,
        bidResponse: response,
        step: "success",
      }));
    } catch (err) {
      console.error("Failed to place bid:", err);
    }
  };

  const currentStep = flowState.step;

  return (
    <>
      <Dialog open={isOpen} onOpenChange={(open) => !open && handleClose()}>
        <DialogContent
          className={cn(
            "w-full bg-card border-border p-6 shadow-xl rounded-xl gap-0 z-50 focus-visible:outline-none transition-all duration-200",
            currentStep === "place-bid" ? "max-w-lg" : "max-w-md"
          )}
          showCloseButton={currentStep !== "success"}
        >
          <VisuallyHidden.Root>
            <DialogTitle>Place Bid Dialog</DialogTitle>
          </VisuallyHidden.Root>
          {/* Step 1: Place Bid Form */}
          {currentStep === "place-bid" && (
            <PlaceBidStep
              auctionTitle={flowState.auctionTitle}
              currentBid={flowState.currentBid}
              minIncrement={flowState.minIncrement}
              bidAmount={flowState.bidAmount}
              onBidAmountChange={(amt) => setFlowState((prev) => ({ ...prev, bidAmount: amt }))}
              selectedAddress={flowState.selectedAddress}
              selectedPayment={flowState.selectedPayment}
              onChangeAddress={() => setFlowState((prev) => ({ ...prev, step: "choose-address" }))}
              onAddPayment={() => setIsPaymentSheetOpen(true)}
              onContinue={() => setFlowState((prev) => ({ ...prev, step: "review" }))}
              onCancel={handleClose}
            />
          )}

          {/* Step 2: Choose Address List */}
          {currentStep === "choose-address" && (
            <ChooseAddressStep
              selectedAddressId={flowState.selectedAddress?.id}
              onSelectAddress={handleSelectAddress}
              onCancel={() => setFlowState((prev) => ({ ...prev, step: "place-bid" }))}
            />
          )}

          {/* Step 4: Review and Confirm Details */}
          {currentStep === "review" && (
            <ReviewConfirmStep
              auctionTitle={flowState.auctionTitle}
              bidAmount={flowState.bidAmount}
              currentBid={flowState.currentBid}
              minIncrement={flowState.minIncrement}
              selectedAddress={flowState.selectedAddress}
              selectedPayment={flowState.selectedPayment}
              onConfirm={handlePlaceBidSubmit}
              onCancel={() => setFlowState((prev) => ({ ...prev, step: "place-bid" }))}
              onChangeAddress={() => setFlowState((prev) => ({ ...prev, step: "choose-address" }))}
              onChangePayment={() => setIsPaymentSheetOpen(true)}
              isSubmitting={placeBidMutation.isPending}
            />
          )}

          {/* Step 5: Bid Success */}
          {currentStep === "success" && (
            <BidSuccessStep
              bidResponse={flowState.bidResponse}
              onViewAuction={handleClose}
              onClose={handleClose}
            />
          )}
        </DialogContent>
      </Dialog>

      {/* Step 3: Payment Sheet overlay on the right side */}
      <PaymentProviderSheet
        isOpen={isPaymentSheetOpen}
        onClose={() => setIsPaymentSheetOpen(false)}
        onSave={handleSavePaymentMethod}
        bidAmount={flowState.bidAmount}
        deliveryAddress={flowState.selectedAddress}
      />
    </>
  );
}
