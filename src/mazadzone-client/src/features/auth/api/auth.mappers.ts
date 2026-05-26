/**
 * Pure mapping functions for the Auth feature.
 * Decouples raw backend DTO models from presentation ViewModels and session state.
 */

import type { AuthUser } from "@/stores/auth.store";
import type { RegisterFormValues } from "../validations/register.schema";
import type { RegisterBidderRequest } from "./auth.contracts";

/**
 * Splits a full name string into separate first, second, third, and last names
 * required by the backend bidder registration schema.
 */
export function mapRegisterFormToRequest(
  form: RegisterFormValues,
): RegisterBidderRequest {
  const names = form.fullName.trim().split(/\s+/);
  const firstName = names[0] || "";
  const secondName = names[1] || "";
  const thirdName = names[2] || "";
  // All remaining names are grouped into the lastName slot
  const lastName = names.slice(3).join(" ") || "";

  const addressParts = form.address.trim().split(/\s+/);
  const city = addressParts[0] || "Amman";
  const street = addressParts[1] || "";
  const building = addressParts[2] || "1";
  const landmark = addressParts.slice(3).join(" ") || "";

  return {
    email: form.email,
    password: form.password,
    phoneNumber: form.phoneNumber,
    nationalId: form.nationalId,
    firstName,
    secondName,
    thirdName,
    lastName,
    address: {
      city,
      street,
      building,
      landmark,
    },
  };
}

/**
 * Parses a JWT token payload to extract standard claims for the user's active session.
 */
export function decodeJwtToken(token: string): AuthUser {
  try {
    const payload = token.split(".")[1];
    if (!payload) throw new Error("Invalid JWT token structure");

    const decoded = JSON.parse(atob(payload)) as {
      sub?: string;
      email?: string;
      name?: string;
      role?: string;
    };

    return {
      id: decoded.sub || "unknown-id",
      email: decoded.email || "",
      fullName: decoded.name || "User",
      role: (decoded.role?.toLowerCase() as any) || "bidder",
    };
  } catch (error) {
    console.error("Failed to decode JWT token payload:", error);
    return {
      id: "unknown-id",
      email: "",
      fullName: "User",
      role: "bidder",
    };
  }
}
