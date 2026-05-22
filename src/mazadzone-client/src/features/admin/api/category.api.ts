import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import type {
  Category,
  Subcategory,
  CreateCategoryInput,
  UpdateCategoryInput,
  CreateSubcategoryInput,
  UpdateSubcategoryInput,
} from "../types/category.types";

// --- persistent mock categories database in-memory ---
let mockCategories: Category[] = [
  {
    id: "cat-1",
    name: "Tech and Electronics",
    slug: "tech-electronics",
    description: "Devices, gadgets, and consumer electronics.",
    iconName: "Monitor",
    isActive: true,
    auctionsCount: 1248,
    subcategories: [
      { id: "sub-1-1", name: "Phones", slug: "phones", isActive: true, parentCategoryId: "cat-1", auctionsCount: 654 },
      { id: "sub-1-2", name: "Laptops", slug: "laptops", isActive: true, parentCategoryId: "cat-1", auctionsCount: 412 },
      { id: "sub-1-3", name: "Cameras", slug: "cameras", isActive: true, parentCategoryId: "cat-1", auctionsCount: 120 },
      { id: "sub-1-4", name: "Accessories", slug: "accessories", isActive: true, parentCategoryId: "cat-1", auctionsCount: 62 },
    ],
  },
  {
    id: "cat-2",
    name: "Fashion and Style",
    slug: "fashion-style",
    description: "Trendy apparel, accessories, and shoes.",
    iconName: "Shirt",
    isActive: true,
    auctionsCount: 932,
    subcategories: [
      { id: "sub-2-1", name: "Men", slug: "men", isActive: true, parentCategoryId: "cat-2", auctionsCount: 450 },
      { id: "sub-2-2", name: "Women", slug: "women", isActive: true, parentCategoryId: "cat-2", auctionsCount: 382 },
      { id: "sub-2-3", name: "Shoes", slug: "shoes", isActive: true, parentCategoryId: "cat-2", auctionsCount: 100 },
    ],
  },
  {
    id: "cat-3",
    name: "Home and Living",
    slug: "home-living",
    description: "Household items, decorations, and utensils.",
    iconName: "Home",
    isActive: true,
    auctionsCount: 876,
    subcategories: [
      { id: "sub-3-1", name: "Furniture", slug: "furniture", isActive: true, parentCategoryId: "cat-3", auctionsCount: 420 },
      { id: "sub-3-2", name: "Kitchen", slug: "kitchen", isActive: true, parentCategoryId: "cat-3", auctionsCount: 310 },
      { id: "sub-3-3", name: "Decor", slug: "decor", isActive: true, parentCategoryId: "cat-3", auctionsCount: 146 },
    ],
  },
  {
    id: "cat-4",
    name: "Collectibles and Art",
    slug: "collectibles-art",
    description: "Rare coins, postage stamps, paintings, and drawings.",
    iconName: "Image",
    isActive: true,
    auctionsCount: 654,
    subcategories: [
      { id: "sub-4-1", name: "Coins", slug: "coins", isActive: true, parentCategoryId: "cat-4", auctionsCount: 312 },
      { id: "sub-4-2", name: "Stamps", slug: "stamps", isActive: true, parentCategoryId: "cat-4", auctionsCount: 142 },
      { id: "sub-4-3", name: "Art", slug: "art", isActive: true, parentCategoryId: "cat-4", auctionsCount: 200 },
    ],
  },
  {
    id: "cat-5",
    name: "Hobbies and Leisure",
    slug: "hobbies-leisure",
    description: "Equipment for hobbies, physical activities, and gaming.",
    iconName: "Bike",
    isActive: true,
    auctionsCount: 532,
    subcategories: [
      { id: "sub-5-1", name: "Sports", slug: "sports", isActive: true, parentCategoryId: "cat-5", auctionsCount: 210 },
      { id: "sub-5-2", name: "Outdoor", slug: "outdoor", isActive: true, parentCategoryId: "cat-5", auctionsCount: 182 },
      { id: "sub-5-3", name: "Gaming", slug: "gaming", isActive: true, parentCategoryId: "cat-5", auctionsCount: 140 },
    ],
  },
  {
    id: "cat-6",
    name: "Motors",
    slug: "motors",
    description: "Vehicles, spare parts, and engine components.",
    iconName: "Car",
    isActive: true,
    auctionsCount: 1023,
    subcategories: [
      { id: "sub-6-1", name: "Cars", slug: "cars", isActive: true, parentCategoryId: "cat-6", auctionsCount: 610 },
      { id: "sub-6-2", name: "Motorcycles", slug: "motorcycles", isActive: true, parentCategoryId: "cat-6", auctionsCount: 290 },
      { id: "sub-6-3", name: "Parts", slug: "parts", isActive: true, parentCategoryId: "cat-6", auctionsCount: 123 },
    ],
  },
];

