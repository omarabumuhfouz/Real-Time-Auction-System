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
  const formData = new FormData();
  formData.append("email", request.email);
  formData.append("password", request.password);
  formData.append("phoneNumber", request.phoneNumber);
  formData.append("nationalId", request.nationalId);
  formData.append("firstName", request.firstName);
  formData.append("secondName", request.secondName);
  formData.append("thirdName", request.thirdName);
  formData.append("lastName", request.lastName);
  formData.append("city", request.address.city);
  formData.append("street", request.address.street);
  formData.append("building", request.address.building);
  formData.append("landmark", request.address.landmark);

  // Note: We are purposely NOT sending a file/image here for testing.

  const response = await api.post<RegisterBidderDto>(
    "/api/v1/bidders/register",
    formData,
    {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    },
  );
  return response.data;
}
