"use client";

import { useFormContext } from "react-hook-form";
import { AlertCircle } from "lucide-react";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";

export function AuctionTimingForm() {
  const {
    register,
    formState: { errors },
  } = useFormContext();

  return (
    <div className="space-y-5 pt-2">
      <h3 className="text-lg font-bold text-foreground tracking-tight border-b border-border/40 pb-3">
        Auction Timing
      </h3>
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
        {/* Start Date & Time */}
        <div className="space-y-2 text-left">
          <Label htmlFor="startDate" className="text-base font-bold text-foreground flex items-center gap-1">
            Start Date & Time <span className="text-red-500">*</span>
          </Label>
          <div className="relative">
            <Input
              id="startDate"
              type="datetime-local"
              className="h-12 rounded-xl border border-input bg-input-background text-base px-4 pr-10 focus-visible:ring-2 focus-visible:ring-primary w-full text-foreground"
              {...register("startDate")}
            />
          </div>
          {errors.startDate && (
            <p className="text-xs text-red-500 font-bold">{errors.startDate.message as string}</p>
          )}
        </div>

        {/* End Date & Time */}
        <div className="space-y-2 text-left">
          <Label htmlFor="endDate" className="text-base font-bold text-foreground flex items-center gap-1">
            End Date & Time <span className="text-red-500">*</span>
          </Label>
          <div className="relative">
            <Input
              id="endDate"
              type="datetime-local"
              className="h-12 rounded-xl border border-input bg-input-background text-base px-4 pr-10 focus-visible:ring-2 focus-visible:ring-primary w-full text-foreground"
              {...register("endDate")}
            />
          </div>
          {errors.endDate && (
            <p className="text-xs text-red-500 font-bold">{errors.endDate.message as string}</p>
          )}
        </div>
      </div>
      
      <div className="text-xs text-muted-foreground font-semibold space-y-2 bg-muted/40 p-3 rounded-lg border border-border/20">
        <div className="flex items-start gap-1.5 text-left">
          <AlertCircle className="h-4 w-4 text-muted-foreground shrink-0 mt-0.5" />
          <div className="space-y-1.5">
            <p>
              The auction will remain in pending view for you and appear as &quot;Upcoming&quot; to bidders until the start time, when it will automatically go active.
            </p>
            <p className="text-amber-600 dark:text-amber-400 font-bold">
              Important: You can only edit the auction details while it is in &quot;Pending&quot; status. Once the auction goes &quot;Active&quot;, editing is fully disabled to ensure bidding integrity.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
