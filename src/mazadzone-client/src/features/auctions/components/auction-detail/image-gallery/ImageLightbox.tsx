"use client";

import { useState, useEffect, useCallback } from "react";
import Image from "next/image";
import { X, ChevronLeft, ChevronRight } from "lucide-react";
import { ThumbnailStrip } from "./ThumbnailStrip";

export interface ImageLightboxProps {
  images: string[];
  title: string;
  initialIndex: number;
  onClose: () => void;
}

/**
 * Full-screen lightbox modal for viewing auction images without cropping.
 *
 * Features:
 * - object-contain main image (no cuts)
 * - Prev/Next arrow navigation
 * - ThumbnailStrip rail for direct image jump
 * - Keyboard support: ← → to navigate, Esc to close
 * - Click outside (backdrop) to close
 */
export function ImageLightbox({ images, title, initialIndex, onClose }: ImageLightboxProps) {
  const [activeIndex, setActiveIndex] = useState(initialIndex);

  const goNext = useCallback(() => {
    setActiveIndex((prev) => (prev + 1) % images.length);
  }, [images.length]);

  const goPrev = useCallback(() => {
    setActiveIndex((prev) => (prev - 1 + images.length) % images.length);
  }, [images.length]);

  useEffect(() => {
    const handleKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
      if (e.key === "ArrowRight") goNext();
      if (e.key === "ArrowLeft") goPrev();
    };

    document.addEventListener("keydown", handleKey);
    document.body.style.overflow = "hidden";

    return () => {
      document.removeEventListener("keydown", handleKey);
      document.body.style.overflow = "";
    };
  }, [onClose, goNext, goPrev]);

  return (
    <div
      className="fixed inset-0 z-50 flex flex-col items-center justify-center bg-black/92 backdrop-blur-sm"
      onClick={onClose}
      aria-modal="true"
      role="dialog"
      aria-label={`Image viewer — ${title}`}
    >
      {/* Inner content — stops propagation so clicking it doesn't close the modal */}
      <div
        className="relative flex flex-col items-center gap-4 w-full max-w-6xl px-4"
        onClick={(e) => e.stopPropagation()}
      >
        {/* ── Top Bar ─────────────────────────────────── */}
        <div className="flex w-full items-center justify-between">
          <span className="text-sm font-semibold text-white/60 tracking-widest uppercase">
            {activeIndex + 1} / {images.length}
          </span>
          <button
            type="button"
            onClick={onClose}
            aria-label="Close image viewer"
            className="flex h-9 w-9 items-center justify-center rounded-full bg-white/10 text-white hover:bg-white/20 transition-colors"
          >
            <X className="size-5" />
          </button>
        </div>

        {/* ── Main Image ──────────────────────────────── */}
        <div className="relative w-full" style={{ height: "70vh" }}>
          {images.length > 1 && (
            <button
              type="button"
              onClick={goPrev}
              aria-label="Previous image"
              className="absolute left-3 top-1/2 z-10 -translate-y-1/2 flex h-10 w-10 items-center justify-center rounded-full bg-black/50 text-white hover:bg-black/70 transition-colors"
            >
              <ChevronLeft className="size-6" />
            </button>
          )}

          <Image
            key={images[activeIndex]}
            src={images[activeIndex]}
            alt={`${title} — image ${activeIndex + 1}`}
            fill
            sizes="(max-width: 1280px) 95vw, 1200px"
            className="rounded-xl object-contain animate-in fade-in duration-300"
            priority
          />

          {images.length > 1 && (
            <button
              type="button"
              onClick={goNext}
              aria-label="Next image"
              className="absolute right-3 top-1/2 z-10 -translate-y-1/2 flex h-10 w-10 items-center justify-center rounded-full bg-black/50 text-white hover:bg-black/70 transition-colors"
            >
              <ChevronRight className="size-6" />
            </button>
          )}
        </div>

        {/* ── Thumbnail Rail ──────────────────────────── */}
        {images.length > 1 && (
          <ThumbnailStrip
            images={images}
            title={title}
            activeIndex={activeIndex}
            onSelect={setActiveIndex}
            orientation="horizontal"
          />
        )}
      </div>
    </div>
  );
}
