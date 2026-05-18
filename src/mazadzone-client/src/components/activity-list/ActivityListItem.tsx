import * as React from "react"

import { cn } from "@/lib/utils"

/**
 * ActivityListItem Component
 * 
 * A structural container for individual rows inside an ActivityList. Handles
 * styling consistency across rows, including padding, background colors, borders,
 * shadows, and hover transition effects. Also ensures standard responsive flex directions (stacked on mobile, row on desktop).
 * 
 * @example
 * <ActivityListItem className="hover:border-primary">
 *   <div>Details</div>
 * </ActivityListItem>
 */
const ActivityListItem = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={cn(
      "flex flex-col md:flex-row items-center justify-between p-4 md:p-6 bg-white border border-gray-100 rounded-2xl shadow-sm hover:shadow-md transition-all gap-4",
      className
    )}
    {...props}
  />
))
ActivityListItem.displayName = "ActivityListItem"

export { ActivityListItem }
