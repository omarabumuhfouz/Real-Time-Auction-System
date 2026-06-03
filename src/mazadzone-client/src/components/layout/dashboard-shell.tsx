import type { ReactNode } from "react";
import { cn } from "@/lib/utils";

export interface DashboardShellProps {
  children: ReactNode;
  className?: string;
}

export function DashboardShell({
  children,
  className,
}: DashboardShellProps) {
  return (
    <div
      className={cn(
        "max-w-[1600px] mx-auto w-full px-4 md:px-6 py-6 md:py-8 space-y-6",
        className,
      )}
    >
      {children}
    </div>
  );
}
