/**
 * Local contract representations of backend API contracts for the Disputes feature.
 */

export interface OpenDisputeRequest {
  reason: string;
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
