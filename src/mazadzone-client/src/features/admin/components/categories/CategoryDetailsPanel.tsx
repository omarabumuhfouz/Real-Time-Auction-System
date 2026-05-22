"use client";

import React from "react";
import { ChevronDown, ChevronUp, Pencil, Plus, Trash2, GripVertical } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Switch } from "@/components/ui/switch";
import { Badge } from "@/components/ui/badge";
import type { Category, Subcategory } from "../../types/category.types";
import { CategoryIcon, getCategoryStyles } from "../../constants/category.constants";

interface CategoryDetailsPanelProps {
  selectedCategory: Category | null;
  isDetailsCollapsed: boolean;
  onToggleCollapse: () => void;
  isUpdatingStatus: boolean;
  onToggleStatus: (category: Category, checked: boolean) => void;
  onEditCategory: (category: Category) => void;
  onAddSubcategory: (categoryId: string) => void;
  onDeleteCategory: (id: string, name: string) => void;
  onEditSubcategory: (categoryId: string, subcategory: Subcategory) => void;
  onDeleteSubcategory: (id: string, parentCategoryId: string, name: string) => void;
}

export function CategoryDetailsPanel({
  selectedCategory,
  isDetailsCollapsed,
  onToggleCollapse,
  isUpdatingStatus,
  onToggleStatus,
  onEditCategory,
  onAddSubcategory,
  onDeleteCategory,
  onEditSubcategory,
  onDeleteSubcategory,
}: CategoryDetailsPanelProps) {
  if (!selectedCategory) {
    return (
      <div className="bg-card border border-border rounded-2xl p-12 text-center text-muted-foreground font-semibold italic shadow-xs">
        Select a category to view and edit its details.
      </div>
    );
  }

  const styles = getCategoryStyles(selectedCategory.iconName || "FolderOpen");

  return (
    <div className="bg-card border border-border rounded-2xl p-6 space-y-6 shadow-xs sticky top-6">
      {/* Sidebar Header Collapse */}
      <div className="flex items-center justify-between border-b border-border pb-4 select-none">
        <h3 className="text-sm font-bold text-muted-foreground uppercase tracking-wider">
          Category Details
        </h3>
        <Button
          variant="ghost"
          size="icon-xs"
          onClick={onToggleCollapse}
          className="text-muted-foreground hover:text-foreground h-6 w-6 rounded-md"
        >
          {isDetailsCollapsed ? <ChevronDown className="h-4 w-4" /> : <ChevronUp className="h-4 w-4" />}
        </Button>
      </div>

      {!isDetailsCollapsed && (
        <>
          {/* Category Card Header Block */}
          <div className="flex items-center gap-3">
            <div
              className={cn(
                "p-3 border rounded-2xl shrink-0 shadow-xs",
                styles.bg,
                styles.text,
                styles.border
              )}
            >
              <CategoryIcon name={selectedCategory.iconName || "FolderOpen"} className="h-6 w-6" />
            </div>
            <div className="flex flex-col min-w-0">
              <span className="font-bold text-lg text-foreground truncate">
                {selectedCategory.name}
              </span>
              <div className="mt-1 flex">
                <Badge
                  className={cn(
                    "font-bold text-[9px] uppercase tracking-wider rounded-md px-1.5 py-0.5 pointer-events-none select-none",
                    selectedCategory.isActive
                      ? "bg-success/10 text-success border border-success/30"
                      : "bg-destructive/10 text-destructive border border-destructive/30"
                  )}
                  variant="outline"
                >
                  {selectedCategory.isActive ? "Active" : "Inactive"}
                </Badge>
              </div>
            </div>
          </div>

          {/* Category Details Text Block */}
          <div className="space-y-4 text-xs font-semibold">
            <div className="flex items-baseline justify-between border-b border-border/40 pb-2">
              <span className="text-muted-foreground">Slug</span>
              <span className="text-foreground font-mono font-medium">{selectedCategory.slug}</span>
            </div>

            <div className="border-b border-border/40 pb-2.5 space-y-1">
              <span className="text-muted-foreground block">Description</span>
              <p className="text-foreground leading-relaxed font-medium">
                {selectedCategory.description || (
                  <span className="italic text-muted-foreground/60 font-semibold">No description provided.</span>
                )}
              </p>
            </div>

            <div className="flex items-baseline justify-between border-b border-border/40 pb-2">
              <span className="text-muted-foreground">Parent Category</span>
              <span className="text-foreground">None</span>
            </div>

            {/* Status switch in sidebar */}
            <div className="flex items-center justify-between py-2 border-b border-border/40">
              <span className="text-muted-foreground">Status</span>
              <div className="flex items-center gap-2">
                <span className="text-xs text-muted-foreground font-semibold">
                  {selectedCategory.isActive ? "Active" : "Inactive"}
                </span>
                <Switch
                  checked={selectedCategory.isActive}
                  disabled={isUpdatingStatus}
                  onCheckedChange={(checked) => onToggleStatus(selectedCategory, checked)}
                />
              </div>
            </div>
          </div>

          {/* Subcategories List Section */}
          <div className="space-y-3">
            <span className="text-xs font-bold text-muted-foreground uppercase tracking-wider block">
              Subcategories
            </span>

            <div className="border border-border rounded-xl divide-y divide-border max-h-[220px] overflow-y-auto bg-muted/10">
              {selectedCategory.subcategories.length === 0 ? (
                <div className="p-4 text-center text-xs text-muted-foreground font-semibold italic">
                  No subcategories added yet.
                </div>
              ) : (
                selectedCategory.subcategories.map((sub) => (
                  <div
                    key={sub.id}
                    className="flex items-center justify-between p-3.5 hover:bg-card/40 transition-colors"
                  >
                    <div className="flex items-center gap-2 min-w-0">
                      <GripVertical className="h-4 w-4 text-muted-foreground/45 shrink-0 select-none cursor-grab" />
                      <span className="text-xs font-semibold text-foreground truncate">
                        {sub.name}
                      </span>
                    </div>
                    <div className="flex items-center gap-1.5 shrink-0 ml-2">
                      <Badge
                        variant="outline"
                        className={cn(
                          "font-bold text-[8px] pointer-events-none select-none px-1 rounded-sm border-transparent",
                          sub.isActive ? "text-success bg-success/5" : "text-muted-foreground bg-muted"
                        )}
                      >
                        {sub.isActive ? "Active" : "Inactive"}
                      </Badge>
                      <Button
                        variant="ghost"
                        size="icon-xs"
                        onClick={() => onEditSubcategory(selectedCategory.id, sub)}
                        title="Edit Subcategory"
                        className="text-muted-foreground hover:text-foreground h-6 w-6 rounded-md"
                      >
                        <Pencil className="h-3 w-3" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="icon-xs"
                        onClick={() => onDeleteSubcategory(sub.id, selectedCategory.id, sub.name)}
                        title="Delete Subcategory"
                        className="text-muted-foreground hover:text-destructive hover:bg-destructive/10 h-6 w-6 rounded-md"
                      >
                        <Trash2 className="h-3 w-3" />
                      </Button>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>

          {/* Panel Actions block at bottom */}
          <div className="grid grid-cols-2 gap-3 pt-2">
            <Button
              variant="outline"
              onClick={() => onEditCategory(selectedCategory)}
              className="font-bold border border-border bg-card text-foreground hover:bg-muted rounded-xl h-10 px-3 cursor-pointer text-xs flex items-center justify-center gap-1"
            >
              <Pencil className="h-3.5 w-3.5" />
              Edit Category
            </Button>
            <Button
              variant="outline"
              onClick={() => onAddSubcategory(selectedCategory.id)}
              className="font-bold border border-border bg-card text-foreground hover:bg-muted rounded-xl h-10 px-3 cursor-pointer text-xs flex items-center justify-center gap-1"
            >
              <Plus className="h-3.5 w-3.5" />
              Add Subcategory
            </Button>
          </div>

          <Button
            variant="outline"
            onClick={() => onDeleteCategory(selectedCategory.id, selectedCategory.name)}
            className="w-full font-bold border border-destructive/25 text-destructive hover:bg-destructive/10 hover:text-destructive rounded-xl h-10 px-4 cursor-pointer text-xs flex items-center justify-center gap-1.5"
          >
            <Trash2 className="h-4 w-4" />
            Delete Category
          </Button>
        </>
      )}
    </div>
  );
}
