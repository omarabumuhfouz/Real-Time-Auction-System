import { APP_CONFIG } from "@/config/app.config";

/**
 * Currency formatting utilities.
 * Pure functions — no side effects, no external dependencies.
 */

/**
 * Formats a number as currency using the app's configured currency.
 *
 * @example formatCurrency(1234.5) // "$1,234.50"
 * @example formatCurrency(1234.5, { compact: true }) // "$1.2K"
 */
export function formatCurrency(
  amount: number,
  options?: { compact?: boolean },
): string {
  const { symbol, locale } = APP_CONFIG.currency;

  const formatted = new Intl.NumberFormat(locale, {
    style: "decimal",
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
    ...(options?.compact && {
      notation: "compact",
      maximumFractionDigits: 1,
    }),
  }).format(amount);

  return `${formatted} ${symbol}`;
}

/**
 * Parses a currency string back to a number.
 * Strips non-numeric characters except the decimal point.
 *
 * @example parseCurrencyInput("$1,234.50") // 1234.5
 */
export function parseCurrencyInput(value: string): number {
  const cleaned = value.replace(/[^0-9.]/g, "");
  return parseFloat(cleaned) || 0;
}
