import { cn } from "@/lib/utils";

export interface FactItem {
  label: string;
  value: string;
  muted?: boolean;
}

export interface InlineFactsProps {
  facts: FactItem[];
  className?: string;
}

export function InlineFacts({ facts, className }: InlineFactsProps) {
  return (
    <div
      className={cn(
        "flex flex-wrap items-center gap-x-1 gap-y-2 rounded-lg border border-border bg-card/50 px-4 py-3",
        className,
      )}
    >
      {facts.map((fact, i) => (
        <div key={fact.label} className="flex items-center gap-1">
          {/* Separator dot (skip for the first item) */}
          {i > 0 && (
            <span className="text-border mx-2 hidden sm:inline" aria-hidden="true">
              ·
            </span>
          )}
          <span
            className={cn(
              "text-xs font-medium",
              fact.muted
                ? "text-muted-foreground/50"
                : "text-muted-foreground",
            )}
          >
            {fact.label}:
          </span>
          <span
            className={cn(
              "text-sm font-bold",
              fact.muted
                ? "text-foreground/40"
                : "text-foreground",
            )}
          >
            {fact.value}
          </span>
        </div>
      ))}
    </div>
  );
}
