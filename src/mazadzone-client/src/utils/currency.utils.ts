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

/**
 * Formats a standard float string to dot-separated thousands and decimal, forcing the decimal part to be .00 (e.g., "111111111.50" -> "111.111.111.00")
 */
export function formatPriceOnBlur(val: string): string {
  if (!val) return "";
  
  // Clean all non-digit and non-dot characters
  const cleanVal = val.replace(/[^0-9.]/g, "");
  
  let [integerPart] = cleanVal.split(".");
  
  // Format integer part with dots every 3 digits
  integerPart = integerPart.replace(/\D/g, "");
  if (!integerPart) integerPart = "0";
  const formattedInteger = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ".");
  
  return `${formattedInteger}.00`;
}

/**
 * Strips the thousand-separator dots so the user can edit the value cleanly, forcing .00 decimal part (e.g., "111.111.111.00" -> "111111111.00")
 */
export function unformatPriceOnFocus(val: string): string {
  if (!val) return "";
  const parts = val.split(".");
  if (parts.length <= 1) return `${val.replace(/\D/g, "")}.00`;
  
  const integerPart = parts.slice(0, -1).join("");
  return `${integerPart}.00`;
}

/**
 * Parses a dot-formatted price (thousands dot, decimals dot) back to a number.
 * Assumes the format ends with a 2-digit decimal part.
 */
export function parseDotFormattedPrice(formattedStr: string): number {
  if (!formattedStr) return 0;
  const cleanDigits = formattedStr.replace(/\./g, "");
  return parseFloat(cleanDigits) / 100;
}
