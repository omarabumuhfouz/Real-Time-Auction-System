import React, { Suspense } from "react";
import { ModerateUsersPage } from "@/features/admin";

export const metadata = {
  title: "Moderate Users - Admin Dashboard",
  description: "Review and manage user accounts and account statuses.",
};

export default function UsersPage() {
  return (
    <Suspense fallback={<div className="p-8 text-center text-muted-foreground">Loading users administration...</div>}>
      <ModerateUsersPage />
    </Suspense>
  );
}
