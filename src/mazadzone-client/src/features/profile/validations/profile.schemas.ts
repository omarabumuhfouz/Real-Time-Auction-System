import { z } from "zod";

export const profileSchema = z.object({
  fullName: z.string().min(3, "Full name must be at least 3 characters"),
  email: z.string().email("Invalid email address"),
  phoneNumber: z.string().min(9, "Phone number must be at least 9 characters"),
});

export const addressSchema = z.object({
  title: z.string().min(2, "Title must be at least 2 characters (e.g. Home, Work)"),
  streetAddress: z.string().min(3, "Street address must be at least 3 characters"),
  building: z.string().min(1, "Building number/details are required"),
  landmark: z.string().optional().or(z.literal("")),
  city: z.string().min(2, "City name is required"),
  isDefault: z.boolean(),
});

export const changePasswordSchema = z
  .object({
    currentPassword: z.string().min(6, "Current password must be at least 6 characters"),
    newPassword: z.string().min(6, "New password must be at least 6 characters"),
    confirmNewPassword: z.string().min(6, "Confirm password must be at least 6 characters"),
  })
  .refine((data) => data.newPassword === data.confirmNewPassword, {
    message: "New passwords do not match",
    path: ["confirmNewPassword"],
  });

export interface ProfileFormValues {
  fullName: string;
  email: string;
  phoneNumber: string;
}

export type ChangePasswordFormValues = z.infer<typeof changePasswordSchema>;

export interface AddressFormValues {
  title: string;
  streetAddress: string;
  building: string;
  landmark?: string;
  city: string;
  isDefault: boolean;
}
