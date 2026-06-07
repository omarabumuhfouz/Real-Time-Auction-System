import { create } from "zustand";

export interface WinDialogData {
  auctionId: string;
  title: string;
  bidAmount: number;
}

interface WinDialogState {
  isOpen: boolean;
  data: WinDialogData | null;
  openWinDialog: (data: WinDialogData) => void;
  closeWinDialog: () => void;
}

export const useWinDialogStore = create<WinDialogState>()((set) => ({
  isOpen: false,
  data: null,
  openWinDialog: (data) => set({ isOpen: true, data }),
  closeWinDialog: () => set({ isOpen: false, data: null }),
}));

/**
 * Reusable utility to trigger the Win Celebration Dialog from a notification object.
 */
export function triggerWinDialogFromNotification(notification: { title: string; message: string; link?: string }) {
  const messageText = notification.message || "";
  const titleText = notification.title || "";
  const linkText = notification.link || "";

  // Parse auction ID from link first, fallback to message/title
  const idRegex = /[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}/;
  const match = linkText.match(idRegex) || messageText.match(idRegex) || titleText.match(idRegex);
  if (!match) return;

  const auctionId = match[0];

  // Parse bid amount (e.g. from "with a bid of ¤4,235,355.00")
  // We match digits, commas, and dots after "with a bid of"
  const bidRegex = /with a bid of [^\d]*([\d,]+(?:\.\d+)?)/;
  const bidMatch = messageText.match(bidRegex);
  const bidAmount = bidMatch ? parseFloat(bidMatch[1].replace(/,/g, '')) : 0;

  // Clean up title
  const title = titleText
    .replace(/^Congratulations!\s*/i, "")
    .replace(/^You won the auction for\s*/i, "")
    .replace(/\.$/, "")
    .trim();

  useWinDialogStore.getState().openWinDialog({
    auctionId,
    title: title || "Auction Item",
    bidAmount,
  });
}
