"use client";

import { Minus, Plus, MapPin, CreditCard, Home, AlertCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { formatCurrency } from "@/utils/currency.utils";
import type { DeliveryAddress, SavedPaymentMethod } from "../../types/place-bid.types";

export interface PlaceBidStepProps {
  auctionTitle: string;
  currentBid: number;
  minIncrement: number;
  bidAmount: number;
  onBidAmountChange: (amount: number) => void;
  selectedAddress: DeliveryAddress | null;
  selectedPayment: SavedPaymentMethod | null;
  onChangeAddress: () => void;
  onAddPayment: () => void;
  onContinue: () => void;
  onCancel: () => void;
}

export function PlaceBidStep({
  auctionTitle,
  currentBid,
  minIncrement,
  bidAmount,
  onBidAmountChange,
  selectedAddress,
  selectedPayment,
  onChangeAddress,
  onAddPayment,
  onContinue,
  onCancel,
}: PlaceBidStepProps) {
  const minBid = currentBid + minIncrement;

  const handleIncrement = () => {
    onBidAmountChange(bidAmount + minIncrement);
  };

  const handleDecrement = () => {
    if (bidAmount - minIncrement >= minBid) {
      onBidAmountChange(bidAmount - minIncrement);
    }
  };

  const isContinueDisabled = !selectedAddress || !selectedPayment || bidAmount < minBid;

  return (
    <div className="space-y-6 text-left">
      {/* Header */}
      <div>
        <h3 className="text-2xl font-bold text-foreground">Place Bid</h3>
        <p className="text-sm text-muted-foreground mt-1 truncate">{auctionTitle}</p>
      </div>

      {/* Pricing Stats Grid */}
      <div className="grid grid-cols-2 gap-4">
        <div className="bg-muted/15 border border-border/20 rounded-xl p-4">
          <p className="text-xs font-semibold text-muted-foreground">
            Current Bid
          </p>
          <p className="text-2xl font-bold text-foreground mt-1">
            {formatCurrency(currentBid)}
          </p>
        </div>
        <div className="bg-muted/15 border border-border/20 rounded-xl p-4">
          <p className="text-xs font-semibold text-muted-foreground">
            Min. Increment
          </p>
          <p className="text-2xl font-bold text-foreground mt-1">
            {formatCurrency(minIncrement)}
          </p>
        </div>
      </div>

      {/* Bid Selector Box */}
      <div className="space-y-2">
        <label className="text-sm font-bold text-foreground block">Your Bid</label>
        <div className="flex items-center gap-3 w-full">
          <button
            type="button"
            onClick={handleDecrement}
            disabled={bidAmount <= minBid}
            className="h-12 w-12 rounded-xl border border-border flex items-center justify-center bg-muted/10 hover:bg-muted active:scale-95 disabled:opacity-40 disabled:pointer-events-none transition-all cursor-pointer text-foreground font-bold"
            aria-label="Decrease bid"
          >
            <Minus className="h-5 w-5 stroke-[2.5]" />
          </button>
          
          <div className="flex-1 h-12 bg-muted/5 border border-border rounded-xl flex items-center justify-center font-bold text-2xl text-foreground select-none">
            {formatCurrency(bidAmount)}
          </div>

          <button
            type="button"
            onClick={handleIncrement}
            className="h-12 w-12 rounded-xl border border-border flex items-center justify-center bg-muted/10 hover:bg-muted active:scale-95 transition-all cursor-pointer text-primary hover:text-primary/80 font-bold"
            aria-label="Increase bid"
          >
            <Plus className="h-5 w-5 stroke-[2.5]" />
          </button>
        </div>
        {bidAmount < minBid && (
          <p className="text-xs text-red-500 font-semibold flex items-center justify-center gap-1 mt-1">
            <AlertCircle className="h-3.5 w-3.5 shrink-0" />
            Minimum bid required is {formatCurrency(minBid)}
          </p>
        )}
      </div>

      {/* Delivery Address Section */}
      <div className="space-y-2">
        <label className="text-sm font-bold text-foreground block">
          Delivery Address
        </label>
        {selectedAddress ? (
          <div className="flex items-start justify-between p-4 bg-card border border-border rounded-xl">
            <div className="flex items-start gap-3">
              <Home className="h-5 w-5 text-primary shrink-0 mt-0.5" />
              <div className="space-y-0.5 text-xs md:text-sm text-left">
                <p className="font-bold text-foreground">{selectedAddress.label}</p>
                <p className="text-muted-foreground">{selectedAddress.city}</p>
                <p className="text-muted-foreground">
                  {selectedAddress.streetAddress}, {selectedAddress.building}
                </p>
                <p className="text-muted-foreground">Phone: {selectedAddress.phoneNumber}</p>
              </div>
            </div>
            <button
              type="button"
              onClick={onChangeAddress}
              className="text-sm font-bold text-primary hover:underline cursor-pointer transition-colors"
            >
              Change
            </button>
          </div>
        ) : (
          <div className="flex items-center justify-between p-4 border border-dashed border-border rounded-xl bg-card">
            <div className="flex items-center gap-3 text-muted-foreground text-sm">
              <MapPin className="h-5 w-5 shrink-0" />
              <span>No delivery address selected</span>
            </div>
            <button
              type="button"
              onClick={onChangeAddress}
              className="text-sm font-bold text-primary hover:underline cursor-pointer transition-colors"
            >
              Add Address
            </button>
          </div>
        )}
      </div>

      {/* Payment Method Section */}
      <div className="space-y-2">
        <label className="text-sm font-bold text-foreground block">
          Payment Method
        </label>
        {selectedPayment ? (
          <div className="flex items-start justify-between p-4 bg-card border border-border rounded-xl">
            <div className="flex items-start gap-3">
              <CreditCard className="h-5 w-5 text-primary shrink-0 mt-0.5" />
              <div className="space-y-0.5 text-xs md:text-sm text-left">
                <div className="flex items-center gap-2">
                  <p className="font-bold text-foreground">
                    {selectedPayment.cardType} •••• {selectedPayment.lastFourDigits}
                  </p>
                  <span className="text-[10px] font-bold uppercase tracking-wider bg-primary/10 text-primary px-1.5 py-0.5 rounded border border-primary/20">
                    Active
                  </span>
                </div>
                <p className="text-muted-foreground text-xs">{selectedPayment.cardholderName}</p>
              </div>
            </div>
            <button
              type="button"
              onClick={onAddPayment}
              className="text-sm font-bold text-primary hover:underline cursor-pointer transition-colors"
            >
              Change
            </button>
          </div>
        ) : (
          <div className="flex items-center justify-between p-4 border border-dashed border-border rounded-xl bg-card">
            <div className="flex items-start gap-3 text-left">
              <CreditCard className="h-5 w-5 text-muted-foreground shrink-0 mt-0.5" />
              <div className="space-y-0.5">
                <p className="font-bold text-foreground text-sm">No payment method added</p>
                <p className="text-xs text-muted-foreground">Add a payment method to secure your bid.</p>
              </div>
            </div>
            <button
              type="button"
              onClick={onAddPayment}
              className="text-sm font-bold text-primary hover:underline cursor-pointer transition-colors shrink-0"
            >
              Add payment method
            </button>
          </div>
        )}
      </div>

      {/* Actions */}
      <div className="pt-4 space-y-3">
        <Button
          type="button"
          onClick={onContinue}
          disabled={isContinueDisabled}
          className="w-full h-14 rounded-xl text-base font-extrabold"
        >
          Continue
        </Button>
        <button
          type="button"
          onClick={onCancel}
          className="w-full text-center text-sm font-semibold text-muted-foreground hover:text-foreground cursor-pointer transition-colors"
        >
          Cancel
        </button>
      </div>
    </div>
  );
}
