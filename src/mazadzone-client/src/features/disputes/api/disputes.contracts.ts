import type { ImageModelDto } from "@/features/auctions/api/auction.contracts";

/**
 * Local contract representations of backend API contracts for the Disputes feature.
 */

export interface OpenDisputeRequest {
  orderId: string;
  disputeTypeId: string;
  title: string;
  description: string;
  images?: ImageModelDto[] | null;
}

export interface ResolveDisputeRequest {
  resolution: string;
}

export interface DisputeTypeDto {
  id: string;
  name: string;
  description: string;
  isActive: boolean;
}

export interface CreateDisputeTypeRequest {
  name: string;
  description: string;
}
