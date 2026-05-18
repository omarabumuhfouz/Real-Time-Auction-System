import * as React from "react"

import { cn } from "@/lib/utils"

/**
 * ActivityList Component
 * 
 * A semantic wrapper component that manages the container layout for a collection
 * of activity list items (e.g., bids list, orders list). It provides a standard, responsive vertical stack spacing.
 * 
 * @example
 * <ActivityList>
 *   <ActivityListItem>...</ActivityListItem>
 * </ActivityList>
 */
const ActivityList = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={cn("flex flex-col gap-4", className)}
    {...props}
  />
))
ActivityList.displayName = "ActivityList"

export { ActivityList }
