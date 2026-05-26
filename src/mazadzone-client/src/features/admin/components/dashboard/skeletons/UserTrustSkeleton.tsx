import { Skeleton } from "@/components/ui/skeleton";

export function UserTrustSkeleton() {
  return (
    <div className="bg-card text-card-foreground border border-border rounded-xl p-5 md:p-6 flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center gap-2 mb-6">
        <Skeleton className="h-5 w-24" />
        <Skeleton className="h-4 w-4 rounded-full" />
      </div>

      {/* Funnel Steps Row */}
      <div className="grid grid-cols-1 xs:grid-cols-2 gap-4 items-center mb-6">
        {Array.from({ length: 2 }).map((_, index) => (
          <div key={index} className="flex items-center w-full">
            <div className="flex-1 bg-background/50 border border-border rounded-lg p-3 flex items-center gap-3 relative">
              {/* Step badge */}
              <Skeleton className="absolute top-2 right-2 h-5 w-5 rounded-full" />
              {/* Icon */}
              <Skeleton className="h-10 w-10 rounded-md shrink-0" />
              {/* Labels */}
              <div className="flex flex-col gap-1.5 flex-1">
                <Skeleton className="h-3 w-16" />
                <Skeleton className="h-4.5 w-12" />
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Account Status Overview */}
      <div className="flex flex-col gap-6 flex-1 justify-between">
        <div className="space-y-4">
          <Skeleton className="h-3.5 w-36" />

          <div className="space-y-4">
            {Array.from({ length: 3 }).map((_, index) => (
              <div key={index} className="space-y-2">
                <div className="flex justify-between items-center">
                  <div className="flex items-center gap-2">
                    <Skeleton className="h-2.5 w-2.5 rounded-full shrink-0" />
                    <Skeleton className="h-3.5 w-20" />
                  </div>
                  <div className="flex gap-4">
                    <Skeleton className="h-3.5 w-8" />
                    <Skeleton className="h-3.5 w-8" />
                  </div>
                </div>
                {/* Progress bar line */}
                <Skeleton className="h-2 w-full rounded-full" />
              </div>
            ))}
          </div>
        </div>

        {/* Trust Score Box */}
        <div className="bg-background/40 border border-border rounded-xl p-4 flex flex-col items-center justify-center text-center mt-2">
          <Skeleton className="h-10 w-10 rounded-full mb-2" />
          <Skeleton className="h-7 w-16" />
          <Skeleton className="h-3 w-20 mt-2" />
          <Skeleton className="h-5 w-32 rounded-md mt-2" />
        </div>
      </div>
    </div>
  );
}