// --- Hook to get all Categories ---
export function useGetCategories() {
  return useQuery<Category[]>({
    queryKey: ["admin", "categories"],
    queryFn: async () => {
      try {
        const response = await api.get<Category[]>("/admin/categories");
        return response.data;
      } catch (error) {
        console.warn("Failed to fetch admin categories from backend, falling back to mock data:", error);
        return [...mockCategories];
      }
    },
    staleTime: 5 * 60 * 1000,
  });
}

// --- Hook to get Category detail by ID ---
export function useGetCategoryDetail(id: string) {
  return useQuery<Category>({
    queryKey: ["admin", "categories", id],
    queryFn: async () => {
      try {
        const response = await api.get<Category>(`/admin/categories/${id}`);
        return response.data;
      } catch (error) {
        console.warn(`Failed to fetch category detail for ID ${id}, falling back to mock data:`, error);
        const category = mockCategories.find((c) => c.id === id);
        if (!category) {
          throw new Error("Category not found in mock database");
        }
        return { ...category };
      }
    },
    enabled: !!id,
  });
}

// --- Mutation to Create Category ---
export function useCreateCategory() {
  const queryClient = useQueryClient();
  return useMutation<Category, Error, CreateCategoryInput>({
    mutationFn: async (newCategory) => {
      try {
        const response = await api.post<Category>("/admin/categories", newCategory);
        return response.data;
      } catch (error) {
        console.warn("Failed to create category on backend, falling back to mock database:", error);
        
        const created: Category = {
          id: `cat-${Date.now()}`,
          name: newCategory.name,
          slug: newCategory.slug || newCategory.name.toLowerCase().replace(/[^a-z0-9]+/g, "-"),
          description: newCategory.description,
          iconName: newCategory.iconName || "FolderOpen",
          isActive: newCategory.isActive,
          auctionsCount: 0,
          subcategories: [],
        };
        
        mockCategories = [created, ...mockCategories];
        return created;
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "categories"] });
    },
  });
}

// --- Mutation to Update Category ---
export function useUpdateCategory() {
  const queryClient = useQueryClient();
  return useMutation<Category, Error, { id: string; data: UpdateCategoryInput }>({
    mutationFn: async ({ id, data }) => {
      try {
        const response = await api.put<Category>(`/admin/categories/${id}`, data);
        return response.data;
      } catch (error) {
        console.warn(`Failed to update category ${id} on backend, falling back to mock database:`, error);
        
        const index = mockCategories.findIndex((c) => c.id === id);
        if (index === -1) {
          throw new Error("Category not found in mock database");
        }
        
        const updatedCategory: Category = {
          ...mockCategories[index],
          ...data,
          slug: data.slug || (data.name ? data.name.toLowerCase().replace(/[^a-z0-9]+/g, "-") : mockCategories[index].slug),
        };
        
        mockCategories = [
          ...mockCategories.slice(0, index),
          updatedCategory,
          ...mockCategories.slice(index + 1),
        ];
        
        return updatedCategory;
      }
    },
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ["admin", "categories"] });
      queryClient.invalidateQueries({ queryKey: ["admin", "categories", data.id] });
    },
  });
}

