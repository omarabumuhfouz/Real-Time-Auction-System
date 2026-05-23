export enum AdminDisputeStatus {
  Open = "Open",
  UnderReview = "Under Review",
  AwaitingResponse = "Awaiting Response",
  Resolved = "Resolved",
  Rejected = "Rejected",
}

export enum AdminDisputeCategory {
  ItemNotAsDescribed = "Item Not as Described",
  ItemNotReceived = "Item Not Received",
  DamagedItem = "Damaged Item",
  DeliveryDelay = "Delivery Delay",
  PaymentIssue = "Payment Issue",
  RefundRequest = "Refund Request",
}

export interface AdminDisputeParties {
  claimant: string;
  respondent: string;
}

export interface AdminDispute {
  id: string;
  orderOrAuctionId: string;
  orderOrAuctionName: string;
  parties: AdminDisputeParties;
  category: AdminDisputeCategory;
  status: AdminDisputeStatus;
  submittedDate: string; // ISO string or formatted string
}

export interface AdminDisputesFilters {
  search?: string;
  status?: string;
  category?: string;
  sortBy?: string;
  submittedDate?: string;
  page?: number;
  pageSize?: number;
}
