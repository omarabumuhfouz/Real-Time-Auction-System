import { Skeleton } from "@/components/ui/skeleton";

export function OpenDisputesBreakdownSkeleton() {
  return (
    <div className="bg-card text-card-foreground border border-border rounded-xl p-5 md:p-6 flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2">
          <Skeleton className="h-5 w-44" />
          <Skeleton className="h-4 w-4 rounded-full" />
        </div>
      </div>

      {/* Donut and Queue list layout */}
      <div className="flex flex-col sm:flex-row xl:flex-col 2xl:flex-row items-center justify-between gap-6 flex-1 py-2">
        {/* Left: Donut Chart Circle Skeleton */}
        <div className="relative w-44 h-44 flex items-center justify-center shrink-0">
          <div className="w-40 h-40 rounded-full border-[18px] border-muted animate-pulse flex items-center justify-center">
            <div className="flex flex-col items-center">
              <Skeleton className="h-6 w-10 mb-1" />
              <Skeleton className="h-2 w-16" />
            </div>
          </div>
        </div>

        {/* Right: Queue List Skeletons */}
        <div className="flex-1 w-full space-y-3.5">
          {/* Header Labels */}
          <div className="flex justify-between items-center pb-2 border-b border-border">
            <Skeleton className="h-3 w-20" />
            <div className="flex gap-8">
              <Skeleton className="h-3 w-8" />
              <Skeleton className="h-3 w-16" />
            </div>
          </div>

          {/* List items */}
          <div className="space-y-4">
            {Array.from({ length: 4 }).map((_, index) => (
              <div key={index} className="flex justify-between items-center">
                {/* Dot + Label */}
                <div className="flex items-center gap-2">
                  <Skeleton className="h-2.5 w-2.5 rounded-full shrink-0" />
                  <Skeleton className="h-3.5 w-24 sm:w-28" />
                </div>
                {/* Count & Percent */}
                <div className="flex items-center gap-8">
                  <Skeleton className="h-3.5 w-6" />
                  <Skeleton className="h-3.5 w-12 rounded-md" />
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Review CTA panel */}
      <div className="mt-5 p-3.5 bg-accent/40 rounded-lg flex flex-col sm:flex-row xl:flex-col 2xl:flex-row items-start sm:items-center xl:items-start 2xl:items-center justify-between gap-4 border border-accent">
        <div className="flex items-start gap-2.5 flex-1">
          <Skeleton className="h-5 w-5 rounded-full shrink-0" />
          <div className="space-y-1.5 flex-1">
            <Skeleton className="h-3 w-full" />
            <Skeleton className="h-3 w-2/3" />
          </div>
        </div>
        <Skeleton className="h-8 w-full sm:w-32 xl:w-full 2xl:w-32 rounded-md shrink-0" />
      </div>
    </div>
  );
}
