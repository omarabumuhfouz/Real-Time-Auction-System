import { AlertCircle, RotateCcw } from "lucide-react";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

export interface SectionErrorProps {
  title?: string;
  description?: string;
  onRetry?: () => void;
  className?: string;
}

export function SectionError({
  title = "Failed to load content",
  description = "A connection error occurred while retrieving data from the server.",
  onRetry,
  className,
}: SectionErrorProps) {
  return (
    <div
      className={cn(
        "flex flex-col items-center justify-center text-center p-8 rounded-2xl border border-destructive/20 bg-destructive/5 backdrop-blur-sm space-y-4 max-w-lg mx-auto my-6 animate-fade-in",
        className
      )}
      role="alert"
    >
      <div className="flex h-12 w-12 items-center justify-center rounded-2xl bg-destructive/10 text-destructive ring-1 ring-destructive/20 shadow-sm">
        <AlertCircle className="h-6 w-6" aria-hidden="true" />
      </div>
      
      <div className="space-y-1.5">
        <h3 className="text-lg font-bold text-foreground tracking-tight">{title}</h3>
        {description && (
          <p className="text-sm text-muted-foreground leading-relaxed max-w-sm mx-auto">
            {description}
          </p>
        )}
      </div>

      {onRetry && (
        <Button
          onClick={onRetry}
          variant="outline"
          size="sm"
          className="mt-2 cursor-pointer font-semibold gap-2 border-destructive/20 hover:bg-destructive/5 hover:text-destructive"
        >
          <RotateCcw className="h-3.5 w-3.5" />
          <span>Try Again</span>
        </Button>
      )}
    </div>
  );
}
