export interface CreateDisputeInput {
  orderId: string;
  disputeTypeId: string;
  title: string;
  description: string;
  evidenceFiles?: File[];
}

export interface Dispute {
  id: string;
  orderId: string;
  title: string;
  description: string;
  status: "Pending" | "Resolved" | "UnderReview";
  createdAt: string;
  updatedAt: string;
}
