import * as React from "react"

import { cn } from "@/lib/utils"

/**
 * ActivityItemActions Component
 * 
 * A layout container specifically designed to align and justify call-to-action buttons (CTAs) 
 * consistently at the trailing end of an activity list row. Handles standard alignment on desktop 
 * and full-width layout constraints on mobile viewports.
 */
const ActivityItemActions = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={cn("flex justify-end w-full md:w-auto shrink-0", className)}
    {...props}
  />
))
ActivityItemActions.displayName = "ActivityItemActions"

export { ActivityItemActions }
