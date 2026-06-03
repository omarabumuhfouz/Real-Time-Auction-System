import { cn } from "@/lib/utils";

export interface StatusPillItem {
  key: string;
  label: string;
  count: number;
  color: string;
}

export interface StatusPillBarProps {
  items: StatusPillItem[];
  activeKey: string;
  onSelect: (key: string) => void;
  className?: string;
}

export function StatusPillBar({
  items,
  activeKey,
  onSelect,
  className,
}: StatusPillBarProps) {
  return (
    <div className={cn("flex flex-wrap items-center gap-2.5", className)}>
      {items.map((item) => {
        const isActive = item.key === activeKey;
        return (
          <button
            key={item.key}
            type="button"
            onClick={() => onSelect(isActive ? "All" : item.key)}
            className={cn(
              "inline-flex items-center justify-between rounded-2xl w-32 h-12 px-3 text-xs font-semibold transition-colors cursor-pointer border",
              "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
              isActive
                ? "text-white shadow-sm border-transparent"
                : "bg-card text-muted-foreground hover:bg-muted border-border/80",
            )}
            style={
              isActive
                ? { backgroundColor: item.color }
                : undefined
            }
          >
            <div className="flex items-center gap-2">
              {!isActive && (
                <span
                  className="h-2 w-2 rounded-full shrink-0"
                  style={{ backgroundColor: item.color }}
                />
              )}
              <span>{item.label}</span>
            </div>
            <span
              className={cn(
                "tabular-nums font-bold text-xs px-2 py-0.5 rounded-md",
                isActive ? "text-white/90 bg-white/10" : "text-foreground bg-muted/60",
              )}
            >
              {item.count}
            </span>
          </button>
        );
      })}
    </div>
  );
}
