import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type {
  Category,
  Subcategory,
  CreateCategoryInput,
  UpdateCategoryInput,
  CreateSubcategoryInput,
  UpdateSubcategoryInput,
} from "../../types/category.types";
import {
  fetchCategoriesTree,
  fetchCategoryDetail,
  createCategoryApi,
  updateCategoryApi,
  deleteCategoryApi,
  createSubcategoryApi,
  updateSubcategoryApi,
  deleteSubcategoryApi,
} from "./category.api";
import { categoryKeys } from "./category.keys";

// --- Hook to get all Categories ---
export function useGetCategories() {
  return useQuery<Category[]>({
    queryKey: categoryKeys.tree(),
    queryFn: fetchCategoriesTree,
    staleTime: 5 * 60 * 1000,
  });
}

// --- Hook to get Category detail by ID ---
export function useGetCategoryDetail(id: string) {
  return useQuery<Category>({
    queryKey: categoryKeys.detail(id),
    queryFn: () => fetchCategoryDetail(id),
    enabled: !!id,
  });
}

// --- Mutation to Create Category ---
export function useCreateCategory() {
  const queryClient = useQueryClient();
  return useMutation<Category, Error, CreateCategoryInput>({
    mutationFn: createCategoryApi,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.all });
    },
  });
}

// --- Mutation to Update Category ---
export function useUpdateCategory() {
  const queryClient = useQueryClient();
  return useMutation<Category, Error, { id: string; data: UpdateCategoryInput }>({
    mutationFn: ({ id, data }) => updateCategoryApi(id, data),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.all });
      queryClient.invalidateQueries({ queryKey: categoryKeys.detail(data.id) });
    },
  });
}

// --- Mutation to Delete Category ---
export function useDeleteCategory() {
  const queryClient = useQueryClient();
  return useMutation<void, Error, string>({
    mutationFn: deleteCategoryApi,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.all });
    },
  });
}

// --- Mutation to Create Subcategory ---
export function useCreateSubcategory() {
  const queryClient = useQueryClient();
  return useMutation<Subcategory, Error, CreateSubcategoryInput>({
    mutationFn: createSubcategoryApi,
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.all });
      queryClient.invalidateQueries({ queryKey: categoryKeys.detail(data.parentCategoryId) });
    },
  });
}

// --- Mutation to Update Subcategory ---
export function useUpdateSubcategory() {
  const queryClient = useQueryClient();
  return useMutation<Subcategory, Error, { id: string; data: UpdateSubcategoryInput }>({
    mutationFn: ({ id, data }) => updateSubcategoryApi(id, data),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.all });
      if (data.parentCategoryId) {
        queryClient.invalidateQueries({ queryKey: categoryKeys.detail(data.parentCategoryId) });
      }
    },
  });
}

// --- Mutation to Delete Subcategory ---
export function useDeleteSubcategory() {
  const queryClient = useQueryClient();
  return useMutation<void, Error, { id: string; parentCategoryId: string }>({
    mutationFn: ({ id, parentCategoryId }) => deleteSubcategoryApi(id, parentCategoryId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.all });
      queryClient.invalidateQueries({ queryKey: categoryKeys.detail(variables.parentCategoryId) });
    },
  });
}
