import { z } from "zod";

const MAX_EVIDENCE_FILES = 5;
const MAX_EVIDENCE_FILE_SIZE_BYTES = 10 * 1024 * 1024;
const ALLOWED_EVIDENCE_MIME_TYPES = ["image/jpeg", "image/png"] as const;

const fileSchema = z.custom<File>(
  (value) => {
    if (typeof File === "undefined") {
      return true;
    }

    return value instanceof File;
  },
  {
    message: "Invalid evidence file",
  },
);

export const disputeSchema = z.object({
  disputeTypeId: z.string().min(1, "Please select a dispute type"),
  title: z
    .string()
    .min(5, "Title must be at least 5 characters")
    .max(100, "Title cannot exceed 100 characters"),
  description: z
    .string()
    .min(10, "Description must be at least 10 characters")
    .max(1000, "Description cannot exceed 1000 characters"),
  evidenceFiles: z
    .array(fileSchema)
    .max(MAX_EVIDENCE_FILES, `You can upload up to ${MAX_EVIDENCE_FILES} evidence files`)
    .refine(
      (files) => files.every((file) => file.size <= MAX_EVIDENCE_FILE_SIZE_BYTES),
      `Each evidence file must be 10MB or smaller`,
    )
    .refine(
      (files) =>
        files.every(
          (file) =>
            ALLOWED_EVIDENCE_MIME_TYPES.includes(
              file.type as (typeof ALLOWED_EVIDENCE_MIME_TYPES)[number],
            ),
        ),
      "Only PNG and JPG images are allowed",
    ),
});

export type DisputeFormValues = z.infer<typeof disputeSchema>;
