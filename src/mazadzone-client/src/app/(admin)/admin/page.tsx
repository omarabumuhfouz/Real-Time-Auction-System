import { AdminDashboardPage } from "@/features/admin";
import { createPageMetadata } from "@/components/seo/metadata";

export const metadata = createPageMetadata({
  title: "Admin Dashboard Overview",
  description: "MazadZone administrative overview, moderation queues, and metrics control panel.",
  path: "/admin",
});

export default function Page() {
  return <AdminDashboardPage />;
}
