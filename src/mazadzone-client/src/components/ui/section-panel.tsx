import type { ReactNode } from "react";
import { cn } from "@/lib/utils";

export interface SectionPanelProps {
  title: string;
  subtitle?: string;
  action?: ReactNode;
  children: ReactNode;
  className?: string;
  /** Remove default padding — useful when children manage their own padding */
  noPadding?: boolean;
}

export function SectionPanel({
  title,
  subtitle,
  action,
  children,
  className,
  noPadding = false,
}: SectionPanelProps) {
  return (
    <section
      className={cn(
        "bg-card border border-border rounded-xl",
        !noPadding && "p-6",
        className,
      )}
    >
      {/* Header */}
      <div
        className={cn(
          "flex flex-col sm:flex-row sm:items-center justify-between gap-3",
          noPadding && "px-6 pt-6",
          "pb-4 border-b border-border/50 mb-5",
        )}
      >
        <div className="text-left">
          <h3 className="text-base font-bold tracking-tight text-foreground">
            {title}
          </h3>
          {subtitle && (
            <p className="text-xs text-muted-foreground mt-0.5">
              {subtitle}
            </p>
          )}
        </div>
        {action && <div className="shrink-0">{action}</div>}
      </div>

      {/* Content */}
      <div className={cn(noPadding && "px-6 pb-6")}>{children}</div>
    </section>
  );
}
