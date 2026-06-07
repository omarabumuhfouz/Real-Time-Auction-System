import type { FormFieldConfig, UserProfile, AddressFieldConfig } from "../types/profile.types";

/**
 * Returns the field configurations for the Account Information form.
 * Handles dynamic properties like `disabled` state, placeholders from the API, and read-only values.
 */
export const getAccountInfoFields = (
  isPending: boolean,
  profile: UserProfile
): FormFieldConfig[] => [
  {
    id: "profilePhone",
    label: "Phone Number",
    placeholder: profile.phoneNumber || "Enter phone number",
    name: "phoneNumber",
    readOnly: true,
  },
  {
    id: "profileNationalId",
    label: "National ID",
    value: profile.nationalId ?? "1234567890",
    disabled: true,
    hint: "National ID cannot be changed",
  },
  {
    id: "profileEmail",
    label: "Email Address",
    placeholder: profile.email || "Enter email address",
    name: "email",
    disabled: isPending,
  },
];

/**
 * Returns the field configurations for the Address creation/editing form.
 */
export const getAddressFields = (isPending: boolean): AddressFieldConfig[] => [
  {
    id: "title",
    label: "Address Title (e.g. Home, Work)",
    placeholder: "Enter address title",
    name: "title",
    disabled: isPending,
  },
  {
    id: "streetAddress",
    label: "Street Address",
    placeholder: "Street name",
    name: "streetAddress",
    disabled: isPending,
  },
  {
    id: "building",
    label: "Building",
    placeholder: "Building no.",
    name: "building",
    disabled: isPending,
  },
  {
    id: "landmark",
    label: "Landmark",
    placeholder: "Near mall, next to mosque...",
    name: "landmark",
    disabled: isPending,
  },
  {
    id: "city",
    label: "City",
    placeholder: "City",
    name: "city",
    disabled: isPending,
  },
];
