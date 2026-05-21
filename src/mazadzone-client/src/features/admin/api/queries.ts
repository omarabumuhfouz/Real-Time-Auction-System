import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import type { AdminDashboardOverviewData, AuctionActivityTrend, UserGrowthTrend } from "../types/admin.types";

// --- Mock Data Matching the Reference Screenshot ---
const MOCK_ADMIN_OVERVIEW_DATA: AdminDashboardOverviewData = {
  metrics: {
    totalAuctions: { value: 12840, changePercent: 10.5, isPositive: true },
    liveAuctions: { value: 2162, changePercent: 8.3, isPositive: true },
    endingWithin24h: { value: 145, changePercent: 15.2, isPositive: true },
    completedOrders: { value: 3840, changePercent: 12.1, isPositive: true },
    openDisputes: { value: 118, changePercent: -4.7, isPositive: false },
    platformRevenue: { value: 48720, changePercent: 22.4, isPositive: true },
  },
  auctionActivity: {
    dataPoints: [
      { label: "May 6-12", newAuctions: 1000, bidsPlaced: 9000 },
      { label: "May 13-19", newAuctions: 1300, bidsPlaced: 13000 },
      { label: "May 20-26", newAuctions: 1600, bidsPlaced: 11000 },
      { label: "May 27-Jun 2", newAuctions: 1400, bidsPlaced: 13000 },
      { label: "Jun 3-9", newAuctions: 1600, bidsPlaced: 18000 },
    ],
    totalNewAuctions: { value: 7216, changePercent: 10.4, isPositive: true },
    totalBidsPlaced: { value: 68540, changePercent: 14.3, isPositive: true },
  },
  openDisputesBreakdown: {
    totalItems: 118,
    queues: [
      { key: "shipping-delivery", name: "Shipping & Delivery Issues", count: 42, changePercent: 8.7, isPositive: true },
      { key: "item-condition", name: "Item Not As Described", count: 36, changePercent: 5.1, isPositive: true },
      { key: "payment-holds", name: "Payment & Holds Glitches", count: 24, changePercent: -3.2, isPositive: false },
      { key: "others", name: "Others", count: 16, changePercent: 12.4, isPositive: true },
    ],
  },
  userTrust: {
    workflowSteps: [
      { step: 1, name: "Registered Users", count: 28734 },
      { step: 2, name: "Seller Enabled", count: 3128 },
    ],
    accountStatusOverview: [
      { status: "Active Accounts", count: 25452, percentage: 88.6, color: "success" },
      { status: "Suspended Accounts", count: 2432, percentage: 8.5, color: "warning" },
      { status: "Banned Accounts", count: 850, percentage: 2.9, color: "destructive" },
    ],
    trustScore: 96.7,
    trustScoreChangePercent: 1.3,
    isPositive: true,
  },
  userGrowth: {
    dataPoints: [
      { label: "Apr 29 - May 5", newUsers: 410, newSellers: 110 },
      { label: "May 6-12", newUsers: 620, newSellers: 210 },
      { label: "May 13-19", newUsers: 780, newSellers: 260 },
      { label: "May 20-26", newUsers: 980, newSellers: 320 },
      { label: "May 27-Jun 2", newUsers: 1150, newSellers: 390 },
      { label: "Jun 3-9", newUsers: 1720, newSellers: 610 },
    ],
    totalNewUsers: { value: 6670, changePercent: 19.3, isPositive: true },
    totalNewSellers: { value: 2270, changePercent: 21.6, isPositive: true },
  },
  categoryHealth: {
    categories: [
      { name: "Tech and Electronics", liveAuctionsCount: 642 },
      { name: "Fashion and Style", liveAuctionsCount: 538 },
      { name: "Motors", liveAuctionsCount: 392 },
      { name: "Home and Living", liveAuctionsCount: 268 },
      { name: "Collectibles and Art", liveAuctionsCount: 198 },
      { name: "Hobbies and Leisure", liveAuctionsCount: 124 },
    ],
    totalLiveAuctions: 2162,
    totalLiveAuctionsChangePercent: 8.3,
    isPositive: true,
  },
  payments: {
    isConnected: true,
    heldFunds: 184250,
    completedPayments: 1086340,
    failedPayments: 2760,
    refundsChargebacks: 28760,
    lastSync: "2 mins ago",
  },
};

/**
 * Hook to retrieve admin overview statistics.
 * Integrates with standard API endpoints but falls back to screenshot values if backend is offline.
 */
