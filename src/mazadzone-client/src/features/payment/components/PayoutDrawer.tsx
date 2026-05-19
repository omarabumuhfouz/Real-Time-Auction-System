"use client";

import { useState, useEffect } from "react";
import { createPortal } from "react-dom";
import { X, Shield, ShieldCheck } from "lucide-react";

import { useAuthStore } from "@/stores/auth.store";
import { type PayoutDetails } from "../types";
import { CreditCardPayoutForm } from "./CreditCardPayoutForm";
import { 
  type CreditCardFormValues 
} from "../validations/creditCard.schema";

interface PayoutDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (details: PayoutDetails) => void;
}

export function PayoutDrawer({ isOpen, onClose, onSave }: PayoutDrawerProps) {
  const { user } = useAuthStore();
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Expose slide-in animation state
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => {
      if (isOpen) {
        setIsMounted(true);
        document.body.style.overflow = "hidden"; // Disable page scrolling
      } else {
        setIsMounted(false);
        document.body.style.overflow = "unset";
      }
    }, 0);
    return () => {
      clearTimeout(timer);
      document.body.style.overflow = "unset";
    };
  }, [isOpen]);

  // Card brand detection inside Card Number input for mapping cardType
  const getCardBrand = (cardNumberVal: string) => {
    const cleanNum = cardNumberVal.replace(/\s/g, "");
    if (cleanNum.startsWith("4")) return "VISA";
    if (/^5[1-5]/.test(cleanNum)) return "MASTERCARD";
    if (/^3[47]/.test(cleanNum)) return "AMEX";
    return "";
  };

  // Submission handlers
  const handleCardSave = async (data: CreditCardFormValues) => {
    setIsSubmitting(true);
    try {
      await new Promise((resolve) => setTimeout(resolve, 1200)); // Simulated secure processing
      const cardBrand = getCardBrand(data.cardNumber) || "Visa";
      
      onSave({
        type: "card",
        cardNumber: data.cardNumber,
        cardType: cardBrand,
        expiryDate: data.expiryDate,
        cvv: data.cvv,
        cardholderName: data.cardholderName,
      });

      onClose();
    } catch (e) {
      console.error("Card payment processing failed", e);
    } finally {
      setIsSubmitting(false);
    }
  };

  // Client mount check to support SSR/Next.js hydration safety
  const [mounted, setMounted] = useState(false);
  useEffect(() => {
    const timer = setTimeout(() => {
      setMounted(true);
    }, 0);
    return () => {
      clearTimeout(timer);
      setMounted(false);
    };
  }, []);

  if (!isOpen || !mounted) return null;

  return createPortal(
    <div className="fixed inset-0 z-100 flex justify-end">
      {/* Backdrop overlay */}
      <div 
        onClick={onClose}
        className="fixed inset-0 bg-black/60 backdrop-blur-sm transition-opacity duration-300 animate-fade-in"
      />

      {/* Drawer panel */}
      <div 
        className={`relative w-full sm:w-[500px] h-full bg-card border-l border-border shadow-2xl flex flex-col justify-between overflow-y-auto transition-transform duration-300 z-50 ${
          isMounted ? "translate-x-0" : "translate-x-full"
        } animate-slide-in-right`}
      >
        
        {/* Scrollable Form Body Container */}
        <div className="p-6 sm:p-8 space-y-6 flex-1 text-left">
          
          {/* Header Secure & Powered Title */}
          <div className="flex items-start justify-between border-b border-border/40 pb-4 relative">
            <div className="space-y-1 text-left">
              <h3 className="text-xl font-bold text-foreground tracking-tight">
                Add Payout Method (Secure)
              </h3>
              <div className="flex items-center gap-1.5 text-[#635BFF] font-bold text-[13px]">
                <span className="text-muted-foreground font-semibold">Powered by</span>
                <ShieldCheck className="h-4 w-4 text-[#635BFF] shrink-0 stroke-[2.5]" />
                <span className="tracking-tight text-sm font-extrabold">Stripe</span>
              </div>
            </div>
            <button
              type="button"
              onClick={onClose}
              className="text-muted-foreground hover:text-foreground transition-colors cursor-pointer rounded-full hover:bg-muted p-1.5 shrink-0"
            >
              <X className="h-5 w-5" />
            </button>
          </div>

          {/* Secure Information Blue Banner */}
          <div className="flex items-start gap-3 bg-[#EAF1FC] dark:bg-blue-950/20 border border-blue-100 dark:border-blue-900/30 rounded-xl p-4 text-xs md:text-sm text-[#1A73E8] dark:text-blue-300">
            <Shield className="h-5 w-5 text-[#1A73E8] shrink-0 mt-0.5" />
            <p className="leading-relaxed font-semibold">
              Your payment information is handled securely by Stripe. MazadZone does not store your full card details.
            </p>
          </div>

          {/* Render Modular Credit Card Sub-form */}
          <CreditCardPayoutForm 
            onSave={handleCardSave} 
            onCancel={onClose} 
            isSubmitting={isSubmitting} 
            defaultCardholderName={user?.fullName}
          />

        </div>
      </div>
    </div>,
    document.body
  );
}
