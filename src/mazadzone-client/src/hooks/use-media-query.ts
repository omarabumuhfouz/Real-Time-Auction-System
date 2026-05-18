import { useSyncExternalStore } from "react";

/**
 * Subscribes to a CSS media query and returns whether it currently matches.
 *
 * SSR-safe: returns `false` during server rendering and updates on mount.
 *
 * @example
 * ```tsx
 * const isMobile = useMediaQuery("(max-width: 768px)");
 * const prefersReducedMotion = useMediaQuery("(prefers-reduced-motion: reduce)");
 * ```
 */
export function useMediaQuery(query: string): boolean {
  return useSyncExternalStore(
    (callback) => {
      const mediaQuery = window.matchMedia(query);
      mediaQuery.addEventListener("change", callback);
      return () => mediaQuery.removeEventListener("change", callback);
    },
    () => window.matchMedia(query).matches,
    () => false // SSR fallback
  );
}

