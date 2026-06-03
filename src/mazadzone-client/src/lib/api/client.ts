import axios, {
  type AxiosError,
  type AxiosInstance,
  type InternalAxiosRequestConfig,
  type AxiosRequestConfig,
} from "axios";
import type { ApiError, ApiResponse, HttpValidationProblemDetails } from "@/types/api.types";
import { env } from "@/config/env";

// --- Create Axios Instance --------------------------------------

const apiClient: AxiosInstance = axios.create({
  baseURL: env.NEXT_PUBLIC_API_BASE_URL,
  timeout: 15_000,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

// --- Request Interceptor ----------------------------------------
// Injects the Authorization header if a token exists.
// We import lazily from the auth store to avoid circular dependencies.

apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Dynamic import avoids circular dependency between api client ↔ auth store
    // eslint-disable-next-line @typescript-eslint/no-require-imports
    const { useAuthStore } = require("@/stores/auth.store") as {
      useAuthStore: { getState: () => { accessToken: string | null } };
    };

    const token = useAuthStore.getState().accessToken;

    // Do not include authorization header for register or login endpoints to prevent preflight CORS issues
    const isExcluded =
      config.url &&
      (config.url.endsWith("/api/v1/bidders/register") ||
        config.url.endsWith("/api/v1/auth/login"));

    if (token && config.headers && !isExcluded) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error: AxiosError) => Promise.reject(error),
);

// --- Response Interceptor ---------------------------------------
// Normalizes error responses into a consistent ApiError shape.

apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<HttpValidationProblemDetails>) => {
    const data = error.response?.data;
    const apiError: ApiError = {
      message:
        data?.detail ??
        data?.title ??
        error.message ??
        "An unexpected error occurred",
      statusCode: error.response?.status ?? 500,
      errors: data?.errors,
      ...(process.env.NODE_ENV === "development" && {
        originalError: error,
      }),
    };

    // TODO: Handle 401 — trigger token refresh or redirect to login
    if (apiError.statusCode === 401) {
      // Future: call token refresh logic here
    }

    return Promise.reject(apiError);
  },
);

// --- Typed Helper Methods ---------------------------------------

// --- URL Formatting Helper --------------------------------------
const formatUrl = (url: string): string => {
  if (url.startsWith("http://") || url.startsWith("https://")) {
    return url;
  }
  // If already prefixed with /api/v1, leave as is
  if (url.startsWith("/api/v1")) {
    return url;
  }
  if (url.startsWith("api/v1")) {
    return `/${url}`;
  }
  
  // Format as /api/v1/endpoint
  const cleanUrl = url.startsWith("/") ? url.slice(1) : url;
  return `/api/v1/${cleanUrl}`;
};

export const api = {
  get: <T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> =>
    apiClient.get<T>(formatUrl(url), config).then((r) => ({
      data: r.data,
      success: true,
      message: "",
      timestamp: new Date().toISOString(),
    })),

  post: <T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> =>
    apiClient.post<T>(formatUrl(url), data, config).then((r) => ({
      data: r.data,
      success: true,
      message: "",
      timestamp: new Date().toISOString(),
    })),

  put: <T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> =>
    apiClient.put<T>(formatUrl(url), data, config).then((r) => ({
      data: r.data,
      success: true,
      message: "",
      timestamp: new Date().toISOString(),
    })),

  patch: <T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> =>
    apiClient.patch<T>(formatUrl(url), data, config).then((r) => ({
      data: r.data,
      success: true,
      message: "",
      timestamp: new Date().toISOString(),
    })),

  delete: <T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> =>
    apiClient.delete<T>(formatUrl(url), config).then((r) => ({
      data: r.data,
      success: true,
      message: "",
      timestamp: new Date().toISOString(),
    })),
};

export { apiClient };
