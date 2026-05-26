import { toast } from "sonner";
import type { AppToastOptions, FeedbackType, ApiFeedbackMessage } from "../api-feedback/api-feedback.types";
import { normalizeApiFeedback } from "../api-feedback/api-feedback.utils";

function buildSonnerOpts(options?: AppToastOptions) {
  return {
    duration: options?.duration,
    action: options?.action
      ? {
          label: options.action.label,
          onClick: options.action.onClick,
        }
      : undefined,
  };
}

/**
 * Pure TypeScript object that wraps Sonner's toast API.
 * Can be called anywhere in the app (even outside React rendering scopes,
 * e.g., in raw API interceptors or services).
 */
export const appToast = {
  success(title: string, message?: string, options?: AppToastOptions) {
    toast.success(title, {
      description: message,
      ...buildSonnerOpts(options),
    });
  },

  error(title: string, message?: string, options?: AppToastOptions) {
    toast.error(title, {
      description: message,
      ...buildSonnerOpts(options),
    });
  },

  warning(title: string, message?: string, options?: AppToastOptions) {
    toast.warning(title, {
      description: message,
      ...buildSonnerOpts(options),
    });
  },

  info(title: string, message?: string, options?: AppToastOptions) {
    toast.info(title, {
      description: message,
      ...buildSonnerOpts(options),
    });
  },

  show(
    type: FeedbackType,
    title: string,
    message?: string,
    options?: AppToastOptions
  ) {
    switch (type) {
      case "success":
        toast.success(title, { description: message, ...buildSonnerOpts(options) });
        break;
      case "warning":
        toast.warning(title, { description: message, ...buildSonnerOpts(options) });
        break;
      case "info":
        toast.info(title, { description: message, ...buildSonnerOpts(options) });
        break;
      case "error":
      default:
        toast.error(title, { description: message, ...buildSonnerOpts(options) });
        break;
    }
  },

  fromApiFeedback(
    feedback: ApiFeedbackMessage,
    options?: AppToastOptions
  ) {
    this.show(
      feedback.type,
      feedback.title ?? (feedback.type === "success" ? "Success" : "Error"),
      feedback.message,
      options
    );
  },

  fromApiError(
    error: unknown,
    fallbackMessage = "An unexpected error occurred. Please try again.",
    options?: AppToastOptions
  ) {
    const feedback = normalizeApiFeedback(error, fallbackMessage, "error");
    this.fromApiFeedback(feedback, options);
  },

  fromApiResponse(
    response: unknown,
    defaultType: FeedbackType = "success",
    options?: AppToastOptions
  ) {
    const feedback = normalizeApiFeedback(response, "Operation completed.", defaultType);
    this.fromApiFeedback(feedback, options);
  },
};

/**
 * Backward-compatibility hook that simply returns the global appToast wrapper.
 * Keeps existing components that call useAppToast() functioning without changes.
 */
export function useAppToast() {
  return appToast;
}
