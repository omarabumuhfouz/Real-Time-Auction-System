/**
 * Category-specific TypeScript types and interfaces.
 */

export interface Subcategory {
  id: string;
  name: string;
  slug: string;
  description?: string;
  isActive: boolean;
  parentCategoryId: string;
  auctionsCount: number;
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
  iconName?: string; // name of the Lucide icon, like "Monitor", "Shirt", "Home", "Image", "Bike", "Carfront", "Gem", "Music"
  isActive: boolean;
  auctionsCount: number;
  subcategories: Subcategory[];
}

export interface CreateCategoryInput {
  name: string;
  slug: string;
  description?: string;
  iconName?: string;
  isActive: boolean;
}

export interface UpdateCategoryInput {
  name?: string;
  slug?: string;
  description?: string;
  iconName?: string;
  isActive?: boolean;
}

export interface CreateSubcategoryInput {
  name: string;
  slug: string;
  description?: string;
  isActive: boolean;
  parentCategoryId: string;
}

export interface UpdateSubcategoryInput {
  name?: string;
  slug?: string;
  description?: string;
  isActive?: boolean;
}