export function useGetAdminOverviewStats() {
  return useQuery<AdminDashboardOverviewData>({
    queryKey: ["admin", "stats", "overview"],
    queryFn: async () => {
      try {
        const response = await api.get<AdminDashboardOverviewData>("/admin/stats/overview");
        return response.data;
      } catch (error) {
        console.warn("Failed to fetch admin stats from backend, falling back to mock data:", error);
        return MOCK_ADMIN_OVERVIEW_DATA;
      }
    },
    // Keep data fresh during layout interaction, but cache is fallback
    staleTime: 5 * 60 * 1000,
  });
}

function getMockActivityData(timeframe: string): AuctionActivityTrend {
  if (timeframe === "month") {
    return {
      dataPoints: [
        { label: "Week 1", newAuctions: 950, bidsPlaced: 8500 },
        { label: "Week 2", newAuctions: 1200, bidsPlaced: 11000 },
        { label: "Week 3", newAuctions: 1550, bidsPlaced: 10500 },
        { label: "Week 4", newAuctions: 1300, bidsPlaced: 12500 },
        { label: "Week 5", newAuctions: 1500, bidsPlaced: 17000 },
      ],
      totalNewAuctions: { value: 6500, changePercent: 8.2, isPositive: true },
      totalBidsPlaced: { value: 59500, changePercent: 11.8, isPositive: true },
    };
  }
  if (timeframe === "year") {
    return {
      dataPoints: [
        { label: "Jan-Mar", newAuctions: 3800, bidsPlaced: 28000 },
        { label: "Apr-Jun", newAuctions: 4600, bidsPlaced: 36000 },
        { label: "Jul-Sep", newAuctions: 5400, bidsPlaced: 42000 },
        { label: "Oct-Dec", newAuctions: 6200, bidsPlaced: 51000 },
      ],
      totalNewAuctions: { value: 20000, changePercent: 15.6, isPositive: true },
      totalBidsPlaced: { value: 157000, changePercent: 18.2, isPositive: true },
    };
  }
  // Default is "week"
  return MOCK_ADMIN_OVERVIEW_DATA.auctionActivity;
}

function getMockGrowthData(timeframe: string): UserGrowthTrend {
  if (timeframe === "month") {
    return {
      dataPoints: [
        { label: "Week 1", newUsers: 390, newSellers: 95 },
        { label: "Week 2", newUsers: 580, newSellers: 190 },
        { label: "Week 3", newUsers: 720, newSellers: 230 },
        { label: "Week 4", newUsers: 900, newSellers: 290 },
        { label: "Week 5", newUsers: 1450, newSellers: 480 },
      ],
      totalNewUsers: { value: 4040, changePercent: 14.5, isPositive: true },
      totalNewSellers: { value: 1285, changePercent: 16.8, isPositive: true },
    };
  }
  if (timeframe === "year") {
    return {
      dataPoints: [
        { label: "Jan-Mar", newUsers: 1800, newSellers: 450 },
        { label: "Apr-Jun", newUsers: 2400, newSellers: 680 },
        { label: "Jul-Sep", newUsers: 3100, newSellers: 850 },
        { label: "Oct-Dec", newUsers: 3900, newSellers: 1100 },
      ],
      totalNewUsers: { value: 11200, changePercent: 25.4, isPositive: true },
      totalNewSellers: { value: 3080, changePercent: 28.1, isPositive: true },
    };
  }
  // Default is "week"
  return MOCK_ADMIN_OVERVIEW_DATA.userGrowth;
}

export function useGetAuctionActivityTrend(timeframe: string) {
  return useQuery<AuctionActivityTrend>({
    queryKey: ["admin", "stats", "activity", timeframe],
    queryFn: async () => {
      try {
        const response = await api.get<AuctionActivityTrend>(`/admin/stats/activity?timeframe=${timeframe}`);
        return response.data;
      } catch (error) {
        console.warn(`Failed to fetch admin activity stats for ${timeframe}, falling back to mock data:`, error);
        return getMockActivityData(timeframe);
      }
    },
    staleTime: 5 * 60 * 1000,
  });
}

export function useGetUserGrowthTrend(timeframe: string) {
  return useQuery<UserGrowthTrend>({
    queryKey: ["admin", "stats", "growth", timeframe],
    queryFn: async () => {
      try {
        const response = await api.get<UserGrowthTrend>(`/admin/stats/growth?timeframe=${timeframe}`);
        return response.data;
      } catch (error) {
        console.warn(`Failed to fetch admin growth stats for ${timeframe}, falling back to mock data:`, error);
        return getMockGrowthData(timeframe);
      }
    },
    staleTime: 5 * 60 * 1000,
  });
}
