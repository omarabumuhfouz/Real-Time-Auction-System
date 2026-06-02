"use client";

import React, { useState } from "react";
import { Plus, Loader2, AlertTriangle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Category, Subcategory } from "../../types/category.types";
import {
  useGetCategories,
  useUpdateCategory,
  useDeleteCategory,
  useDeleteSubcategory,
} from "../../api";
import { CategoryFormModal } from "./CategoryFormModal";
import { SubcategoryFormModal } from "./SubcategoryFormModal";
import { CategoriesTable } from "./CategoriesTable";
import { CategoryDetailsPanel } from "./CategoryDetailsPanel";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";

export function ManageCategoriesPage() {
  const { data: categories = [], isLoading, isError, refetch } = useGetCategories();
  const updateCategoryMutation = useUpdateCategory();
  const deleteCategoryMutation = useDeleteCategory();
  const deleteSubcategoryMutation = useDeleteSubcategory();

  // Selected Category (defaults to first one when categories load)
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null);
  const [isDetailsCollapsed, setIsDetailsCollapsed] = useState(false);

  // Form Modals State
  const [categoryModal, setCategoryModal] = useState<{ isOpen: boolean; data: Category | null }>({
    isOpen: false,
    data: null,
  });
  const [subcategoryModal, setSubcategoryModal] = useState<{
    isOpen: boolean;
    parentCategoryId: string | null;
    data: Subcategory | null;
  }>({
    isOpen: false,
    parentCategoryId: null,
    data: null,
  });

  // Deletion Confirmation Dialog States
  const [deleteCatConfirm, setDeleteCatConfirm] = useState<{ isOpen: boolean; id: string | null; name: string }>({
    isOpen: false,
    id: null,
    name: "",
  });
  const [deleteSubConfirm, setDeleteSubConfirm] = useState<{
    isOpen: boolean;
    id: string | null;
    parentCategoryId: string | null;
    name: string;
  }>({
    isOpen: false,
    id: null,
    parentCategoryId: null,
    name: "",
  });

  const activeCategoryId = selectedCategoryId ?? categories[0]?.id ?? null;
  const selectedCategory = categories.find((c) => c.id === activeCategoryId) || null;

  // Toggle Category Status (Active/Inactive) via switch
  const handleToggleCategoryStatus = async (category: Category, checked: boolean) => {
    try {
      await updateCategoryMutation.mutateAsync({
        id: category.id,
        data: { isActive: checked },
      });
    } catch (err) {
      console.error("Failed to toggle category status:", err);
    }
  };

  // Perform category deletion
  const handleDeleteCategory = async () => {
    if (!deleteCatConfirm.id) return;
    try {
      await deleteCategoryMutation.mutateAsync(deleteCatConfirm.id);
      
      // If deleted category was selected, select next available
      if (activeCategoryId === deleteCatConfirm.id) {
        const remaining = categories.filter((c) => c.id !== deleteCatConfirm.id);
        setSelectedCategoryId(remaining.length > 0 ? remaining[0].id : null);
      }
      setDeleteCatConfirm({ isOpen: false, id: null, name: "" });
    } catch (err) {
      console.error("Failed to delete category:", err);
    }
  };

  // Perform subcategory deletion
  const handleDeleteSubcategory = async () => {
    if (!deleteSubConfirm.id || !deleteSubConfirm.parentCategoryId) return;
    try {
      await deleteSubcategoryMutation.mutateAsync({
        id: deleteSubConfirm.id,
        parentCategoryId: deleteSubConfirm.parentCategoryId,
      });
      setDeleteSubConfirm({ isOpen: false, id: null, parentCategoryId: null, name: "" });
    } catch (err) {
      console.error("Failed to delete subcategory:", err);
    }
  };

  if (isLoading) {
    return (
      <div className="p-8 flex flex-col items-center justify-center min-h-[60vh] gap-3">
        <Loader2 className="h-8 w-8 text-primary animate-spin" />
        <p className="text-sm text-muted-foreground font-semibold">Loading auction categories...</p>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="p-8 flex flex-col items-center justify-center min-h-[60vh] gap-4">
        <AlertTriangle className="h-10 w-10 text-destructive animate-bounce" />
        <div className="text-center">
          <h3 className="text-lg font-bold text-foreground">Failed to Load Categories</h3>
          <p className="text-sm text-muted-foreground mt-1 max-w-md">
            An error occurred while communicating with the server. Please try again.
          </p>
        </div>
        <Button onClick={() => refetch()} variant="outline">
          Retry Fetching
        </Button>
      </div>
    );
  }

  return (
    <div className="p-4 md:p-8 space-y-6">
      {/* Page Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div className="space-y-1">
          <h1 className="text-3xl md:text-4xl font-bold tracking-tight text-foreground">
            Manage Categories
          </h1>
          <p className="text-sm text-muted-foreground font-medium">
            Create, edit, organize, and manage auction categories and subcategories.
          </p>
        </div>
        <Button
          onClick={() => setCategoryModal({ isOpen: true, data: null })}
          className="bg-primary text-primary-foreground hover:bg-primary/90 font-bold shrink-0 self-start md:self-center h-10 px-4 rounded-xl cursor-pointer shadow-sm flex items-center gap-2"
        >
          <Plus className="h-5 w-5" />
          Add Category
        </Button>
      </div>

      {/* Two Column Layout: Main table (left) + Category details panel (right) */}
      <div className="grid grid-cols-1 xl:grid-cols-3 gap-6 items-start">
        {/* Table View Component */}
        <CategoriesTable
          categories={categories}
          activeCategoryId={activeCategoryId}
          onSelectCategory={(id) => setSelectedCategoryId(id)}
          onEditCategory={(cat) => setCategoryModal({ isOpen: true, data: cat })}
          onAddSubcategory={(categoryId) =>
            setSubcategoryModal({ isOpen: true, parentCategoryId: categoryId, data: null })
          }
          onDeleteCategory={(id, name) => setDeleteCatConfirm({ isOpen: true, id, name })}
        />

        {/* Right Sidebar: Category Details Panel */}
        <div className="xl:col-span-1">
          <CategoryDetailsPanel
            selectedCategory={selectedCategory}
            isDetailsCollapsed={isDetailsCollapsed}
            onToggleCollapse={() => setIsDetailsCollapsed(!isDetailsCollapsed)}
            isUpdatingStatus={updateCategoryMutation.isPending}
            onToggleStatus={handleToggleCategoryStatus}
            onEditCategory={(cat) => setCategoryModal({ isOpen: true, data: cat })}
            onAddSubcategory={(categoryId) =>
              setSubcategoryModal({ isOpen: true, parentCategoryId: categoryId, data: null })
            }
            onDeleteCategory={(id, name) => setDeleteCatConfirm({ isOpen: true, id, name })}
            onEditSubcategory={(categoryId, sub) =>
              setSubcategoryModal({ isOpen: true, parentCategoryId: categoryId, data: sub })
            }
            onDeleteSubcategory={(id, parentCategoryId, name) =>
              setDeleteSubConfirm({ isOpen: true, id, parentCategoryId, name })
            }
          />
        </div>
      </div>

      {/* Categories Modification Dialog Modal */}
      <CategoryFormModal
        isOpen={categoryModal.isOpen}
        onClose={() => setCategoryModal({ isOpen: false, data: null })}
        category={categoryModal.data}
      />

      {/* Subcategories Modification Dialog Modal */}
      {subcategoryModal.parentCategoryId && (
        <SubcategoryFormModal
          isOpen={subcategoryModal.isOpen}
          onClose={() => setSubcategoryModal({ isOpen: false, parentCategoryId: null, data: null })}
          parentCategoryId={subcategoryModal.parentCategoryId}
          subcategory={subcategoryModal.data}
        />
      )}

      {/* Category Deletion Confirmation Dialog */}
      <Dialog
        open={deleteCatConfirm.isOpen}
        onOpenChange={(open) => !open && setDeleteCatConfirm({ isOpen: false, id: null, name: "" })}
      >
        <DialogContent className="max-w-[420px] p-6 bg-card border border-border sm:rounded-2xl gap-0">
          <DialogHeader className="text-left mb-4">
            <DialogTitle className="text-lg font-bold text-foreground flex items-center gap-2">
              <AlertTriangle className="h-5 w-5 text-destructive shrink-0" />
              Delete Category
            </DialogTitle>
            <DialogDescription className="text-xs text-muted-foreground mt-2 leading-relaxed font-semibold">
              Are you sure you want to delete the category{" "}
              <span className="font-bold text-foreground">&quot;{deleteCatConfirm.name}&quot;</span>? This
              action is permanent and will delete all associated subcategories.
            </DialogDescription>
          </DialogHeader>
          <div className="flex items-center gap-3 mt-4">
            <Button
              onClick={() => setDeleteCatConfirm({ isOpen: false, id: null, name: "" })}
              variant="outline"
              disabled={deleteCategoryMutation.isPending}
              className="flex-1 font-bold px-4 py-2.5 h-11 text-xs cursor-pointer rounded-xl bg-card border border-border text-foreground hover:bg-muted"
            >
              Cancel
            </Button>
            <Button
              onClick={handleDeleteCategory}
              disabled={deleteCategoryMutation.isPending}
              className="flex-1 font-bold px-4 py-2.5 h-11 text-xs cursor-pointer rounded-xl text-white flex items-center justify-center gap-1.5 bg-destructive hover:bg-destructive/95 text-destructive-foreground border-transparent"
            >
              {deleteCategoryMutation.isPending && <Loader2 className="size-4 animate-spin" />}
              Confirm Delete
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Subcategory Deletion Confirmation Dialog */}
      <Dialog
        open={deleteSubConfirm.isOpen}
        onOpenChange={(open) =>
          !open && setDeleteSubConfirm({ isOpen: false, id: null, parentCategoryId: null, name: "" })
        }
      >
        <DialogContent className="max-w-[420px] p-6 bg-card border border-border sm:rounded-2xl gap-0">
          <DialogHeader className="text-left mb-4">
            <DialogTitle className="text-lg font-bold text-foreground flex items-center gap-2">
              <AlertTriangle className="h-5 w-5 text-destructive shrink-0" />
              Delete Subcategory
            </DialogTitle>
            <DialogDescription className="text-xs text-muted-foreground mt-2 leading-relaxed font-semibold">
              Are you sure you want to delete the subcategory{" "}
              <span className="font-bold text-foreground">&quot;{deleteSubConfirm.name}&quot;</span>? This
              action is permanent.
            </DialogDescription>
          </DialogHeader>
          <div className="flex items-center gap-3 mt-4">
            <Button
              onClick={() =>
                setDeleteSubConfirm({ isOpen: false, id: null, parentCategoryId: null, name: "" })
              }
              variant="outline"
              disabled={deleteSubcategoryMutation.isPending}
              className="flex-1 font-bold px-4 py-2.5 h-11 text-xs cursor-pointer rounded-xl bg-card border border-border text-foreground hover:bg-muted"
            >
              Cancel
            </Button>
            <Button
              onClick={handleDeleteSubcategory}
              disabled={deleteSubcategoryMutation.isPending}
              className="flex-1 font-bold px-4 py-2.5 h-11 text-xs cursor-pointer rounded-xl text-white flex items-center justify-center gap-1.5 bg-destructive hover:bg-destructive/95 text-destructive-foreground border-transparent"
            >
              {deleteSubcategoryMutation.isPending && <Loader2 className="size-4 animate-spin" />}
              Confirm Delete
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}
