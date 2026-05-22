import { Skeleton } from "@/components/ui/skeleton";

export function MetricCardSkeleton() {
  return (
    <div className="bg-card text-card-foreground border border-border rounded-xl p-4 sm:p-5">
      <div className="flex items-start justify-between gap-3">
        <div className="flex flex-col gap-1.5 min-w-0 flex-1">
          {/* Title */}
          <Skeleton className="h-4 w-24 sm:w-28" />
          {/* Value */}
          <Skeleton className="h-8 w-16 sm:w-20 mt-1" />
        </div>
        {/* Icon */}
        <Skeleton className="h-10 w-10 sm:h-12 sm:w-12 rounded-lg shrink-0" />
      </div>
      {/* Footer */}
      <div className="flex items-center gap-1.5 mt-5">
        <Skeleton className="h-5 w-12 rounded-md" />
        <Skeleton className="h-3.5 w-16" />
      </div>
    </div>
  );
}
