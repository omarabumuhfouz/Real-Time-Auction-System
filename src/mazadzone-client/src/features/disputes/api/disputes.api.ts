import { api } from "@/lib/api/client";
import type { CreateDisputeInput, Dispute } from "../types/disputes.types";
import type { DisputeTypeDto, CreateDisputeTypeRequest } from "./disputes.contracts";

/**
 * Files a new dispute for a given order on the ASP.NET Core backend.
 */
export async function fileDispute(input: CreateDisputeInput): Promise<Dispute> {
  await api.post(`/api/v1/orders/${input.orderId}/dispute`, {
    reason: `${input.title}\n\n${input.description}`,
  });

  return {
    id: `disp-${Math.random().toString(36).substring(2, 11)}`,
    orderId: input.orderId,
    title: input.title,
    description: input.description,
    status: "Pending",
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  };
}

/**
 * Retrieves all configured dispute types in the system.
 */
export async function fetchDisputeTypes(): Promise<DisputeTypeDto[]> {
  const response = await api.get<DisputeTypeDto[]>("/dispute-types");
  return response.data;
}

/**
 * Creates a new dispute type configuration.
 */
export async function createDisputeTypeApi(request: CreateDisputeTypeRequest): Promise<void> {
  await api.post("/dispute-types", request);
}

/**
 * Updates an existing dispute type configuration.
 */
export async function updateDisputeTypeApi(id: string, request: CreateDisputeTypeRequest): Promise<void> {
  await api.put(`/dispute-types/${id}`, request);
}

/**
 * Soft deletes an existing dispute type configuration.
 */
export async function deleteDisputeTypeApi(id: string): Promise<void> {
  await api.delete(`/dispute-types/${id}`);
}
