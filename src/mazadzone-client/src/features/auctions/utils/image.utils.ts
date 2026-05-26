export function getAuctionImageFallback(
  title: string,
  width: number = 800,
  height: number = 800,
): string {
  return `https://placehold.co/${width}x${height}/F4F1EA/1F2937?text=${encodeURIComponent(title)}`;
}
