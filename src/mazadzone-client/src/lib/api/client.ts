import axios, {
  type AxiosError,
  type AxiosInstance,
  type InternalAxiosRequestConfig,
  type AxiosRequestConfig,
} from "axios";
import type { ApiError, ApiResponse, HttpValidationProblemDetails } from "@/types/api.types";
import type { AuthUser } from "@/stores/auth.store";
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

// --- Token Refresh Helper ---------------------------------------

let refreshPromise: Promise<string> | null = null;

async function handleTokenRefresh(): Promise<string> {
  if (refreshPromise) {
    return refreshPromise;
  }

  refreshPromise = (async () => {
    try {
      // Dynamic imports to avoid circular dependency
      // eslint-disable-next-line @typescript-eslint/no-require-imports
      const { getRefreshToken } = require("@/lib/auth/token") as {
        getRefreshToken: () => string | null;
      };

      const refreshTok = getRefreshToken();
      if (!refreshTok) {
        throw new Error("No refresh token available");
      }

      // eslint-disable-next-line @typescript-eslint/no-require-imports
      const { decodeJwtToken } = require("@/features/auth/api/auth.mappers") as {
        decodeJwtToken: (token: string) => AuthUser;
      };

      // eslint-disable-next-line @typescript-eslint/no-require-imports
      const { useAuthStore } = require("@/stores/auth.store") as {
        useAuthStore: {
          getState: () => {
            login: (user: AuthUser, token: string, refreshToken: string) => void;
            logout: () => void;
          };
        };
      };

      // Make a direct request using raw axios to bypass the apiClient interceptors
      const response = await axios.post<{ token: string; refreshToken: string }>(
        `${env.NEXT_PUBLIC_API_BASE_URL}/api/v1/auth/refresh`,
        { refreshToken: refreshTok },
        {
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
          },
        }
      );

      const { token, refreshToken } = response.data;
      const decodedUser = decodeJwtToken(token);

      // Update the store and localStorage
      useAuthStore.getState().login(decodedUser, token, refreshToken);

      return token;
    } catch (error) {
      // If refresh fails, log the user out and redirect to login page
      // eslint-disable-next-line @typescript-eslint/no-require-imports
      const { useAuthStore } = require("@/stores/auth.store") as {
        useAuthStore: {
          getState: () => {
            logout: () => void;
          };
        };
      };

      useAuthStore.getState().logout();

      if (typeof window !== "undefined") {
        window.location.href = "/login";
      }

      throw error;
    } finally {
      refreshPromise = null;
    }
  })();

  return refreshPromise;
}

// --- Request Interceptor ----------------------------------------
// Injects the Authorization header if a token exists and handles pre-emptive refresh.

apiClient.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    // Dynamic import avoids circular dependency between api client ↔ auth store
    // eslint-disable-next-line @typescript-eslint/no-require-imports
    const { useAuthStore } = require("@/stores/auth.store") as {
      useAuthStore: { getState: () => { accessToken: string | null } };
    };

    let token = useAuthStore.getState().accessToken;

    // Do not include authorization header or perform refresh check for excluded endpoints
    const isExcluded =
      config.url &&
      (config.url.endsWith("/api/v1/bidders/register") ||
        config.url.endsWith("/api/v1/auth/login") ||
        config.url.endsWith("/api/v1/auth/refresh"));

    if (token && !isExcluded) {
      // eslint-disable-next-line @typescript-eslint/no-require-imports
      const { isTokenExpired } = require("@/lib/auth/token") as {
        isTokenExpired: (token: string) => boolean;
      };

      if (isTokenExpired(token)) {
        try {
          token = await handleTokenRefresh();
        } catch (error) {
          // If pre-emptive refresh fails, reject the request early
          return Promise.reject(error);
        }
      }

      if (config.headers) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    }

    return config;
  },
  (error: AxiosError) => Promise.reject(error),
);

// --- Response Interceptor ---------------------------------------
// Normalizes error responses into a consistent ApiError shape and handles reactive token refresh for 401 status.

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<HttpValidationProblemDetails>) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };
    const statusCode = error.response?.status ?? 500;

    // Do not attempt token refresh for excluded endpoints or if already retried
    const isExcluded =
      originalRequest?.url &&
      (originalRequest.url.endsWith("/api/v1/bidders/register") ||
        originalRequest.url.endsWith("/api/v1/auth/login") ||
        originalRequest.url.endsWith("/api/v1/auth/refresh"));

    if (statusCode === 401 && originalRequest && !originalRequest._retry && !isExcluded) {
      // Only attempt refresh if we actually have a refresh token stored
      // eslint-disable-next-line @typescript-eslint/no-require-imports
      const { getRefreshToken } = require("@/lib/auth/token") as {
        getRefreshToken: () => string | null;
      };

      if (getRefreshToken()) {
        originalRequest._retry = true;
        try {
          const newToken = await handleTokenRefresh();
          if (originalRequest.headers) {
            originalRequest.headers.Authorization = `Bearer ${newToken}`;
          }
          return apiClient(originalRequest);
        } catch (refreshError) {
          return Promise.reject(refreshError);
        }
      }
    }

    const data = error.response?.data;
    const isServerError = statusCode >= 500;
    const apiError: ApiError = {
      message:
        (isServerError
          ? data?.title
          : data?.detail ?? data?.title) ??
        error.message ??
        "An unexpected error occurred",
      statusCode,
      errors: data?.errors,
      ...(process.env.NODE_ENV === "development" && {
        originalError: error,
      }),
    };

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
