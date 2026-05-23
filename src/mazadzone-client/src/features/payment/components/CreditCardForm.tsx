"use client";

import { useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { HelpCircle, Loader2, Lock, AlertTriangle } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { PaymentMode } from "../types";
import { formatCurrency } from "@/utils/currency.utils";

import { 
  creditCardSchema, 
  type CreditCardFormValues 
} from "../validations/creditCard.schema";

export interface CreditCardFormProps {
  onSave: (data: CreditCardFormValues) => Promise<void>;
  onCancel: () => void;
  isSubmitting: boolean;
  defaultCardholderName?: string;
  mode?: PaymentMode;
  authorizationAmount?: number;
  submitButtonText?: string;
  infoBannerText?: string;
}

export function CreditCardForm({
  onSave,
  onCancel,
  isSubmitting,
  defaultCardholderName = "Omar Ahmad",
  mode = "payout",
  authorizationAmount = 0,
  submitButtonText,
  infoBannerText
}: CreditCardFormProps) {
  
  const {
    handleSubmit,
    control,
    formState: { errors },
    reset,
  } = useForm<CreditCardFormValues>({
    resolver: zodResolver(creditCardSchema),
    defaultValues: {
      cardNumber: "",
      expiryDate: "",
      cvv: "",
      cardholderName: defaultCardholderName,
    },
  });

  // Formatting helpers
  const formatCardNumber = (val: string) => {
    const v = val.replace(/\s+/g, "").replace(/[^0-9]/gi, "");
    const matches = v.match(/\d{4,16}/g);
    const match = (matches && matches[0]) || "";
    const parts = [];
    for (let i = 0, len = match.length; i < len; i += 4) {
      parts.push(match.substring(i, i + 4));
    }
    return parts.length > 0 ? parts.join(" ") : v;
  };

  const formatExpiryDate = (val: string) => {
    const v = val.replace(/\s+/g, "").replace(/[^0-9]/gi, "");
    if (v.length >= 2) {
      return `${v.substring(0, 2)}/${v.substring(2, 4)}`;
    }
    return v;
  };

  // Card brand detection inside the Card Number input
  const getCardBrand = (cardNumberVal: string) => {
    const cleanNum = cardNumberVal.replace(/\s/g, "");
    if (cleanNum.startsWith("4")) return "VISA";
    if (/^5[1-5]/.test(cleanNum)) return "MASTERCARD";
    if (/^3[47]/.test(cleanNum)) return "AMEX";
    return "";
  };

  const onSubmit = async (data: CreditCardFormValues) => {
    await onSave(data);
    reset();
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5 text-left">
      {/* Accepted Cards */}
      <div className="flex items-center justify-between py-1 flex-wrap gap-2">
        <span className="text-xs font-bold text-muted-foreground uppercase tracking-wider">
          Accepted cards
        </span>
        
        {/* Visual inline SVG logos */}
        <div className="flex items-center gap-2">
          <div className="h-6 w-9 bg-slate-50 dark:bg-slate-900 border border-border rounded-md flex items-center justify-center p-0.5 overflow-hidden">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32" className="h-5 w-auto">
              <g className="nc-icon-wrapper">
                <rect x="2" y="7" width="28" height="18" rx="3" ry="3" fill="#fff" strokeWidth="0"></rect>
                <path d="m27,7H5c-1.657,0-3,1.343-3,3v12c0,1.657,1.343,3,3,3h22c1.657,0,3-1.343,3-3v-12c0-1.657-1.343-3-3-3Zm2,15c0,1.103-.897,2-2,2H5c-1.103,0-2-.897-2-2v-12c0-1.103.897-2,2-2h22c1.103,0,2,.897,2,2v12Z" strokeWidth="0" opacity=".15"></path>
                <path d="m27,8H5c-1.105,0-2,.895-2,2v1c0-1.105.895-2,2-2h22c1.105,0,2,.895,2,2v-1c0-1.105-.895-2-2-2Z" fill="#fff" opacity=".2" strokeWidth="0"></path>
                <path d="m13.392,12.624l-2.838,6.77h-1.851l-1.397-5.403c-.085-.332-.158-.454-.416-.595-.421-.229-1.117-.443-1.728-.576l.041-.196h2.98c.38,0,.721.253.808.69l.738,3.918,1.822-4.608h1.84Z" fill="#1434cb" strokeWidth="0"></path>
                <path d="m20.646,17.183c.008-1.787-2.47-1.886-2.453-2.684.005-.243.237-.501.743-.567.251-.032.943-.058,1.727.303l.307-1.436c-.421-.152-.964-.299-1.638-.299-1.732,0-2.95.92-2.959,2.238-.011.975.87,1.518,1.533,1.843.683.332.912.545.909.841-.005.454-.545.655-1.047.663-.881.014-1.392-.238-1.799-.428l-.318,1.484c.41.188,1.165.351,1.947.359,1.841,0,3.044-.909,3.05-2.317" fill="#1434cb" strokeWidth="0"></path>
                <path d="m25.423,12.624h-1.494c-.337,0-.62.195-.746.496l-2.628,6.274h1.839l.365-1.011h2.247l.212,1.011h1.62l-1.415-6.77Zm-2.16,4.372l.922-2.542.53,2.542h-1.452Z" fill="#1434cb" strokeWidth="0"></path>
                <path fill="#1434cb" strokeWidth="0" d="M15.894 12.624L14.446 19.394 12.695 19.394 14.143 12.624 15.894 12.624z"></path>
              </g>
            </svg>
          </div>
          <div className="h-6 w-9 bg-slate-50 dark:bg-slate-900 border border-border rounded-md flex items-center justify-center p-0.5">
            <svg viewBox="0 0 100 62" className="h-4 w-auto">
              <circle cx="34" cy="31" r="28" fill="#EB001B" />
              <circle cx="66" cy="31" r="28" fill="#F79E1B" opacity="0.85" />
            </svg>
          </div>
          <div className="h-6 w-9 bg-slate-50 dark:bg-slate-900 border border-border rounded-md flex items-center justify-center p-0.5">
            <svg viewBox="0 0 100 100" className="h-4 w-auto">
              <rect width="100" height="100" rx="8" fill="#007bc1" />
              <text x="50%" y="60%" textAnchor="middle" fill="#FFFFFF" fontSize="34" fontWeight="900" fontFamily="sans-serif">
                AMEX
              </text>
            </svg>
          </div>
        </div>
      </div>

      {/* Card Number */}
      <div className="space-y-1.5 relative">
        <Label htmlFor="cardNumber" className="text-sm font-semibold text-foreground">
          Card number
        </Label>
        <div className="relative">
          <Controller
            name="cardNumber"
            control={control}
            render={({ field }) => {
              const brand = getCardBrand(field.value);
              return (
                <>
                  <Input
                    id="cardNumber"
                    type="text"
                    placeholder="4242 4242 4242 4242"
                    maxLength={19}
                    className="h-12 rounded-xl text-base px-4 pr-16 border border-input focus-visible:ring-2 focus-visible:ring-primary w-full bg-input-background font-mono"
                    {...field}
                    onChange={(e) => {
                      const formatted = formatCardNumber(e.target.value);
                      field.onChange(formatted);
                    }}
                  />
                  {/* Live inline Card Logo inside input */}
                  {brand && (
                    <span className="absolute right-4 top-1/2 -translate-y-1/2 text-xs font-black tracking-widest text-[#1B73E8] bg-blue-50 dark:bg-slate-800 px-2 py-1 rounded border border-blue-100">
                      {brand}
                    </span>
                  )}
                </>
              );
            }}
          />
        </div>
        {errors.cardNumber && (
          <p className="text-xs text-red-500 font-semibold">{errors.cardNumber.message}</p>
        )}
      </div>

      {/* Expiry Date & CVV (Grid Layout) */}
      <div className="grid grid-cols-2 gap-4">
        {/* Expiry Date */}
        <div className="space-y-1.5">
          <Label htmlFor="expiryDate" className="text-sm font-semibold text-foreground">
            Expiry date
          </Label>
          <Controller
            name="expiryDate"
            control={control}
            render={({ field }) => (
              <Input
                id="expiryDate"
                type="text"
                placeholder="MM / YY"
                maxLength={5}
                className="h-12 rounded-xl text-base px-4 border border-input focus-visible:ring-2 focus-visible:ring-primary w-full bg-input-background font-mono"
                {...field}
                onChange={(e) => {
                  const formatted = formatExpiryDate(e.target.value);
                  field.onChange(formatted);
                }}
              />
            )}
          />
          {errors.expiryDate && (
            <p className="text-xs text-red-500 font-semibold">{errors.expiryDate.message}</p>
          )}
        </div>

        {/* CVV */}
        <div className="space-y-1.5 relative">
          <Label htmlFor="cvv" className="text-sm font-semibold text-foreground">
            CVV
          </Label>
          <div className="relative">
            <Controller
              name="cvv"
              control={control}
              render={({ field }) => (
                <>
                  <Input
                    id="cvv"
                    type="password"
                    placeholder="123"
                    maxLength={4}
                    className="h-12 rounded-xl text-base px-4 pr-10 border border-input focus-visible:ring-2 focus-visible:ring-primary w-full bg-input-background font-mono"
                    {...field}
                    onChange={(e) => {
                      const cleaned = e.target.value.replace(/\D/g, "").substring(0, 4);
                      field.onChange(cleaned);
                    }}
                  />
                  <HelpCircle className="absolute right-3.5 top-1/2 -translate-y-1/2 h-4.5 w-4.5 text-muted-foreground shrink-0 cursor-help" />
                </>
              )}
            />
          </div>
          {errors.cvv && (
            <p className="text-xs text-red-500 font-semibold">{errors.cvv.message}</p>
          )}
        </div>
      </div>

      {/* Cardholder Name */}
      <div className="space-y-1.5">
        <Label htmlFor="cardholderName" className="text-sm font-semibold text-foreground">
          Cardholder name
        </Label>
        <Controller
          name="cardholderName"
          control={control}
          render={({ field }) => (
            <Input
              id="cardholderName"
              type="text"
              placeholder="Omar Ahmad"
              className="h-12 rounded-xl text-base px-4 border border-input focus-visible:ring-2 focus-visible:ring-primary w-full bg-input-background font-semibold"
              {...field}
            />
          )}
        />
        {errors.cardholderName && (
          <p className="text-xs text-red-500 font-semibold">{errors.cardholderName.message}</p>
        )}
      </div>

      {/* Important Banner */}
      {mode === "payment" ? (
        <div className="bg-muted/40 border border-border rounded-xl p-4 text-left space-y-2 mt-4">
          <div className="flex flex-col text-left">
            <span className="text-xs font-bold text-muted-foreground uppercase tracking-wider">
              Bid Deposit (10%):
            </span>
            <span className="text-xl font-black text-foreground mt-0.5">
              {formatCurrency(authorizationAmount)}
            </span>
          </div>
          <p className="text-xs leading-relaxed text-muted-foreground font-medium pt-2 border-t border-border/40">
            Your card will be saved securely. No funds will be held until you confirm your bid in the next step
          </p>
        </div>
      ) : (
        <div className="flex items-start gap-3 bg-muted/40 border border-border/50 rounded-xl p-4 text-xs md:text-sm text-muted-foreground mt-4 text-left">
          <Lock className="h-5 w-5 text-muted-foreground shrink-0 mt-0.5" />
          <div className="space-y-0.5">
            <p className="font-bold text-foreground">Important</p>
            <p className="leading-relaxed">
              {infoBannerText || "This payout method will be used to receive your earnings after a successful sale."}
            </p>
          </div>
        </div>
      )}

      {/* Buttons Actions */}
      <div className="pt-4">
        <Button
          type="submit"
          disabled={isSubmitting}
          className="w-full rounded-xl h-14 bg-[#0B1528] hover:bg-[#162238] text-white font-extrabold text-lg transition-all cursor-pointer flex items-center justify-center gap-2 shadow-sm"
        >
          {isSubmitting ? (
            <>
              <Loader2 className="h-5 w-5 animate-spin" />
              {mode === "payment" ? "Authorizing Card..." : "Verifying Card Details..."}
            </>
          ) : (
            <>
              <Lock className="h-4.5 w-4.5 shrink-0 stroke-[2.5]" />
              {submitButtonText || (mode === "payment" ? "Save & Authorize Bid" : "Save Payout Method")}
            </>
          )}
        </Button>
        
        <button
          type="button"
          onClick={onCancel}
          className="text-sm font-semibold text-muted-foreground hover:text-foreground mt-4 block mx-auto text-center cursor-pointer transition-colors"
        >
          Cancel
        </button>
      </div>
    </form>
  );
}
