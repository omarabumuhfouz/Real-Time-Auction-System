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
    queryFn: fetchDisputeTypes,
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
