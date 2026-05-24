import { Spinner } from "@/components/ui/spinner";

/**
 * Root-level loading state.
 * Shown by Next.js while a route segment is loading.
 */
export default function Loading() {
  return (
    <div className="flex flex-1 items-center justify-center py-16">
      <Spinner size="lg" />
    </div>
  );
}
