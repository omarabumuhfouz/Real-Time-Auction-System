import { Bell } from "lucide-react";
import { EmptyState } from "@/components/feedback/empty-state";

export const EmptyNotifications = () => {
  return (
    <EmptyState
      title="No notifications yet"
      description="You'll see updates here"
      icon={Bell}
    />
  );
};
