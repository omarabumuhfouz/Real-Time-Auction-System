"use client";

import { useState } from "react";
import { Shield, ShieldAlert, Check } from "lucide-react";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { CreditCardForm, type CreditCardFormValues } from "@/features/payment";
import { formatCurrency } from "@/utils/currency.utils";
import type { DeliveryAddress, SavedPaymentMethod, CardBrand } from "../../types/place-bid.types";

export interface PaymentProviderSheetProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (paymentMethod: SavedPaymentMethod) => void;
  bidAmount: number;
  deliveryAddress: DeliveryAddress | null;
}

export function PaymentProviderSheet({
  isOpen,
  onClose,
  onSave,
  bidAmount,
  deliveryAddress,
}: PaymentProviderSheetProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [billingSameAsDelivery, setBillingSameAsDelivery] = useState(true);

  const getCardBrand = (num: string): CardBrand => {
    const cleanNum = num.replace(/\s/g, "");
    if (cleanNum.startsWith("4")) return "VISA";
    if (/^5[1-5]/.test(cleanNum)) return "MASTERCARD";
    if (/^3[47]/.test(cleanNum)) return "AMEX";
    return "VISA";
  };

  const handleFormSave = async (data: CreditCardFormValues) => {
    setIsSubmitting(true);
    try {
      // Simulate Stripe card tokenization and verification (no actual fund holding happens here)
      await new Promise((resolve) => setTimeout(resolve, 1500));
      
      const cleanNum = data.cardNumber.replace(/\s/g, "");
      const lastFour = cleanNum.slice(-4);
      
      onSave({
        id: `pm-${Date.now()}`,
        cardType: getCardBrand(data.cardNumber),
        lastFourDigits: lastFour,
        expiryDate: data.expiryDate,
        cardholderName: data.cardholderName,
        isDefault: true,
      });
      onClose();
    } catch (e) {
      console.error("Card authorization failed:", e);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={(open) => !open && onClose()}>
      <SheetContent
        side="right"
        className="w-full sm:max-w-[500px] bg-card border-l border-border p-6 flex flex-col justify-start overflow-y-auto z-100"
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
                  Secure Payment Provider
                </SheetTitle>
                <div className="flex items-center gap-1.5 text-indigo-500 font-bold text-[11px] mt-0.5">
                  <span className="text-muted-foreground font-semibold">Powered by</span>
                  <span className="tracking-tight text-xs font-extrabold uppercase">Stripe</span>
                </div>
              </div>
            </div>
          </SheetHeader>

          {/* Secure Info Banner */}
          <div className="flex items-start gap-3 bg-[#EAF1FC] dark:bg-blue-950/20 border border-blue-100 dark:border-blue-900/30 rounded-xl p-4 text-xs md:text-sm text-[#1A73E8] dark:text-blue-300">
            <Shield className="h-5 w-5 text-[#1A73E8] shrink-0 mt-0.5" />
            <p className="leading-relaxed font-medium">
              This payment setup is handled by a third-party provider. MazadZone does not store your card details.
            </p>
          </div>

          {/* Billing Address Selection */}
          {deliveryAddress && (
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
                  Billing address same as delivery
                </Label>
                {billingSameAsDelivery ? (
                  <p className="text-xs text-muted-foreground leading-normal">
                    Using: {deliveryAddress.building}, {deliveryAddress.streetAddress}, {deliveryAddress.city}
                  </p>
                ) : (
                  <p className="text-xs text-muted-foreground leading-normal">
                    You will be prompted to enter billing address details at checkout.
                  </p>
                )}
              </div>
            </div>
          )}

          {/* Reusable Credit Card Form */}
          <div className="pt-2">
            <CreditCardForm
              onSave={handleFormSave}
              onCancel={onClose}
              isSubmitting={isSubmitting}
              defaultCardholderName={deliveryAddress?.fullName || "Omar Ahmad"}
              mode="payment"
              authorizationAmount={bidAmount * 0.1}
              submitButtonText="Save & Authorize Deposit"
              infoBannerText="Your card will be saved securely. No funds will be held until you confirm your bid in the next step."
            />
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
}
