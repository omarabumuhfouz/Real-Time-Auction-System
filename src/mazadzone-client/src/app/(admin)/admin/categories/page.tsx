import React, { Suspense } from "react";
import { ManageCategoriesPage } from "@/features/admin";

export const metadata = {
  title: "Manage Categories - Admin Dashboard",
  description: "Create, edit, organize, and manage auction categories and subcategories.",
};

export default function CategoriesPage() {
  return (
    <Suspense
      fallback={
        <div className="p-8 text-center text-muted-foreground font-semibold">
          Loading categories administration...
        </div>
      }
    >
      <ManageCategoriesPage />
    </Suspense>
  );
}
