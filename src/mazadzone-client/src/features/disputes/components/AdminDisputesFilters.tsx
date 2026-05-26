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
    <div className="flex flex-col xl:flex-row gap-4 p-4 border border-border bg-card rounded-2xl items-start xl:items-center justify-between shadow-sm">
      <div className="flex flex-wrap items-center gap-4 flex-1 w-full xl:w-auto">
        <div className="relative w-full sm:w-[320px]">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-muted-foreground" />
          <Input
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search disputes by ID, order, auction, or user..."
            className="pl-9 bg-card border-border h-10 w-full"
          />
        </div>

        <div className="flex flex-col gap-1 w-full sm:w-auto min-w-[160px]">
          <span className="text-[10px] font-bold text-foreground/80 px-1 uppercase tracking-wider">Dispute Status</span>
          <Select value={status} onValueChange={setStatus}>
            <SelectTrigger className="h-10 bg-card border-border">
              <SelectValue placeholder="All Statuses" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="All Statuses">All Statuses</SelectItem>
              {Object.values(AdminDisputeStatus).map((s) => (
                <SelectItem key={s} value={s}>
                  {s}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="flex flex-col gap-1 w-full sm:w-auto min-w-[160px]">
          <span className="text-[10px] font-bold text-foreground/80 px-1 uppercase tracking-wider">Category</span>
          <Select value={category} onValueChange={setCategory}>
            <SelectTrigger className="h-10 bg-card border-border">
              <SelectValue placeholder="All Categories" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="All Categories">All Categories</SelectItem>
              {Object.values(AdminDisputeCategory).map((c) => (
                <SelectItem key={c} value={c}>
                  {c}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="flex flex-col gap-1 w-full sm:w-auto min-w-[160px]">
          <span className="text-[10px] font-bold text-foreground/80 px-1 uppercase tracking-wider">Sort By</span>
          <Select defaultValue="Submitted Date">
            <SelectTrigger className="h-10 bg-card border-border">
              <SelectValue placeholder="Sort By" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="Submitted Date">Submitted Date</SelectItem>
              <SelectItem value="Status">Status</SelectItem>
            </SelectContent>
          </Select>
        </div>

        <div className="flex flex-col gap-1 w-full sm:w-[160px]">
          <span className="text-[10px] font-bold text-foreground/80 px-1 uppercase tracking-wider">Submitted Date</span>
          <div className="relative">
            <Input
              type="date"
              className="h-10 pr-9 bg-card border-border text-foreground/80"
              placeholder="Select date"
            />
            <Calendar className="absolute right-3 top-1/2 -translate-y-1/2 size-4 text-muted-foreground pointer-events-none" />
          </div>
        </div>
      </div>

      <div className="flex items-center gap-3 w-full xl:w-auto justify-end mt-4 xl:mt-0">
        <Button variant="outline" className="h-10 gap-2 font-bold px-4 hover:bg-muted text-foreground">
          <Download className="size-4" />
          Export
        </Button>
        <Button className="h-10 bg-primary text-primary-foreground hover:bg-primary/90 font-bold px-6">
          Bulk Actions
        </Button>
      </div>
    </div>
  );
}
