import { AlertCircle } from "lucide-react";
import { cn } from "@/lib/utils";

export interface FormErrorMessageProps {
  message?: string;
  className?: string;
}

export function FormErrorMessage({ message, className }: FormErrorMessageProps) {
  if (!message) return null;

  return (
    <p
      className={cn(
        "flex items-center gap-1.5 text-xs font-semibold text-destructive mt-1.5 animate-slide-in",
        className
      )}
    >
      <AlertCircle className="h-3.5 w-3.5 shrink-0" aria-hidden="true" />
      <span>{message}</span>
    </p>
  );
}
