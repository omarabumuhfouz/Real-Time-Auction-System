"use client";

import { useState, useCallback } from "react";

const NATIONAL_ID_REGEX = /\b\d{14}\b/;
const NATIONAL_ID_LOOSE_REGEX = /\d[\d\s\-]{12,16}\d/;

function cleanOcrNumber(raw: string): string {
  return raw.replace(/\D/g, "");
}

export type OcrPhase = "idle" | "scanning";

export interface UseNationalIdOcrReturn {
  ocrPhase: OcrPhase;
  /** Runs OCR on the given image file and returns the detected 14-digit ID, or null if not found. */
  scanFile: (file: File) => Promise<string | null>;
}

/**
 * useNationalIdOcr
 *
 * Exposes an imperative `scanFile()` function intended to be called inside
 * the form's onSubmit handler — so OCR and the API request happen in a single flow.
 * Does NOT auto-trigger on file selection.
 */
export function useNationalIdOcr(): UseNationalIdOcrReturn {
  const [ocrPhase, setOcrPhase] = useState<OcrPhase>("idle");

  const scanFile = useCallback(async (file: File): Promise<string | null> => {
    if (!file.type.startsWith("image/")) return null;

    setOcrPhase("scanning");

    try {
      const Tesseract = await import("tesseract.js");

      const result = await Tesseract.recognize(file, "ara+eng", {
        logger: () => {},
      });

      const text = result.data.text;

      // 1. Strict 14-digit match
      let match = text.match(NATIONAL_ID_REGEX);

      // 2. Loose fallback (digits with separators)
      if (!match) {
        const looseMatch = text.match(NATIONAL_ID_LOOSE_REGEX);
        if (looseMatch) {
          const cleaned = cleanOcrNumber(looseMatch[0]);
          if (cleaned.length === 14) match = [cleaned];
        }
      }

      return match ? cleanOcrNumber(match[0]) : null;
    } catch (err) {
      console.error("[useNationalIdOcr] OCR failed:", err);
      return null;
    } finally {
      setOcrPhase("idle");
    }
  }, []);

  return { ocrPhase, scanFile };
}
