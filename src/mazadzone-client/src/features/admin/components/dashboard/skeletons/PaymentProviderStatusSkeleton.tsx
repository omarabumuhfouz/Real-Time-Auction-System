import { Skeleton } from "@/components/ui/skeleton";

export function PaymentProviderStatusSkeleton() {
  return (
    <div className="bg-card text-card-foreground border border-border rounded-xl p-5 md:p-6 flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between mb-5">
        <div className="flex items-center gap-2">
          <Skeleton className="h-5 w-48" />
          <Skeleton className="h-4 w-4 rounded-full" />
        </div>
        {/* Connection Badge */}
        <Skeleton className="h-5 w-28 rounded-full" />
      </div>

      {/* Grid mapping payments stats */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 flex-1">
        {Array.from({ length: 4 }).map((_, index) => (
          <div key={index} className="bg-background/40 border border-border rounded-lg p-3.5 flex items-center gap-3">
            {/* Icon */}
            <Skeleton className="h-10 w-10 rounded-lg shrink-0" />
            {/* Labels */}
            <div className="flex flex-col gap-1.5 flex-1">
              <Skeleton className="h-3 w-16" />
              <Skeleton className="h-4.5 w-24" />
            </div>
          </div>
        ))}
      </div>

      {/* Footer */}
      <div className="flex items-center justify-between mt-5 pt-4 border-t border-border">
        <Skeleton className="h-3.5 w-32" />
        <Skeleton className="h-3.5 w-36" />
      </div>
    </div>
  );
}
