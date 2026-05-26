import { Metadata } from "next";
import { AdminDisputesPage } from "@/features/disputes";

export const metadata: Metadata = {
  title: "Resolve Disputes | MazadZone Admin",
  description: "Review and manage dispute cases across orders and auctions.",
};

export default function Page() {
  return <AdminDisputesPage />;
}
