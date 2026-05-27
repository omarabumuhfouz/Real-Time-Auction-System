import { redirect } from "next/navigation";
import { ROUTES } from "@/config/routes.config";

/**
 * Redirect /seller directly to /seller/auctions dashboard.
 */
export default function Page() {
  redirect(ROUTES.SELLER.AUCTIONS);
}
