"use client";

import React from "react";
import { Info, Lock, CheckCircle2, AlertTriangle, RotateCcw, ExternalLink } from "lucide-react";
import { formatCurrency } from "@/utils/currency.utils";
import { cn } from "@/lib/utils";
import type { PaymentStats } from "../types/admin.types";

import { PaymentProviderStatusSkeleton } from "./skeletons";

interface PaymentProviderStatusProps {
  data?: PaymentStats;
  isLoading?: boolean;
}

interface StatItemProps {
  label: string;
  value: number;
  icon: React.ComponentType<{ className?: string }>;
  iconColor: string;
  iconBg: string;
}

function PaymentStatItem({ label, value, icon: Icon, iconColor, iconBg }: StatItemProps) {
  const formattedValue = formatCurrency(value);

  return (
    <div className="bg-background/40 border border-border rounded-lg p-3.5 flex items-center gap-3">
      {/* Icon */}
      <div className={cn("h-10 w-10 rounded-lg flex items-center justify-center shrink-0", iconBg)}>
        <Icon className={cn("h-5 w-5", iconColor)} />
      </div>
      {/* Labels */}
      <div className="flex flex-col">
        <span className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wide">
          {label}
        </span>
        <span className="text-base font-extrabold text-foreground mt-0.5 font-mono">
          {formattedValue}
        </span>
      </div>
    </div>
  );
}

export function PaymentProviderStatus({ data, isLoading }: PaymentProviderStatusProps) {
  if (isLoading || !data) {
    return <PaymentProviderStatusSkeleton />;
  }
  return (
    <div className="bg-card text-card-foreground border border-border rounded-xl p-5 md:p-6 flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between mb-5">
        <div className="flex items-center gap-1.5">
          <h3 className="text-base font-bold text-foreground">Payment Provider Status</h3>
          <span title="Integrated payment processor credentials and transaction sums">
            <Info className="h-4 w-4 text-muted-foreground cursor-help" />
          </span>
        </div>

        {/* Connection Badge */}
        {data.isConnected ? (
          <span className="flex items-center gap-1 text-[10px] font-semibold text-success-foreground px-2 py-0.5 bg-success/20 rounded-full">
            <span className="h-1.5 w-1.5 rounded-full bg-[oklch(0.529614_0.149485_148.9899)] animate-pulse" />
            Provider Connected
          </span>
        ) : (
          <span className="flex items-center gap-1 text-[10px] font-semibold text-destructive px-2 py-0.5 bg-destructive/15 rounded-full">
            <span className="h-1.5 w-1.5 rounded-full bg-destructive" />
            Provider Offline
          </span>
        )}
      </div>

      {/* Grid mapping payments stats */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 flex-1">
        <PaymentStatItem
          label="Held Funds"
          value={data.heldFunds}
          icon={Lock}
          iconColor="text-[oklch(0.4878_0.2432_264.4)]" // info blue
          iconBg="bg-info/25"
        />
        <PaymentStatItem
          label="Completed Payments"
          value={data.completedPayments}
          icon={CheckCircle2}
          iconColor="text-[oklch(0.529614_0.149485_148.9899)]" // success green
          iconBg="bg-success/25"
        />
        <PaymentStatItem
          label="Failed Payments"
          value={data.failedPayments}
          icon={AlertTriangle}
          iconColor="text-[oklch(0.5095_0.2086_28.51)]" // destructive red
          iconBg="bg-destructive/15"
        />
        <PaymentStatItem
          label="Refunds / Chargebacks"
          value={data.refundsChargebacks}
          icon={RotateCcw}
          iconColor="text-[oklch(0.7049_0.1989_45.77)]" // primary orange
          iconBg="bg-primary/10"
        />
      </div>

      {/* Footer */}
      <div className="flex items-center justify-between mt-5 pt-4 border-t border-border text-[11px] text-muted-foreground">
        <span>Last Sync: {data.lastSync}</span>
        <a
          href="https://stripe.com"
          target="_blank"
          rel="noopener noreferrer"
          className="flex items-center gap-1 text-primary font-semibold hover:underline"
        >
          <span>View Provider Dashboard</span>
          <ExternalLink className="h-3 w-3" />
        </a>
      </div>
    </div>
  );
}
