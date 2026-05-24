"use client";

/**
 * app-alert.tsx
 *
 * Reusable inline alert component for MazadZone.
 *
 * Wraps shadcn `Alert` with MazadZone design-system tokens so that
 * success, info, and warning states look consistent alongside the
 * existing `destructive` variant.
 */

import type { ReactNode } from "react";
import {
  CheckCircle2,
  AlertCircle,
  TriangleAlert,
  Info,
} from "lucide-react";
import { cn } from "@/lib/utils";
import {
  Alert,
  AlertTitle,
  AlertDescription,
  AlertAction,
} from "@/components/ui/alert";
import type { FeedbackType } from "@/lib/api-feedback/api-feedback.types";

export interface AppAlertProps {

  type: FeedbackType;
  title?: string;
  message?: string;
  className?: string;
  action?: ReactNode;
}

const TYPE_CONFIG: Record<
  FeedbackType,
  { icon: typeof CheckCircle2; className: string }
> = {
  success: {
    icon: CheckCircle2,
    className:
      "bg-success border-success/30 text-success-foreground *:[svg]:text-success-foreground",
  },
  error: {
    icon: AlertCircle,
    className:
      "bg-destructive/8 border-destructive/20 text-destructive *:[svg]:text-destructive",
  },
  warning: {
    icon: TriangleAlert,
    className:
      "bg-warning border-warning/30 text-warning-foreground *:[svg]:text-warning-foreground",
  },
  info: {
    icon: Info,
    className:
      "bg-info border-info/30 text-info-foreground *:[svg]:text-info-foreground",
  },
};

/**
 * Inline feedback alert using design-system semantic tokens.
 */
export function AppAlert({
  type,
  title,
  message,
  className,
  action,
}: AppAlertProps) {
  if (!title && !message) return null;

  const { icon: Icon, className: typeClassName } = TYPE_CONFIG[type];

  return (
    <Alert
      className={cn(
        "rounded-xl border py-3 px-4",
        typeClassName,
        className
      )}
      role="alert"
    >
      <Icon className="size-4 shrink-0" aria-hidden="true" />

      {title && (
        <AlertTitle className="font-semibold text-sm leading-tight">
          {title}
        </AlertTitle>
      )}

      {message && (
        <AlertDescription
          className={cn(
            "text-sm leading-snug",
            !title && "text-current opacity-90"
          )}
        >
          {message}
        </AlertDescription>
      )}

      {action && <AlertAction>{action}</AlertAction>}
    </Alert>
  );
}
