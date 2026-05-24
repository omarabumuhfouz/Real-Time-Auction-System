/**
 * api-feedback.utils.ts
 *
 * Utilities for normalising backend API responses and errors into a
 * consistent `ApiFeedbackMessage` shape that can be forwarded to toasts
 * or inline alerts.
 *
 * Design goals:
 *  - Defensive: never throw. Always return a safe fallback.
 *  - Agnostic: works with Axios errors, plain Error objects, and raw response bodies.
 */

import type { ApiFeedbackMessage, FeedbackType } from "./api-feedback.types";

/** Safely checks whether a value is a non-null plain object. */
function isObject(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

/** Safely reads a string field from an unknown object. */
function getString(obj: Record<string, unknown>, key: string): string | undefined {
  const val = obj[key];
  return typeof val === "string" && val.trim() !== "" ? val.trim() : undefined;
}

/**
 * Extracts the first human-readable validation message from a field-errors
 * map, e.g. `{ errors: { bidAmount: ["Bid must be higher than current bid."] } }`.
 */
function extractFirstValidationError(
  errors: Record<string, unknown>
): string | undefined {
  for (const field of Object.keys(errors)) {
    const messages = errors[field];
    if (Array.isArray(messages) && messages.length > 0) {
      const first = messages[0];
      if (typeof first === "string" && first.trim()) {
        return first.trim();
      }
    }
  }
  return undefined;
}

/**
 * Parses a raw error map object into record of validation fields.
 */
function parseErrorsMap(errorsObj: Record<string, unknown>): Record<string, string[]> {
  const result: Record<string, string[]> = {};
  for (const field of Object.keys(errorsObj)) {
    const val = errorsObj[field];
    if (Array.isArray(val)) {
      const strings = val.filter((item): item is string => typeof item === "string");
      if (strings.length > 0) {
        result[field] = strings;
      }
    } else if (typeof val === "string" && val.trim()) {
      result[field] = [val.trim()];
    }
  }
  return result;
}

/**
 * Maps an HTTP status code to a semantic `FeedbackType`.
 */
export function getApiFeedbackTypeFromStatus(status: number): FeedbackType {
  if (status >= 200 && status < 300) return "success";
  if (status === 401 || status === 403) return "error";
  if (status >= 400 && status < 500) return "error";
  return "error";
}

/**
 * Extracts a human-readable message string from an unknown error.
 */
export function getApiMessage(
  error: unknown,
  fallback = "An unexpected error occurred. Please try again."
): string {
  if (isObject(error)) {
    const response = error["response"];
    if (isObject(response)) {
      const data = response["data"];
      if (isObject(data)) {
        const msg = getString(data, "message");
        if (msg) return msg;

        const errors = data["errors"];
        if (isObject(errors)) {
          const firstError = extractFirstValidationError(errors);
          if (firstError) return firstError;
        }
      }
    }

    const msg = getString(error, "message");
    if (msg) return msg;
  }

  if (typeof error === "string" && error.trim()) return error.trim();

  return fallback;
}

/**
 * Determines the `FeedbackType` for an error object.
 */
export function getApiFeedbackType(error: unknown): FeedbackType {
  if (isObject(error)) {
    const response = error["response"];
    if (isObject(response)) {
      const status = response["status"];
      if (typeof status === "number") {
        return getApiFeedbackTypeFromStatus(status);
      }
    }
  }
  return "error";
}

/**
 * Normalises any backend payload or error into a consistent `ApiFeedbackMessage`
 */
export function normalizeApiFeedback(
  payload: unknown,
  fallback = "An unexpected error occurred.",
  defaultType: FeedbackType = "error"
): ApiFeedbackMessage {
  if (isObject(payload)) {
    const message = getString(payload, "message");
    const title = getString(payload, "title");
    const code = getString(payload, "code");
    const rawType = getString(payload, "type");
    const type: FeedbackType =
      rawType === "success" ||
      rawType === "error" ||
      rawType === "warning" ||
      rawType === "info"
        ? rawType
        : defaultType;

    const errors = payload["errors"];
    if (!message && isObject(errors)) {
      const firstError = extractFirstValidationError(errors);
      return {
        type,
        title: title ?? "Validation Error",
        message: firstError ?? fallback,
        code,
      };
    }

    if (message) {
      return { type, title, message, code };
    }

    const response = payload["response"];
    if (isObject(response)) {
      const data = response["data"];
      const status = response["status"];
      const resolvedType =
        typeof status === "number"
          ? getApiFeedbackTypeFromStatus(status)
          : defaultType;

      if (isObject(data)) {
        const dataMessage = getString(data, "message");
        const dataTitle = getString(data, "title");
        const dataErrors = data["errors"];
        if (!dataMessage && isObject(dataErrors)) {
          const firstError = extractFirstValidationError(dataErrors);
          return {
            type: resolvedType,
            title: dataTitle ?? "Validation Error",
            message: firstError ?? fallback,
          };
        }
        if (dataMessage) {
          return {
            type: resolvedType,
            title: dataTitle,
            message: dataMessage,
          };
        }
      }
    }
  }

  if (typeof payload === "string" && payload.trim()) {
    return { type: defaultType, message: payload.trim() };
  }

  if (payload instanceof Error && payload.message.trim()) {
    return { type: defaultType, message: payload.message.trim() };
  }

  return { type: defaultType, message: fallback };
}

/**
 * Extracts all validation field errors from a payload or caught error.
 * Useful for inline field-level validation mappings in form states.
 *
 * Supports raw `{ errors: { field: ["msg"] } }` and Axios responses.
 */
export function extractValidationFieldErrors(
  payload: unknown
): Record<string, string[]> | undefined {
  if (!isObject(payload)) return undefined;

  // Direct errors payload
  const errors = payload["errors"];
  if (isObject(errors)) {
    return parseErrorsMap(errors);
  }

  // Axios wrapped payload
  const response = payload["response"];
  if (isObject(response)) {
    const data = response["data"];
    if (isObject(data)) {
      const dataErrors = data["errors"];
      if (isObject(dataErrors)) {
        return parseErrorsMap(dataErrors);
      }
    }
  }

  return undefined;
}
