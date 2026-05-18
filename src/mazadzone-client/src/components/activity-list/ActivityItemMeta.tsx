import * as React from "react"
import Link from "next/link"
import { cn } from "@/lib/utils"

interface ActivityItemMetaProps extends Omit<React.HTMLAttributes<HTMLDivElement>, "title"> {
  title: React.ReactNode
  titleHref?: string
  subtitle?: React.ReactNode
}

/**
 * ActivityItemMeta Component
 * 
 * A structured details section that presents textual metadata alongside the preview image.
 * Renders an optional title (with an optional router link) and custom metadata subtitles beneath it.
 * 
 * @param title - The main textual title/name of the item (can be a React Node).
 * @param titleHref - Optional navigation URL to turn the title header into a router Link.
 * @param subtitle - Additional details, dates, prices or metadata lines displayed below the title.
 */
const ActivityItemMeta = React.forwardRef<HTMLDivElement, ActivityItemMetaProps>(
  ({ className, title, titleHref, subtitle, ...props }, ref) => {
    return (
      <div ref={ref} className={cn("flex flex-col", className)} {...props}>
        {titleHref ? (
          <Link
            href={titleHref}
            className="hover:text-primary transition-colors text-lg md:text-xl font-semibold text-gray-900 line-clamp-1 mb-2 block"
          >
            {title}
          </Link>
        ) : (
          <h3 className="text-lg md:text-xl font-semibold text-gray-900 line-clamp-1 mb-2">
            {title}
          </h3>
        )}
        {subtitle && (
          <div className="flex items-center gap-4 text-sm md:text-base text-gray-500">
            {subtitle}
          </div>
        )}
      </div>
    )
  }
)
ActivityItemMeta.displayName = "ActivityItemMeta"

export { ActivityItemMeta }
