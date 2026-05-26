import { z } from "zod";

/**
 * Zod schema for the "Create Account" registration form.
 * Handles client-side validation before API submission.
 */
export const registerSchema = z
  .object({
    fullName: z.string().min(20, "Full Name must be at least 20 characters"),
    email: z.string().email("Please enter a valid email address"),
    password: z.string().min(8, "Password must be at least 8 characters"),
    confirmPassword: z.string().min(8, "Please confirm your password"),
    phoneNumber: z.string().min(10, "Please enter a valid phone number"),
    address: z.string().min(5, "Please enter your full address"),
    nationalId: z.string().min(10, "Please enter a valid national ID"),
    // The national card image is optional
    nationalCardFile: z.any().optional(),
    agreeToTerms: z.boolean().refine((val) => val === true, "You must agree to the Terms and Privacy Policy"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

export type RegisterFormValues = z.infer<typeof registerSchema>;
