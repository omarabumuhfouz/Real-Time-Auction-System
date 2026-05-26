/**
 * Pure mapping functions for the Profile feature.
 * Decouples raw backend DTO models from presentation ViewModels.
 */

import type { UserProfile, Address } from "../types/profile.types";
import type { BidderProfileDto } from "./profile.contracts";

/**
 * Maps a backend BidderProfileDto to a presentation UserProfile ViewModel.
 */
export function mapBidderProfileToUserProfile(dto: BidderProfileDto): UserProfile {
  return {
    id: dto.id,
    fullName: dto.fullName,
    email: dto.email,
    phoneNumber: dto.phoneNumber,
    nationalId: dto.nationalId,
    avatarUrl: undefined,
    avatarInitial: dto.fullName ? dto.fullName.charAt(0).toUpperCase() : "U",
  };
}

/**
 * Maps a backend bidder profile's primary address to the standard Address ViewModel.
 */
export function mapBidderProfileToDefaultAddress(dto: BidderProfileDto): Address {
  const addressDto = dto.address;
  return {
    id: "primary",
    title: "Primary Address",
    streetAddress: addressDto?.street || "Street",
    building: addressDto?.building || "1",
    landmark: addressDto?.landmark || undefined,
    city: addressDto?.city || "Amman",
    isDefault: true,
  };
}
