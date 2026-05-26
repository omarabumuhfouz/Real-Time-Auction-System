/**
 * Local contract representations of backend API contracts for the Disputes feature.
 */

export interface OpenDisputeRequest {
  reason: string;
}

export interface ResolveDisputeRequest {
  resolution: string;
}
