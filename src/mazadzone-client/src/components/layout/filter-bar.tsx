import React from "react";
import { cn } from "@/lib/utils";

interface FilterBarProps extends React.HTMLAttributes<HTMLDivElement> {
  search?: React.ReactNode;
  filters?: React.ReactNode;
  actions?: React.ReactNode;
}

export function FilterBar({
  search,
  filters,
  actions,
  className,
  ...props
}: FilterBarProps) {
  return (
    <div
      className={cn(
        "bg-card text-card-foreground border border-border rounded-xl p-4 md:p-5 flex flex-col md:flex-row items-start md:items-center justify-between gap-4 w-full shadow-xs",
        className
      )}
      {...props}
    >
      {/* Left Column: Search Input & Selector Filters */}
      <div className="flex flex-col md:flex-row items-stretch md:items-center gap-4 w-full md:w-auto flex-1 min-w-0">
        {search && (
          <div className="w-full md:w-80 lg:w-96 shrink-0">
            {search}
          </div>
        )}
        {filters && (
          <div className="flex flex-wrap items-center gap-3 w-full md:w-auto overflow-x-auto pb-1 md:pb-0 flex-1 min-w-0">
            {filters}
          </div>
        )}
      </div>

      {/* Right Column: Export, Bulk Actions, etc. */}
      {actions && (
        <div className="flex items-center gap-3 self-end md:self-auto justify-end shrink-0 w-full md:w-auto mt-2 md:mt-0 pt-2 md:pt-0">
          {actions}
        </div>
      )}
    </div>
  );
}
