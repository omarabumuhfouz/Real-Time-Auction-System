import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import type { AdminDashboardOverviewData, AuctionActivityTrend, UserGrowthTrend, ModerateUsersResponse, ModerateUserRole, ModerateUserStatus, ModerateUser } from "../types/admin.types";


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

// --- Moderate Users Mock Data & Hook ---

const generateMockUsers = (): ModerateUser[] => {
  const roles: ModerateUserRole[] = ["Bidder", "Seller", "Admin"];
  const statuses: ModerateUserStatus[] = ["Active", "Suspended", "Banned"];
  
  const baseUsers = [
    { fullName: "Ahmad Khan", email: "ahmad.khan@email.com" },
    { fullName: "Sara Ali", email: "sara.ali@email.com" },
    { fullName: "Bilal Hussain", email: "bilal.hussain@email.com" },
    { fullName: "Ayesha Malik", email: "ayesha.malik@email.com" },
    { fullName: "Usman Tariq", email: "usman.tariq@email.com" },
    { fullName: "Hassan Raza", email: "hassan.raza@email.com" },
    { fullName: "Zainab Fatima", email: "zainab.fatima@email.com" },
    { fullName: "Faisal Noor", email: "faisal.noor@email.com" },
    { fullName: "Imran Siddiqui", email: "imran.siddiqui@email.com" },
    { fullName: "Nida Ahmed", email: "nida.ahmed@email.com" },
  ];

  const users: ModerateUser[] = [];
  
  for (let i = 1; i <= 65; i++) {
    const base = baseUsers[(i - 1) % baseUsers.length];
    const role = roles[(i - 1) % roles.length];
    const status = i % 12 === 0 ? "Banned" : i % 8 === 0 ? "Suspended" : "Active";
    
    const joinedDate = new Date();
    joinedDate.setDate(joinedDate.getDate() - (i % 30));
    joinedDate.setHours(10 + (i % 12), i % 60, 0, 0);

    let lastActive = "Today 10:24 AM";
    if (i % 3 === 0) {
      lastActive = "Today 8:15 AM";
    } else if (i % 3 === 1) {
      lastActive = "Yesterday 4:32 PM";
    } else {
      const activeDate = new Date();
      activeDate.setDate(activeDate.getDate() - (i % 5) - 1);
      lastActive = activeDate.toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" }) + " 2:15 PM";
    }

    users.push({
      id: `u${i}`,
      fullName: i > baseUsers.length ? `${base.fullName} ${Math.ceil(i / baseUsers.length)}` : base.fullName,
      email: i > baseUsers.length 
        ? `${base.email.split('@')[0]}.${Math.ceil(i / baseUsers.length)}@${base.email.split('@')[1]}`
        : base.email,
      role,
      status,
      activity: {
        auctions: (i * 3) % 50,
        bids: (i * 7) % 150
      },
      joinedDate: joinedDate.toISOString(),
      lastActive,
    });
  }
  return users;
};

const MOCK_MODERATE_USERS_DATA: ModerateUsersResponse = {
  data: generateMockUsers(),
  totalCount: 65,
  page: 1,
  pageSize: 10,
  totalPages: 7,
};

export interface UseModerateUsersFilters {
  search: string;
  role: ModerateUserRole | "All Roles";
  status: ModerateUserStatus | "All Statuses";
  sortBy: string;
  page: number;
  pageSize: number;
}

export function useModerateUsers(filters: UseModerateUsersFilters) {
  return useQuery<ModerateUsersResponse>({
    queryKey: ["admin", "moderate-users", filters],
    queryFn: async () => {
      try {
        const queryParams = new URLSearchParams({
          search: filters.search,
          role: filters.role,
          status: filters.status,
          sortBy: filters.sortBy,
          page: filters.page.toString(),
          pageSize: filters.pageSize.toString(),
        }).toString();
        
        const response = await api.get<ModerateUsersResponse>(`/admin/users?${queryParams}`);
        return response.data;
      } catch (error) {
        console.warn("Failed to fetch moderate users from backend, falling back to mock data:", error);
        
        // Return filtered mock data
        let filteredData = [...MOCK_MODERATE_USERS_DATA.data];
        
        if (filters.search) {
          const lowerQuery = filters.search.toLowerCase();
          filteredData = filteredData.filter(
            (u) => u.fullName.toLowerCase().includes(lowerQuery) || u.email.toLowerCase().includes(lowerQuery)
          );
        }
        if (filters.role !== "All Roles") {
          filteredData = filteredData.filter((u) => u.role === filters.role);
        }
        if (filters.status !== "All Statuses") {
          filteredData = filteredData.filter((u) => u.status === filters.status);
        }

        // Apply pagination mock
        const startIndex = (filters.page - 1) * filters.pageSize;
        const endIndex = startIndex + filters.pageSize;
        const paginatedData = filteredData.slice(startIndex, endIndex);
        
        return {
          ...MOCK_MODERATE_USERS_DATA,
          data: paginatedData,
          totalCount: filteredData.length,
          page: filters.page,
          pageSize: filters.pageSize,
          totalPages: Math.ceil(filteredData.length / filters.pageSize),
        };
      }
    },
    staleTime: 60 * 1000,
  });
}

export async function exportUsersApi(filters: UseModerateUsersFilters, selectedIds: string[]): Promise<Blob> {
  try {
    const params: Record<string, string> = {
      search: filters.search,
      role: filters.role,
      status: filters.status,
      sortBy: filters.sortBy,
    };
    
    // Pass selected IDs to the server if bulk exporting
    if (selectedIds.length > 0) {
      params.selectedIds = selectedIds.join(',');
    }

    const response = await api.get<Blob>("/admin/users/export", {
      params,
      responseType: "blob",
    });
    
    return response.data;
  } catch (error) {
    console.warn("Failed to reach real export endpoint, generating mock CSV blob:", error);
    
    // --- Mock Fallback for local testing ---
    
    // Simulate network delay
    await new Promise((resolve) => setTimeout(resolve, 1500));
    
    // Filter the mock data just like the API would
    let filteredData = [...MOCK_MODERATE_USERS_DATA.data];
    if (selectedIds.length > 0) {
      filteredData = filteredData.filter((u) => selectedIds.includes(u.id));
    } else {
      if (filters.search) {
        const lowerQuery = filters.search.toLowerCase();
        filteredData = filteredData.filter(
          (u) => u.fullName.toLowerCase().includes(lowerQuery) || u.email.toLowerCase().includes(lowerQuery)
        );
      }
      if (filters.role !== "All Roles") {
        filteredData = filteredData.filter((u) => u.role === filters.role);
      }
      if (filters.status !== "All Statuses") {
        filteredData = filteredData.filter((u) => u.status === filters.status);
      }
    }
    
    // Generate CSV string
    const csvHeader = "ID,Full Name,Email,Role,Status,Joined Date,Last Active\n";
    const csvRows = filteredData.map(u => 
      `${u.id},"${u.fullName}","${u.email}",${u.role},${u.status},${u.joinedDate},"${u.lastActive}"`
    ).join("\n");
    
    const csvContent = csvHeader + csvRows;
    return new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
  }
}

export function updateMockUserStatus(userId: string, status: ModerateUserStatus) {
  const user = MOCK_MODERATE_USERS_DATA.data.find((u) => u.id === userId);
  if (user) {
    user.status = status;
    return true;
  }
  return false;
}



