/**
 * Auction Mutation API fetch functions.
 * Consumed by TanStack Query mutation hooks.
 */

import type {
  Auction,
  CreateAuctionInput,
  UpdateAuctionInput,
} from "../types/auction.types";
import { api } from "@/lib/api/client";
import type { ApiResponse } from "@/types/api.types";
import { simulateDelay } from "./auctions-query.api";

/**
 * Helper to build FormData dynamically from a flat object, excluding specified keys.
 */
function buildFormData(
  input: Record<string, any>,
  excludeKeys: string[] = [],
): FormData {
  const formData = new FormData();

  Object.entries(input).forEach(([key, value]) => {
    if (excludeKeys.includes(key)) return;
    if (value != null) {
      if (value instanceof File) {
        formData.append(key, value);
      } else {
        formData.append(key, value.toString());
      }
    }
  });

  return formData;
}

/**
 * Sends a POST request to create a new auction.
 * Since creation includes local files, it always uses multipart/form-data.
 */
export async function createAuctionApi(input: CreateAuctionInput): Promise<Auction> {
  const formData = buildFormData(input, ["images"]);

  input.images.forEach((file) => {
    formData.append("images", file);
  });

  const { data } = await api.post<Auction>("/auctions", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });

  return data;
}

/**
 * Sends a PATCH request to update an existing auction.
 * Dynamically switches to multipart/form-data if new files/images are added.
 */
export async function updateAuctionApi(
  id: string,
  input: UpdateAuctionInput,
): Promise<Auction> {
  const hasFiles = input.images?.some((img) => img instanceof File);

  if (hasFiles) {
    const formData = buildFormData(input, ["images"]);

    input.images?.forEach((image) => {
      if (image instanceof File) {
        formData.append("images", image);
      } else {
        formData.append("existingImages", image);
      }
    });

    const { data } = await api.patch<Auction>(`/auctions/${id}`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });

    return data;
  }

  const { data } = await api.patch<Auction>(`/auctions/${id}`, input);
  return data;
}

/**
 * Sends a DELETE request to delete an auction.
 */
export async function deleteAuctionApi(id: string): Promise<ApiResponse<void>> {
  await simulateDelay();

  /**
   * --- REAL API CALL (Uncomment when backend is ready) ---
   * return api.delete<void>(`/auctions/${id}`);
   */

  return {
    data: undefined as any,
    message: "Auction listing has been deleted successfully.",
    success: true,
    timestamp: new Date().toISOString(),
  };
}
