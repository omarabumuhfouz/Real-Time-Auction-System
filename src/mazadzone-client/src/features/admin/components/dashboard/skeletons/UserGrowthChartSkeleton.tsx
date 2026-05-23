import { Skeleton } from "@/components/ui/skeleton";

export function UserGrowthChartSkeleton() {
  return (
    <div className="bg-card text-card-foreground border border-border rounded-xl p-5 md:p-6 flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-2">
          <Skeleton className="h-5 w-44" />
          <Skeleton className="h-4 w-4 rounded-full" />
        </div>
        <Skeleton className="h-8 w-24 rounded-md" />
      </div>

      {/* Chart Plot Area */}
      <div className="flex-1 min-h-[300px] flex items-end gap-4 pt-4 px-2">
        {Array.from({ length: 6 }).map((_, index) => (
          <div key={index} className="flex-1 flex flex-col items-center gap-2 h-full justify-end">
            <Skeleton
              className="w-full rounded-t-md"
              style={{
                height: `${[40, 70, 50, 90, 60, 75][index]}%`,
                opacity: 0.85 - index * 0.05,
              }}
            />
            <Skeleton className="h-3 w-10 mt-1" />
          </div>
        ))}
      </div>

      {/* Footer Metrics */}
      <div className="grid grid-cols-2 gap-4 mt-6 pt-6 border-t border-border">
        <div className="flex flex-col gap-1.5">
          <Skeleton className="h-3.5 w-28" />
          <div className="flex items-baseline gap-2 mt-1">
            <Skeleton className="h-7 w-20" />
            <Skeleton className="h-4 w-10 rounded-md" />
          </div>
        </div>
        <div className="flex flex-col gap-1.5">
          <Skeleton className="h-3.5 w-28" />
          <div className="flex items-baseline gap-2 mt-1">
            <Skeleton className="h-7 w-20" />
            <Skeleton className="h-4 w-10 rounded-md" />
          </div>
        </div>
      </div>
    </div>
  );
}
