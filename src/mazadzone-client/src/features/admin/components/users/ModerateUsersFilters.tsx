import React, { useState, useEffect } from "react";
import { Search, Calendar as CalendarIcon, Download, ChevronDown } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Spinner } from "@/components/ui/spinner";
import type { UseModerateUsersFilters } from "../../api";
import type { ModerateUserRole, ModerateUserStatus } from "../../types/admin.types";

interface ModerateUsersFiltersProps {
  filters: UseModerateUsersFilters;
  onFilterChange: (newFilters: Partial<UseModerateUsersFilters>) => void;
  isExporting: boolean;
  onExport: () => void;
  selectedIds: string[];
  onBulkActivate: () => void;
  onBulkSuspend: () => void;
  onBulkBan: () => void;
}

export function ModerateUsersFilters({
  filters,
  onFilterChange,
  isExporting,
  onExport,
  selectedIds,
  onBulkActivate,
  onBulkSuspend,
  onBulkBan
}: ModerateUsersFiltersProps) {
  const hasSelection = selectedIds.length > 0;

  // Local search state for debouncing
  const [localSearch, setLocalSearch] = useState(filters.search || "");

  // Keep local search query in sync when search filter changes from elsewhere (e.g. URL query)
  useEffect(() => {
    setLocalSearch(filters.search || "");
  }, [filters.search]);

  // Apply search filter after 400ms debounce
  useEffect(() => {
    const delayDebounceFn = setTimeout(() => {
      if (localSearch !== filters.search) {
        onFilterChange({ search: localSearch, page: 1 });
      }
    }, 400);

    return () => clearTimeout(delayDebounceFn);
  }, [localSearch, filters.search, onFilterChange]);

  return (
    <div className="bg-card text-card-foreground border border-border rounded-xl p-4 md:p-5 flex flex-col md:flex-row items-start md:items-center justify-between gap-4 w-full">

      {/* Left side: Search and Selects */}
      <div className="flex flex-col md:flex-row items-center gap-3 w-full md:w-auto">
        {/* Search */}
        <div className="relative w-full md:w-80 lg:w-96">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-black/70" />
          <Input
            type="text"
            placeholder="Search users by name or email..."
            className="pl-9 h-9 w-full text-xs bg-white text-black border-transparent placeholder:text-black/50 focus-visible:ring-foreground/20 shadow-sm"
            value={localSearch}
            onChange={(e) => setLocalSearch(e.target.value)}
          />
        </div>

        <div className="flex items-center gap-3 w-full md:w-auto overflow-x-auto pb-1 md:pb-0">
          {/* Role */}
          <div className="flex flex-col gap-1.5 min-w-[120px]">
            <span className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider pl-1">Role</span>
            <Select
              value={filters.role}
              onValueChange={(val) => onFilterChange({ role: val as ModerateUserRole | "All Roles", page: 1 })}
            >
              <SelectTrigger className="cursor-pointer">
                <SelectValue placeholder="All Roles" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="All Roles" className="cursor-pointer">All Roles</SelectItem>
                <SelectItem value="Bidder" className="cursor-pointer">Bidder</SelectItem>
                <SelectItem value="Seller" className="cursor-pointer">Seller</SelectItem>
                <SelectItem value="Admin" className="cursor-pointer">Admin</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Account Status */}
          <div className="flex flex-col gap-1.5 min-w-[130px]">
            <span className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider pl-1">Account Status</span>
            <Select
              value={filters.status}
              onValueChange={(val) => onFilterChange({ status: val as ModerateUserStatus | "All Statuses", page: 1 })}
            >
              <SelectTrigger className="cursor-pointer">
                <SelectValue placeholder="All Statuses" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="All Statuses" className="cursor-pointer">All Statuses</SelectItem>
                <SelectItem value="Active" className="cursor-pointer">Active</SelectItem>
                <SelectItem value="Suspended" className="cursor-pointer">Suspended</SelectItem>
                <SelectItem value="Banned" className="cursor-pointer">Banned</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Sort By */}
          <div className="flex flex-col gap-1.5 min-w-[120px]">
            <span className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider pl-1">Sort By</span>
            <Select
              value={filters.sortBy}
              onValueChange={(val) => onFilterChange({ sortBy: val, page: 1 })}
            >
              <SelectTrigger className="cursor-pointer">
                <SelectValue placeholder="Date Joined" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="dateJoined" className="cursor-pointer">Date Joined</SelectItem>
                <SelectItem value="name" className="cursor-pointer">Name</SelectItem>
                <SelectItem value="lastActive" className="cursor-pointer">Last Active</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Joined Date */}
          <div className="flex flex-col gap-1.5 min-w-[140px]">
            <span className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider pl-1">Joined Date</span>
            <Input
              type="date"
              className="h-9 text-xs bg-background border-border text-muted-foreground"
              placeholder="Select date"
            />
          </div>
        </div>
      </div>

      {/* Right side: Actions */}
      <div className="flex items-center gap-3 self-end md:self-auto w-full md:w-auto justify-end mt-2 md:mt-0 pt-2 md:pt-0">
        <Button
          variant="outline"
          className="h-9 px-4 text-xs font-semibold gap-2 border-border shadow-xs"
          onClick={onExport}
          disabled={isExporting}
        >
          {isExporting ? <Spinner size="sm" className="text-foreground" /> : <Download className="h-3.5 w-3.5" />}
          Export All
        </Button>

        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              className="h-9 px-4 text-xs font-semibold gap-1.5 bg-primary hover:bg-primary/90 text-primary-foreground shadow-xs transition-colors"
              disabled={!hasSelection}
            >
              Bulk Actions {hasSelection && `(${selectedIds.length})`}
              <ChevronDown className="h-3.5 w-3.5 opacity-80" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-48 bg-card text-foreground border-border" sideOffset={5}>
            <DropdownMenuItem className="text-xs cursor-pointer text-success-foreground" onClick={onBulkActivate}>
              Activate Selected
            </DropdownMenuItem>
            <DropdownMenuItem className="text-xs cursor-pointer text-muted-foreground" onClick={onBulkSuspend}>
              Suspend Selected
            </DropdownMenuItem>
            <DropdownMenuItem className="text-xs text-destructive focus:text-destructive cursor-pointer" onClick={onBulkBan}>
              Ban Selected
            </DropdownMenuItem>
            <DropdownMenuItem className="text-xs font-semibold cursor-pointer border-t border-border mt-1 pt-1" onClick={onExport}>
              Export Selected
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  );
}

