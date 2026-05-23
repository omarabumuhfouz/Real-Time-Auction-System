"use client";

import { Check } from "lucide-react";
import { Button } from "@/components/ui/button";
import { formatCurrency } from "@/utils/currency.utils";
import { format } from "date-fns";
import type { CheckoutPaymentResponse } from "../../types/checkout.types";

export interface PaymentSuccessStepProps {
  paymentResponse: CheckoutPaymentResponse | null;
  onClose: () => void;
}

export function PaymentSuccessStep({
  paymentResponse,
  onClose,
}: PaymentSuccessStepProps) {
  if (!paymentResponse) return null;

  return (
    <div className="space-y-6 text-center py-4">
      {/* Animated Success Checkmark */}
      <div className="flex items-center justify-center">
        <div className="h-16 w-16 rounded-full bg-emerald-100 dark:bg-emerald-950/20 text-emerald-600 dark:text-emerald-500 border border-emerald-200 dark:border-emerald-900/30 flex items-center justify-center animate-[scale-in_0.4s_ease-out_forwards]">
          <Check className="h-9 w-9 stroke-[3] animate-[draw-check_0.3s_ease-out_0.2s_both]" />
        </div>
      </div>

      {/* Heading */}
      <div className="space-y-2">
        <h3 className="text-2xl font-black text-foreground">Payment Completed!</h3>
        <p className="text-sm text-muted-foreground max-w-[320px] mx-auto leading-relaxed">
          The remaining 90% payment of{" "}
          <span className="font-bold text-foreground">
            {formatCurrency(paymentResponse.amountPaid)}
          </span>{" "}
          has been processed. Order <span className="font-semibold text-foreground">{paymentResponse.orderNumber}</span> is now confirmed.
        </p>
      </div>

      {/* Receipt details */}
      <div className="border border-border/80 rounded-xl overflow-hidden bg-muted/5 text-left text-xs">
        <div className="p-4 border-b border-border/60 bg-muted/10">
          <h4 className="font-bold text-foreground">Transaction Receipt</h4>
        </div>
        <div className="divide-y divide-border/40 text-muted-foreground p-4 space-y-3">
          <div className="flex justify-between items-center">
            <span>Transaction ID</span>
            <span className="font-medium text-foreground font-mono">{paymentResponse.transactionId}</span>
          </div>
          <div className="flex justify-between items-center pt-2">
            <span>Date</span>
            <span className="font-medium text-foreground">
              {format(new Date(paymentResponse.paidAt), "MMMM dd, yyyy 'at' hh:mm a")}
            </span>
          </div>
          <div className="flex justify-between items-center pt-2">
            <span>Amount Charged (90%)</span>
            <span className="font-bold text-foreground">
              {formatCurrency(paymentResponse.amountPaid)}
            </span>
          </div>
          <div className="flex justify-between items-start pt-2">
            <span>Shipped To</span>
            <span className="font-medium text-foreground text-right max-w-[200px] truncate leading-normal">
              {paymentResponse.deliveryAddress.fullName}
              <br />
              {paymentResponse.deliveryAddress.streetAddress}, {paymentResponse.deliveryAddress.city}
            </span>
          </div>
        </div>
      </div>

      {/* Action Button */}
      <div className="pt-4">
        <Button
          type="button"
          onClick={onClose}
          className="w-full h-12 rounded-xl text-base font-extrabold"
        >
          Back to Won Orders
        </Button>
      </div>
    </div>
  );
}
