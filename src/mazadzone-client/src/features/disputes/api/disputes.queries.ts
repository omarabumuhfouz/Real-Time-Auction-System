import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { 
  fileDispute, 
  fetchDisputeTypes, 
  createDisputeTypeApi, 
  updateDisputeTypeApi, 
  deleteDisputeTypeApi 
} from "./disputes.api";
import type { CreateDisputeInput, Dispute } from "../types/disputes.types";
import type { DisputeTypeDto, CreateDisputeTypeRequest } from "./disputes.contracts";

export const disputeKeys = {
  all: ["disputes"] as const,
  types: ["dispute-types"] as const,
};

// Default dispute types list to fallback on if backend yields empty
export const DEFAULT_DISPUTE_TYPES: DisputeTypeDto[] = [
  { id: "1", name: "Damaged Item", description: "Item arrived broken, non-functional, or significantly damaged during transit.", isActive: true },
  { id: "2", name: "Not as Described", description: "The item significantly differs from listing photos, descriptions, or parameters.", isActive: true },
  { id: "3", name: "Non-Delivery", description: "Seller did not ship the item, or shipment was completely lost in transit.", isActive: true },
  { id: "4", name: "Late Shipment", description: "Item was shipped or delivered significantly after the agreed timeline.", isActive: true },
  { id: "5", name: "Fraudulent Listing", description: "Listing has stolen photos, fake brand credentials, or is an outright scam.", isActive: true }
];

/**
 * Mutation to file a dispute for an order.
 */
export function useFileDispute() {
  const queryClient = useQueryClient();

  return useMutation<Dispute, Error, CreateDisputeInput>({
    mutationFn: fileDispute,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: disputeKeys.all });
    },
  });
}

/**
 * Query to retrieve configured dispute types.
 */
export function useGetDisputeTypes() {
  return useQuery<DisputeTypeDto[]>({
    queryKey: disputeKeys.types,
    queryFn: async () => {
      try {
        const data = await fetchDisputeTypes();
        return data.length > 0 ? data : DEFAULT_DISPUTE_TYPES;
      } catch (error) {
        console.warn("Failed to fetch dispute types from backend, falling back to mock defaults:", error);
        return DEFAULT_DISPUTE_TYPES;
      }
    },
    staleTime: 5 * 60 * 1000,
  });
}

/**
 * Mutation to create a new dispute type classification.
 */
export function useCreateDisputeType() {
  const queryClient = useQueryClient();

  return useMutation<void, Error, CreateDisputeTypeRequest>({
    mutationFn: createDisputeTypeApi,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: disputeKeys.types });
    },
  });
}

export interface UpdateDisputeTypeParams {
  id: string;
  request: CreateDisputeTypeRequest;
}

/**
 * Mutation to update an existing dispute type classification.
 */
export function useUpdateDisputeType() {
  const queryClient = useQueryClient();

  return useMutation<void, Error, UpdateDisputeTypeParams>({
    mutationFn: ({ id, request }) => updateDisputeTypeApi(id, request),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: disputeKeys.types });
    },
  });
}

/**
 * Mutation to soft delete a dispute type classification.
 */
export function useDeleteDisputeType() {
  const queryClient = useQueryClient();

  return useMutation<void, Error, string>({
    mutationFn: deleteDisputeTypeApi,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: disputeKeys.types });
    },
  });
}
