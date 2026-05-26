import { z } from "zod";

export const creditCardSchema = z.object({
  cardNumber: z
    .string()
    .min(12, "Card number is required")
    .regex(/^\d{4}\s\d{4}\s\d{4}\s\d{4}$/, "Card number must be 16 digits (e.g. 4242 4242 4242 4242)"),

  expiryDate: z
    .string()
    .min(1, "Expiry date is required")
    .regex(/^(0[1-9]|1[0-2])\/([0-9]{2})$/, "Expiry date must be in MM/YY format"),
  cvv: z
    .string()
    .min(3, "CVV is required")
    .regex(/^\d{3,4}$/, "CVV must be 3 or 4 digits"),
});

export type CreditCardFormValues = z.infer<typeof creditCardSchema>;
