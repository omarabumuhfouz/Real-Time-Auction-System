export interface MetricTrend {
  value: number | string;
  changePercent: number;
  isPositive: boolean;
}

export interface AdminMetrics {
  totalAuctions: MetricTrend;
  liveAuctions: MetricTrend;
  endingWithin24h: MetricTrend;
  completedOrders: MetricTrend;
  openDisputes: MetricTrend;
  platformRevenue: MetricTrend;
}

export interface BiddingTrendDataPoint {
  label: string; // e.g., "May 6-12"
  newAuctions: number;
  bidsPlaced: number;
}

export interface AuctionActivityTrend {
  dataPoints: BiddingTrendDataPoint[];
  totalNewAuctions: {
    value: number;
    changePercent: number;
    isPositive: boolean;
  };
  totalBidsPlaced: {
    value: number;
    changePercent: number;
    isPositive: boolean;
  };
}

export interface DisputeBreakdownItem {
  key: string; // e.g., "users-awaiting-verification"
  name: string;
  count: number;
  changePercent: number;
  isPositive: boolean;
}

export interface OpenDisputesBreakdown {
  totalItems: number;
  queues: DisputeBreakdownItem[];
}

export interface VerificationWorkflowStep {
  step: number;
  name: string;
  count: number;
}

export interface AccountStatusItem {
  status: string;
  count: number;
  percentage: number;
  color: string; // color mapping key, e.g. "success", "upcoming", "warning", "info"
}

export interface UserTrustStats {
  workflowSteps: VerificationWorkflowStep[];
  accountStatusOverview: AccountStatusItem[];
  trustScore: number;
  trustScoreChangePercent: number;
  isPositive: boolean;
}

export interface UserGrowthDataPoint {
  label: string; // e.g., "May 6-12"
  newUsers: number;
  newSellers: number;
}

export interface UserGrowthTrend {
  dataPoints: UserGrowthDataPoint[];
  totalNewUsers: {
    value: number;
    changePercent: number;
    isPositive: boolean;
  };
  totalNewSellers: {
    value: number;
    changePercent: number;
    isPositive: boolean;
  };
}

export interface CategoryLiveAuctions {
  name: string;
  liveAuctionsCount: number;
}

export interface CategoryHealthStats {
  categories: CategoryLiveAuctions[];
  totalLiveAuctions: number;
  totalLiveAuctionsChangePercent: number;
  isPositive: boolean;
}

export interface PaymentStats {
  isConnected: boolean;
  heldFunds: number;
  completedPayments: number;
  failedPayments: number;
  refundsChargebacks: number;
  lastSync: string; // formatted string like "2 mins ago"
}

export interface AdminDashboardOverviewData {
  metrics: AdminMetrics;
  auctionActivity: AuctionActivityTrend;
  openDisputesBreakdown: OpenDisputesBreakdown;
  userTrust: UserTrustStats;
  userGrowth: UserGrowthTrend;
  categoryHealth: CategoryHealthStats;
  payments: PaymentStats;
}

export type ModerateUserRole = "Bidder" | "Seller" | "Admin";
export type ModerateUserStatus = "Active" | "Suspended" | "Banned";

export interface ModerateUser {
  id: string;
  fullName: string;
  email: string;
  avatarUrl?: string;
  role: ModerateUserRole;
  status: ModerateUserStatus;
  activity: {
    auctions: number;
    bids: number;
  };
  joinedDate: string; // ISO date string
  lastActive: string; // "Today 10:24 AM", "Yesterday 6:47 PM", etc.
}

export interface ModerateUsersResponse {
  data: ModerateUser[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

