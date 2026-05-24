/**
 * api-feedback.types.ts
 *
 * Shared types for the MazadZone API response normalization and toast feedback system.
 */

/** The four semantic feedback states used across the entire app. */
export type FeedbackType = "success" | "error" | "warning" | "info";

/**
 * Normalised shape for a feedback message derived from a backend response.
 */
export interface ApiFeedbackMessage {
  type: FeedbackType;
  title?: string;
  message: string;
  code?: string;
}

/** Options forwarded to the Sonner toast call. */
export interface AppToastOptions {
  duration?: number;
  action?: {
    label: string;
    onClick: () => void;
  };
}
