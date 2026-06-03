"use client";

import { cn } from "@/lib/utils";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

export interface ActivityFiltersProps {
  activeFilter: string;
  setActiveFilter: (filter: string) => void;
  sortBy: string;
  setSortBy: (sortBy: string) => void;
  filters: readonly string[];
  sortPlaceholder?: string;
}

/**
 * ActivityFilters Component
 * 
 * A unified, reusable filtering and sorting control bar designed for activity dashboard layouts.
 * Combines status-based filter buttons on the left with a chronological sort dropdown selector on the right.
 * 
 * @param activeFilter - The currently selected filter value.
 * @param setActiveFilter - A callback function triggered when a filter button is clicked.
 * @param sortBy - The currently selected sorting order value.
 * @param setSortBy - A callback function triggered when the sorting option is changed.
 * @param filters - An array of filter names to display as clickable buttons.
 * @param sortPlaceholder - Custom text placeholder shown in the sorting trigger.
 */
export function ActivityFilters({
  activeFilter,
  setActiveFilter,
  sortBy,
  setSortBy,
  filters,
  sortPlaceholder = "Sort by Date",
}: ActivityFiltersProps) {
  return (
    <div className="flex flex-col md:flex-row items-start md:items-center justify-between gap-4 mb-8">
      <div className="flex flex-wrap items-center gap-2">
        {filters.map((filter) => (
          <button
            key={filter}
            onClick={() => setActiveFilter(filter)}
            className={cn(
              "rounded-full font-medium transition-colors h-12 px-6 flex items-center justify-center shrink-0 text-base cursor-pointer",
              activeFilter === filter
                ? "bg-primary text-white"
                : "bg-gray-100 text-gray-600 hover:bg-gray-200"
            )}
          >
            {filter}
          </button>
        ))}
      </div>

      <div className="w-48">
        <Select value={sortBy} onValueChange={setSortBy}>
          <SelectTrigger className="w-full cursor-pointer">
            <SelectValue placeholder={sortPlaceholder} />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="latest">Latest</SelectItem>
            <SelectItem value="oldest">Oldest</SelectItem>
          </SelectContent>
        </Select>
      </div>
    </div>
  );
}
