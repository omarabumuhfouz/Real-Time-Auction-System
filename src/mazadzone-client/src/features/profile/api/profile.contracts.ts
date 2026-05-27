/**
 * Local contract representations of backend API contracts for the Profile feature.
 */

import type { AddressDto } from "@/features/auth/api/auth.contracts";

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

export interface ChangeEmailRequest {
  newEmail: string;
}

export interface ChangePasswordRequest {
  currentPassword?: string;
  newPassword?: string;
  confirmNewPassword?: string;
}

export interface ProfileSettingsDto {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  nationalId: string;
  city: string;
  street: string;
  building: string;
  landmark: string;
}