// --- Mutation to Delete Category ---
export function useDeleteCategory() {
  const queryClient = useQueryClient();
  return useMutation<void, Error, string>({
    mutationFn: async (id) => {
      try {
        await api.delete<void>(`/admin/categories/${id}`);
      } catch (error) {
        console.warn(`Failed to delete category ${id} on backend, falling back to mock database:`, error);
        
        const index = mockCategories.findIndex((c) => c.id === id);
        if (index === -1) {
          throw new Error("Category not found in mock database");
        }
        
        mockCategories = [
          ...mockCategories.slice(0, index),
          ...mockCategories.slice(index + 1),
        ];
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "categories"] });
    },
  });
}

// --- Mutation to Create Subcategory ---
export function useCreateSubcategory() {
  const queryClient = useQueryClient();
  return useMutation<Subcategory, Error, CreateSubcategoryInput>({
    mutationFn: async (newSub) => {
      try {
        const response = await api.post<Subcategory>(
          `/admin/categories/${newSub.parentCategoryId}/subcategories`,
          newSub
        );
        return response.data;
      } catch (error) {
        console.warn("Failed to create subcategory on backend, falling back to mock database:", error);
        
        const parentCategory = mockCategories.find((c) => c.id === newSub.parentCategoryId);
        if (!parentCategory) {
          throw new Error("Parent category not found in mock database");
        }
        
        const created: Subcategory = {
          id: `sub-${Date.now()}`,
          name: newSub.name,
          slug: newSub.slug || newSub.name.toLowerCase().replace(/[^a-z0-9]+/g, "-"),
          description: newSub.description,
          isActive: newSub.isActive,
          parentCategoryId: newSub.parentCategoryId,
          auctionsCount: 0,
        };
        
        parentCategory.subcategories = [...parentCategory.subcategories, created];
        return created;
      }
    },
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ["admin", "categories"] });
      queryClient.invalidateQueries({ queryKey: ["admin", "categories", data.parentCategoryId] });
    },
  });
}

// --- Mutation to Update Subcategory ---
export function useUpdateSubcategory() {
  const queryClient = useQueryClient();
  return useMutation<Subcategory, Error, { id: string; data: UpdateSubcategoryInput }>({
    mutationFn: async ({ id, data }) => {
      try {
        const response = await api.put<Subcategory>(`/admin/subcategories/${id}`, data);
        return response.data;
      } catch (error) {
        console.warn(`Failed to update subcategory ${id} on backend, falling back to mock database:`, error);
        
        let foundSub: Subcategory | null = null;
        let parentCat: Category | null = null;
        
        for (const cat of mockCategories) {
          const sub = cat.subcategories.find((s) => s.id === id);
          if (sub) {
            foundSub = sub;
            parentCat = cat;
            break;
          }
        }
        
        if (!foundSub || !parentCat) {
          throw new Error("Subcategory not found in mock database");
        }
        
        const updatedSub: Subcategory = {
          ...foundSub,
          ...data,
          slug: data.slug || (data.name ? data.name.toLowerCase().replace(/[^a-z0-9]+/g, "-") : foundSub.slug),
        };
        
        parentCat.subcategories = parentCat.subcategories.map((s) => (s.id === id ? updatedSub : s));
        return updatedSub;
      }
    },
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ["admin", "categories"] });
      queryClient.invalidateQueries({ queryKey: ["admin", "categories", data.parentCategoryId] });
    },
  });
}

// --- Mutation to Delete Subcategory ---
export function useDeleteSubcategory() {
  const queryClient = useQueryClient();
  return useMutation<void, Error, { id: string; parentCategoryId: string }>({
    mutationFn: async ({ id, parentCategoryId }) => {
      try {
        await api.delete<void>(`/admin/subcategories/${id}`);
      } catch (error) {
        console.warn(`Failed to delete subcategory ${id} on backend, falling back to mock database:`, error);
        
        const parentCategory = mockCategories.find((c) => c.id === parentCategoryId);
        if (!parentCategory) {
          throw new Error("Parent category not found in mock database");
        }
        
        parentCategory.subcategories = parentCategory.subcategories.filter((s) => s.id !== id);
      }
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["admin", "categories"] });
      queryClient.invalidateQueries({ queryKey: ["admin", "categories", variables.parentCategoryId] });
    },
  });
}
