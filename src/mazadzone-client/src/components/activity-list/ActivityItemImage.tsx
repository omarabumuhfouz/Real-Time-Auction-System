import * as React from "react"
import Image from "next/image"
import Link from "next/link"
import { cn } from "@/lib/utils"

interface ActivityItemImageProps extends React.HTMLAttributes<HTMLDivElement> {
  src: string
  alt: string
  href?: string
  imageClassName?: string
}

/**
 * ActivityItemImage Component
 * 
 * Renders a consistent, styled container for preview images within an activity list row.
 * Handles responsive dimensions, border styling, overlays, and features optional Next.js Link routing.
 * 
 * @param src - The absolute or relative path to the image source.
 * @param alt - A semantic text description of the image for screen readers and accessibility.
 * @param href - Optional navigation URL. If provided, wraps the image in a clickable Next.js Link.
 * @param imageClassName - Custom Tailwind utility classes applied directly to the HTML Image element.
 */
const ActivityItemImage = React.forwardRef<HTMLDivElement, ActivityItemImageProps>(
  ({ className, src, alt, href, imageClassName, ...props }, ref) => {
    const imageContent = (
      <div
        ref={ref}
        className={cn(
          "relative w-20 h-20 md:w-24 md:h-24 rounded-xl overflow-hidden shrink-0 border border-gray-100",
          className
        )}
        {...props}
      >
        <Image
          src={src}
          alt={alt}
          fill
          className={cn("object-cover", imageClassName)}
        />
      </div>
    )

    if (href) {
      return (
        <Link
          href={href}
          className="shrink-0 hover:opacity-90 transition-opacity cursor-pointer block"
        >
          {imageContent}
        </Link>
      )
    }

    return <div className="shrink-0">{imageContent}</div>
  }
)
ActivityItemImage.displayName = "ActivityItemImage"

export { ActivityItemImage }
