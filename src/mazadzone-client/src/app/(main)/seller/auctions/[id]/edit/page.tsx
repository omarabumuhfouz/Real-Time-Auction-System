import { EditAuctionPage } from "@/features/auctions";

interface PageProps {
  params: Promise<{ id: string }> | { id: string };
}

/**
 * Seller Edit Auction Route wrapper.
 * Thin wrapper that forwards the dynamic ID to the modular feature component.
 */
export default async function Page({ params }: PageProps) {
  // Resolve params asynchronously to support modern Next.js environments cleanly
  const resolvedParams = await params;
  const { id } = resolvedParams;

  return <EditAuctionPage id={id} />;
}
