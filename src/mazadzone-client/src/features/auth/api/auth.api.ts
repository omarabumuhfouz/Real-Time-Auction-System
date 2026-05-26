/**
 * Pure HTTP REST functions for the Auth feature.
 * Connects directly to the ASP.NET Core endpoints.
 */

import { api } from "@/lib/api/client";
import type {
  LoginRequest,
  LogoutRequest,
  RefreshTokenRequest,
  RegisterBidderRequest,
  RegisterBidderDto,
  TokenDto,
} from "./auth.contracts";

/**
 * Authenticate user and obtain access and refresh tokens.
 */
export async function login(request: LoginRequest): Promise<TokenDto> {
  const response = await api.post<TokenDto>("/api/v1/auth/login", request);
  return response.data;
}

/**
 * Refresh an expired access token using a valid refresh token.
 */
export async function refreshToken(
  request: RefreshTokenRequest,
): Promise<TokenDto> {
  const response = await api.post<TokenDto>("/api/v1/auth/refresh", request);
  return response.data;
}

/**
 * Invalidate user session and revoke refresh tokens on logout.
 */
export async function logout(request: LogoutRequest): Promise<void> {
  await api.post<void>("/api/v1/auth/logout", request);
}

/**
 * Register a new bidder profile on the platform.
 */
export async function registerBidder(
  request: RegisterBidderRequest,
): Promise<RegisterBidderDto> {
  const response = await api.post<RegisterBidderDto>(
    "/api/v1/bidders/register",
    request,
  );
  return response.data;
}
