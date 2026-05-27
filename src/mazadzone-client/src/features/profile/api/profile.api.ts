import { api } from "@/lib/api/client";
import type { UserProfile, Address } from "../types/profile.types";
import type { BidderProfileDto, ChangePasswordRequest, ProfileSettingsDto } from "./profile.contracts";
import { mapBidderProfileToUserProfile, mapBidderProfileToDefaultAddress } from "./profile.mappers";
import { useAuthStore } from "@/stores/auth.store";

const ADDRESS_STORAGE_KEY = "mazadzone_profile_addresses";

// Helper to fetch/save local addresses from localStorage
function getLocalAddresses(): Address[] {
  if (typeof window === "undefined") return [];
  const stored = localStorage.getItem(ADDRESS_STORAGE_KEY);
  return stored ? JSON.parse(stored) : [];
}

function saveLocalAddresses(addresses: Address[]): void {
  if (typeof window === "undefined") return;
  localStorage.setItem(ADDRESS_STORAGE_KEY, JSON.stringify(addresses));
}

/**
 * Fetches the user profile details from the ASP.NET Core backend.
 */
export async function fetchUserProfile(): Promise<UserProfile> {
  const userId = useAuthStore.getState().user?.id;
  if (!userId) {
    throw new Error("User is not authenticated");
  }

  const response = await api.get<BidderProfileDto>(`/api/v1/bidders/${userId}`);

  // Store a cached copy of the profile email in localStorage to check for updates
  if (typeof window !== "undefined") {
    localStorage.setItem("mazadzone_last_fetched_email", response.data.email);
  }

  return mapBidderProfileToUserProfile(response.data);
}

/**
 * Updates the user profile details.
 * Triggers backend credential updates for email changes.
 */
export async function updateUserProfile(input: Partial<UserProfile>): Promise<UserProfile> {
  const userId = useAuthStore.getState().user?.id;
  if (!userId) {
    throw new Error("User is not authenticated");
  }

  // Check if email has changed compared to last fetched email
  const lastEmail = typeof window !== "undefined" ? localStorage.getItem("mazadzone_last_fetched_email") : null;
  if (input.email && lastEmail && input.email !== lastEmail) {
    // Fire real backend ChangeEmailRequest
    await api.put("/api/v1/users/email", {
      newEmail: input.email,
    });

    if (typeof window !== "undefined") {
      localStorage.setItem("mazadzone_last_fetched_email", input.email);
    }
  }

  // For general fields (fullName, phoneNumber, etc.) we simulate successful save
  // and update the global store's active user session instantly.
  const activeUser = useAuthStore.getState().user;
  if (activeUser) {
    const updatedUser = {
      ...activeUser,
      fullName: input.fullName || activeUser.fullName,
      email: input.email || activeUser.email,
    };
    useAuthStore.getState().setUser(updatedUser);
  }

  return {
    id: userId,
    fullName: input.fullName || "User",
    email: input.email || "",
    phoneNumber: input.phoneNumber,
    avatarUrl: input.avatarUrl,
  };
}

/**
 * Fetches the user's address book list, combining primary backend address with local storage.
 */
export async function fetchAddresses(): Promise<Address[]> {
  const userId = useAuthStore.getState().user?.id;
  if (!userId) return [];

  let primaryAddress: Address | null = null;
  try {
    const response = await api.get<BidderProfileDto>(`/api/v1/bidders/${userId}`);
    primaryAddress = mapBidderProfileToDefaultAddress(response.data);
  } catch (error) {
    console.error("Failed to load primary address from backend profile:", error);
  }

  const localAddresses = getLocalAddresses();

  if (primaryAddress) {
    // Exclude any duplicate primary address from local list
    const filteredLocal = localAddresses.filter((a) => a.id !== "primary");
    return [primaryAddress, ...filteredLocal];
  }

  return localAddresses;
}

/**
 * Adds a new address to the address book.
 */
export async function createAddress(address: Omit<Address, "id">): Promise<Address> {
  const newAddress: Address = {
    ...address,
    id: Math.random().toString(36).substr(2, 9),
    isDefault: false, // Local storage addresses are non-default secondary locations
  };

  const current = getLocalAddresses();
  saveLocalAddresses([...current, newAddress]);

  return newAddress;
}

/**
 * Updates an existing address in the address book.
 */
export async function updateAddress(id: string, updates: Partial<Address>): Promise<Address> {
  if (id === "primary") {
    // Primary address is locked to backend data, so we bypass modifying it locally
    return {
      id: "primary",
      title: "Primary Address",
      streetAddress: updates.streetAddress || "Street",
      building: updates.building || "1",
      landmark: updates.landmark,
      city: updates.city || "Amman",
      isDefault: true,
    };
  }

  const current = getLocalAddresses();
  let updatedAddress: Address | null = null;

  const modified = current.map((addr) => {
    if (addr.id === id) {
      updatedAddress = { ...addr, ...updates };
      return updatedAddress;
    }
    return addr;
  });

  if (!updatedAddress) {
    throw new Error(`Address with ID ${id} not found.`);
  }

  saveLocalAddresses(modified);
  return updatedAddress;
}

/**
 * Deletes an address from the address book.
 */
export async function removeAddress(id: string): Promise<void> {
  if (id === "primary") return; // Primary address cannot be deleted

  const current = getLocalAddresses();
  const filtered = current.filter((addr) => addr.id !== id);
  saveLocalAddresses(filtered);
}

/**
 * Changes the user's password in the ASP.NET Core backend.
 */
export async function changePassword(input: ChangePasswordRequest): Promise<void> {
  await api.put("/api/v1/users/password", input);
}

/**
 * Fetches the user profile settings from the ASP.NET Core backend.
 */
export async function fetchProfileSettings(userId: string): Promise<ProfileSettingsDto> {
  if (!userId) {
    throw new Error("User ID is required to fetch profile settings");
  }
  const response = await api.get<ProfileSettingsDto>(`/api/v1/users/users/${userId}/profile-settings`);
  return response.data;
}

