/**
 * Local contract representations of backend API contracts for the Auth feature.
 *
 * These types match the ASP.NET Core OpenAPI contract exactly.
 */

export interface AddressDto {
  city: string;
  street: string;
  building: string;
  landmark: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface TokenDto {
  token: string;
  refreshToken: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface LogoutRequest {
  refreshToken: string;
  isLogoutFromAllDevices: boolean;
}

export interface RegisterBidderRequest {
  email: string;
  password: string;
  phoneNumber: string;
  nationalId: string;
  firstName: string;
  secondName: string;
  thirdName: string;
  lastName: string;
  city: string;
  street: string;
  building: string;
  landmark: string;
  file: File;
}

export interface BidderProfileDto {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  address: AddressDto;
  totalBidsPlaced: number;
  reliabilityScore: number;
  nationalId: string;
  memberSince: string;
}

export interface RegisterBidderDto {
  profileInfo: BidderProfileDto;
  tokenInfo: TokenDto;
}
