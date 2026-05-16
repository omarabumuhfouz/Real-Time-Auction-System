"use client";

import Image from "next/image";
import { cn } from "@/lib/utils";

export interface ThumbnailStripProps {
  images: string[];
  title: string;
  activeIndex: number;
  onSelect: (index: number) => void;
  /** "vertical" for the gallery sidebar; "horizontal" for the lightbox rail */
  orientation?: "vertical" | "horizontal";
}

/**
 * Shared thumbnail navigation strip.
 * Used in both the main AuctionImageGallery sidebar and the ImageLightbox rail.
 */
export function ThumbnailStrip({
  images,
  title,
  activeIndex,
  onSelect,
  orientation = "vertical",
}: ThumbnailStripProps) {
  return (
    <div
      className={cn(
        "p-1.5 scrollbar-thin scrollbar-thumb-primary/20 hover:scrollbar-thumb-primary/40",
        orientation === "vertical"
          ? "flex flex-row md:flex-col gap-3 max-h-[639px] overflow-x-auto md:overflow-y-auto"
          : "flex flex-row gap-3 overflow-x-auto max-w-full",
      )}
    >
      {images.map((src, index) => (
        <button
          key={index}
          type="button"
          onClick={() => onSelect(index)}
          aria-label={`View image ${index + 1} of ${images.length}`}
          aria-pressed={activeIndex === index}
          className={cn(
            "relative shrink-0 overflow-hidden cursor-pointer rounded-lg border-2 transition-all duration-300",
            orientation === "vertical" ? "h-16 w-16 md:h-20 md:w-20" : "h-14 w-14",
            activeIndex === index
              ? "border-primary ring-2 ring-primary/20 shadow-lg scale-105"
              : "border-transparent bg-muted/50 hover:border-primary/30 opacity-60 hover:opacity-100 hover:scale-105",
          )}
        >
          <Image
            src={src}
            alt={`${title} — thumbnail ${index + 1}`}
            fill
            sizes="80px"
            className="object-cover"
          />
          {activeIndex === index && (
            <div className="absolute inset-0 bg-primary/5 pointer-events-none" />
          )}
        </button>
      ))}
    </div>
  );
}
