import { api } from "@/lib/api/client";
import type { CreateDisputeInput, Dispute } from "../types/disputes.types";

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
