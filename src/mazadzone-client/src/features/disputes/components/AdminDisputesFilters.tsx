import { Search, Calendar, Download } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { FilterBar } from "@/components/layout/filter-bar";
import { AdminDisputeCategory, AdminDisputeStatus } from "../types/admin-disputes.types";

interface AdminDisputesFiltersProps {
  search: string;
  setSearch: (val: string) => void;
  status: string;
  setStatus: (val: string) => void;
  category: string;
  setCategory: (val: string) => void;
}

export function AdminDisputesFilters({
  search,
  setSearch,
  status,
  setStatus,
  category,
  setCategory,
}: AdminDisputesFiltersProps) {
  return (
    <FilterBar
      search={
        <div className="flex flex-col gap-1.5 w-full">
          <span className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider pl-1">Search</span>
          <div className="relative">
            <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 h-4 w-4 text-black/70" />
            <Input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Search disputes by ID, order, auction, or user..."
              className="pl-9 h-9 w-full text-xs bg-white text-black border-transparent placeholder:text-black/50 focus-visible:ring-foreground/20 shadow-sm"
            />
          </div>
        </div>
      }
      filters={
        <>
          {/* Dispute Status */}
          <div className="flex flex-col gap-1.5 min-w-[160px]">
            <span className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider pl-1">Dispute Status</span>
            <Select value={status} onValueChange={setStatus}>
              <SelectTrigger className="h-9 w-full text-xs rounded-lg cursor-pointer">
                <SelectValue placeholder="All Statuses" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="All Statuses">All Statuses</SelectItem>
                {Object.values(AdminDisputeStatus).map((s) => (
                  <SelectItem key={s} value={s} className="cursor-pointer">
                    {s}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Category */}
          <div className="flex flex-col gap-1.5 min-w-[160px]">
            <span className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider pl-1">Category</span>
            <Select value={category} onValueChange={setCategory}>
              <SelectTrigger className="h-9 w-full text-xs rounded-lg cursor-pointer">
                <SelectValue placeholder="All Categories" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="All Categories">All Categories</SelectItem>
                {Object.values(AdminDisputeCategory).map((c) => (
                  <SelectItem key={c} value={c} className="cursor-pointer">
                    {c}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Sort By */}
          <div className="flex flex-col gap-1.5 min-w-[160px]">
            <span className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider pl-1">Sort By</span>
            <Select defaultValue="Submitted Date">
              <SelectTrigger className="h-9 w-full text-xs rounded-lg cursor-pointer">
                <SelectValue placeholder="Sort By" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Submitted Date" className="cursor-pointer">Submitted Date</SelectItem>
                <SelectItem value="Status" className="cursor-pointer">Status</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Submitted Date */}
          <div className="flex flex-col gap-1.5 min-w-[160px]">
            <span className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider pl-1">Submitted Date</span>
            <div className="relative">
              <Input
                type="date"
                className="h-9 text-xs bg-white text-black border-border pr-8"
                placeholder="Select date"
              />
              <Calendar className="absolute right-2.5 top-1/2 -translate-y-1/2 size-3.5 text-muted-foreground pointer-events-none" />
            </div>
          </div>
        </>
      }
      actions={
        <>
          <Button variant="outline" className="h-9 px-4 text-xs font-semibold gap-2 border-border shadow-xs hover:bg-muted text-foreground">
            <Download className="size-3.5" />
            Export
          </Button>
          <Button className="h-9 px-6 text-xs font-semibold bg-primary text-primary-foreground hover:bg-primary/90 shadow-xs transition-colors">
            Bulk Actions
          </Button>
        </>
      }
    />
  );
}
