"use client";

import { useState } from "react";
import { Shield, Check, AlertTriangle } from "lucide-react";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { CreditCardForm } from "./CreditCardForm";
import { type CreditCardFormValues } from "../validations/creditCard.schema";
import { type PayoutDetails } from "../types";
import { useAddPaymentMethod } from "../api/payment.mutations";
import { useAuthStore } from "@/stores/auth.store";

export interface PaymentMethodDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  onSavePayout?: (details: PayoutDetails) => void;
  onSaveCard?: (card: {
    id: string;
    cardType: "VISA" | "MASTERCARD" | "AMEX";
    lastFourDigits: string;
    expiryDate: string;
    cardholderName: string;
    isDefault: boolean;
  }) => void;
  mode: "payment" | "payout";
  amount?: number;
  deliveryAddress?: {
    fullName: string;
    streetAddress: string;
    building: string;
    city: string;
  } | null;
}

export function PaymentMethodDrawer({
  isOpen,
  onClose,
  onSavePayout,
  onSaveCard,
  mode,
  amount = 0,
  deliveryAddress,
}: PaymentMethodDrawerProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [billingSameAsDelivery, setBillingSameAsDelivery] = useState(true);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const { user } = useAuthStore();
  const addPaymentMethodMutation = useAddPaymentMethod();

  const getCardBrand = (num: string): "VISA" | "MASTERCARD" | "AMEX" => {
    const cleanNum = num.replace(/\s/g, "");
    if (cleanNum.startsWith("4")) return "VISA";
    if (/^5[1-5]/.test(cleanNum)) return "MASTERCARD";
    if (/^3[47]/.test(cleanNum)) return "AMEX";
    return "VISA";
  };

  const getCardBrandEnum = (brand: string): number => {
    switch (brand) {
      case "VISA":
        return 1;
      case "MASTERCARD":
        return 2;
      case "AMEX":
        return 3;
      default:
        return 0; // Unknown
    }
  };

  const handleFormSave = async (data: CreditCardFormValues) => {
    setIsSubmitting(true);
    setSubmitError(null);
    try {
      const cleanNum = data.cardNumber.replace(/\s/g, "");
      const lastFour = cleanNum.slice(-4);
      const cardBrand = getCardBrand(data.cardNumber);
      const cardBrandEnum = getCardBrandEnum(cardBrand);

      const expiryParts = data.expiryDate.split("/");
      const expiryMonth = parseInt(expiryParts[0], 10);
      const expiryYear = 2000 + parseInt(expiryParts[1], 10);

      // Perform real backend HTTP call hitting "/api/v1/users/me/payment-methods"
      const response = await addPaymentMethodMutation.mutateAsync({
        last4Digits: lastFour,
        expiryMonth,
        expiryYear,
        cardholderName: user?.fullName || "Cardholder Name",
        brand: cardBrandEnum,
        gatewayToken: `pm_mock_${Math.random().toString(36).substring(2, 10)}`,
        isDefault: true,
      });

      if (mode === "payout" && onSavePayout) {
        onSavePayout({
          type: "card",
          cardNumber: data.cardNumber,
          cardType: cardBrand,
          expiryDate: data.expiryDate,
          cvv: data.cvv,
          cardholderName: response.cardholderName,
        });
      } else if (mode === "payment" && onSaveCard) {
        onSaveCard({
          id: response.id,
          cardType: cardBrand,
          lastFourDigits: response.last4Digits,
          expiryDate: data.expiryDate,
          cardholderName: response.cardholderName,
          isDefault: response.isDefault,
        });
      }
      onClose();
    } catch (e: any) {
      const errMsg = e.message || "An unexpected error occurred while saving your payment method.";
      setSubmitError(errMsg);
      console.error("Failed to add payment method to user profile:", e.message || e);
      if (e.errors) {
        console.error("Validation errors details:", JSON.stringify(e.errors, null, 2));
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const isPayoutMode = mode === "payout";

  return (
    <Sheet open={isOpen} onOpenChange={(open) => !open && onClose()}>
      <SheetContent
        side="right"
        className="w-full sm:max-w-[500px] bg-card border-l border-border p-6 flex flex-col justify-start overflow-y-auto z-[100]"
      >
        <div className="space-y-6 text-left">
          {/* Header */}
          <SheetHeader className="border-b border-border/40 pb-4">
            <div className="flex items-center gap-2.5">
              <div className="h-8 w-8 rounded-lg bg-primary/10 border border-primary/20 flex items-center justify-center text-primary shrink-0">
                <Shield className="h-4.5 w-4.5" />
              </div>
              <div>
                <SheetTitle className="text-lg font-bold text-foreground">
                  {isPayoutMode ? "Add Payout Method (Secure)" : "Secure Checkout Payment"}
                </SheetTitle>
                <div className="flex items-center gap-1.5 text-indigo-500 font-bold text-[11px] mt-0.5">
                  <span className="text-muted-foreground font-semibold">Powered by</span>
                  <span className="tracking-tight text-xs font-extrabold uppercase text-[#635BFF]">Stripe</span>
                </div>
              </div>
            </div>
          </SheetHeader>

          {/* Secure Info Banner */}
          <div className="flex items-start gap-3 bg-[#EAF1FC] dark:bg-blue-950/20 border border-blue-100 dark:border-blue-900/30 rounded-xl p-4 text-xs md:text-sm text-[#1A73E8] dark:text-blue-300">
            <Shield className="h-5 w-5 text-[#1A73E8] shrink-0 mt-0.5" />
            <p className="leading-relaxed font-semibold">
              {isPayoutMode
                ? "Your payout information is handled securely by Stripe. MazadZone does not store your full card details."
                : "This checkout setup is handled by a third-party payment provider. MazadZone does not store your credit card details."}
            </p>
          </div>

          {/* Billing Address Selection (only for payment mode) */}
          {!isPayoutMode && deliveryAddress && (
            <div className="flex items-start gap-2 bg-muted/20 border border-border/50 rounded-xl p-4">
              <Checkbox
                id="billingSame"
                checked={billingSameAsDelivery}
                onCheckedChange={(checked) => setBillingSameAsDelivery(checked === true)}
                className="mt-0.5 data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground"
              />
              <div className="space-y-1">
                <Label
                  htmlFor="billingSame"
                  className="text-sm font-bold text-foreground cursor-pointer select-none"
                >
                  Billing address same as shipping
                </Label>
                {billingSameAsDelivery ? (
                  <p className="text-xs text-muted-foreground leading-normal">
                    Using: {deliveryAddress.building}, {deliveryAddress.streetAddress}, {deliveryAddress.city}
                  </p>
                ) : (
                  <p className="text-xs text-muted-foreground leading-normal">
                    You will be prompted to enter billing address details at confirmation.
                  </p>
                )}
              </div>
            </div>
          )}

          {/* Submit Error Alert */}
          {submitError && (
            <div className="flex items-start gap-3 bg-destructive/10 border border-destructive/20 rounded-xl p-4 text-xs md:text-sm text-destructive">
              <AlertTriangle className="h-5 w-5 text-destructive shrink-0 mt-0.5" />
              <div className="space-y-1">
                <p className="font-bold">Error Registering Card</p>
                <p className="leading-relaxed font-medium">{submitError}</p>
              </div>
            </div>
          )}

          {/* Credit Card Form Component */}
          <div className="pt-2">
            <CreditCardForm
              onSave={handleFormSave}
              onCancel={onClose}
              isSubmitting={isSubmitting}
              mode={mode}
              authorizationAmount={amount}
              submitButtonText={isPayoutMode ? "Save Payout Method" : "Save & Authorize Payment"}
              infoBannerText={
                isPayoutMode
                  ? undefined
                  : "Your card will be saved securely. The remaining 90% of your bid will be authorized and charged upon confirmation."
              }
            />
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
}
