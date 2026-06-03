"use client";

import { useState } from "react";
import Link from "next/link";
import { useUrlFilters } from "@/hooks/use-url-filters";
import { Plus, Loader2, Download, Gavel, Box, DollarSign } from "lucide-react";
import { Button } from "@/components/ui/button";
import { PageWrapper } from "@/components/layout/page-wrapper";
import { PageHeader } from "@/components/layout/page-header";
import { DashboardShell } from "@/components/layout/dashboard-shell";
import { ROUTES } from "@/config/routes.config";
import { useRequireRole } from "@/hooks/use-require-role";
import {
  useDeleteAuction,
} from "@/features/auctions";
import {
  useGetSellerDashboardAuctions,
  useGetSellerDashboardFinancials,
  useGetSellerDashboardOrders,
} from "@/features/seller";
import { SellerDashboardStats } from "./SellerDashboardStats";
import { SellerAuctionsTable } from "./auctions/SellerAuctionsTable";
import { SellerOrdersTable } from "./orders/SellerOrdersTable";
import { SellerFinancialsTable } from "./financials/SellerFinancialsTable";
import { cn } from "@/lib/utils";

const TABS = [
  { key: "auctions" as const, label: "Auctions", icon: Gavel },
  { key: "orders" as const, label: "Orders", icon: Box },
  { key: "financials" as const, label: "Financials", icon: DollarSign },
] as const;

type TabKey = (typeof TABS)[number]["key"];

export function SellerDashboardPage() {
  const { searchParams, setFilters } = useUrlFilters<{ status: string; sortBy: string; page: number }>();
  const [activeTab, setActiveTab] = useState<TabKey>("auctions");
  
  // Local state for orders filtering to prevent URL clashes with auctions
  const [activeOrderStatus, setActiveOrderStatus] = useState<string>("All");
  const [orderSortBy, setOrderSortBy] = useState<string>("OrderDate");
  const [orderPage, setOrderPage] = useState<number>(1);

  // Local state for financials pagination
  const [financialsPage, setFinancialsPage] = useState<number>(1);

  // Authenticated route protection check via reusable hook
  const { isAuthorized, isLoading: isAuthLoading } = useRequireRole(["seller"], {
    loginMessage: "Please log in to access the Seller Dashboard.",
    unauthorizedMessage: "You must activate your seller privileges to view this page.",
    bypassTesting: false,
  });

  // Table Filtering, Sorting, Pagination from URL for Auctions
  const activeStatus = searchParams.get("status") || "All";
  const sortBy = searchParams.get("sortBy") || "EndTime";
  const tablePage = parseInt(searchParams.get("page") || "1", 10);

  // 1. Fetch current page of seller auctions for table display
  const {
    data: auctionsResponse,
    isLoading: isAuctionsLoading,
  } = useGetSellerDashboardAuctions({
    Page: tablePage,
    PageSize: 5,
    Status: activeStatus === "All" ? undefined : activeStatus,
    SortBy: sortBy === "EndTime" ? "endDateUtc" : sortBy === "CreationDate" ? "creationDate" : sortBy,
  });

  // 1b. Fetch overall seller auctions summary metrics (stable unfiltered query for dashboard stats)
  const {
    data: statsAuctionsResponse,
    isLoading: isStatsAuctionsLoading,
  } = useGetSellerDashboardAuctions({
    Page: 1,
    PageSize: 1,
  });

  // 2. Fetch seller financials summary metrics
  const {
    data: financialsResponse,
    isLoading: isFinancialsLoading,
  } = useGetSellerDashboardFinancials();

  // 3. Fetch seller orders count
  const {
    data: ordersResponse,
    isLoading: isOrdersLoading,
  } = useGetSellerDashboardOrders({
    Page: 1,
    PageSize: 1,
  });

  // 4. Fetch ALL seller orders to calculate capsule numbers dynamically
  const {
    data: allOrdersResponse,
  } = useGetSellerDashboardOrders({
    Page: 1,
    PageSize: 100,
  });

  // 5. Fetch current page of seller orders for table display (with filters & sorting applied)
  const {
    data: ordersTableResponse,
    isLoading: isOrdersTableLoading,
  } = useGetSellerDashboardOrders({
    Page: orderPage,
    PageSize: 5,
    Status: activeOrderStatus === "All" ? undefined : activeOrderStatus,
    SortBy: orderSortBy === "OrderDate" ? "orderDateUtc" : orderSortBy === "TotalAmount" ? "totalAmount" : orderSortBy,
  });

  // 6. Fetch paginated list of completed orders to represent completed payout transactions
  const {
    data: completedOrdersResponse,
    isLoading: isCompletedOrdersLoading,
  } = useGetSellerDashboardOrders({
    Page: financialsPage,
    PageSize: 5,
    Status: "Completed",
  });

  // Delete Auction Mutation
  const { mutateAsync: deleteAuction } = useDeleteAuction();

  const handleDeleteAuction = async (id: string) => {
    await deleteAuction(id).catch(() => {});
  };

  const handleStatusChange = (status: string) => {
    setFilters({ status, page: 1 });
  };

  const handleSortChange = (sort: string) => {
    setFilters({ sortBy: sort, page: 1 });
  };

  const handlePageChange = (page: number) => {
    setFilters({ page });
  };

  // Guard loading screen (prevent flash of protected content)
  if (isAuthLoading || !isAuthorized) {
    return (
      <PageWrapper className="flex items-center justify-center min-h-[70vh]">
        <div className="text-center space-y-4">
          <Loader2 className="h-10 w-10 animate-spin text-primary mx-auto" />
          <p className="text-muted-foreground font-semibold">Verifying credentials...</p>
        </div>
      </PageWrapper>
    );
  }

  // Aggregate stats metrics (from stable, unfiltered queries to avoid filter resets)
  const activeCount = statsAuctionsResponse?.activeAuctions || 0;
  const pendingCount = statsAuctionsResponse?.pending || 0;
  const soldCount = statsAuctionsResponse?.soldItems || 0;
  const unsoldCount = statsAuctionsResponse?.unsold || 0;
  const totalOrdersCount = ordersResponse?.totalCount || financialsResponse?.completedOrdersCount || 0;
  const grossRevenue = financialsResponse?.totalGrossRevenue || 0;
  const platformFees = financialsResponse?.totalPlatformFees || 0;
  const netProfit = financialsResponse?.totalNetProfit || 0;

  // Auctions calculations (from current page/filtered query for table display)
  const auctionsList = auctionsResponse?.auctions || [];
  const totalCount = auctionsResponse?.totalCount || 0;
  const totalPages = Math.ceil(totalCount / 5) || 1;

  // Orders calculations for capsules
  const allOrdersList = allOrdersResponse?.orders || [];
  const allOrdersCount = allOrdersResponse?.totalCount || 0;
  const pendingOrdersCount = allOrdersList.filter(o => o.orderStatus.toLowerCase() === "pending").length;
  const shippedOrdersCount = allOrdersList.filter(o => o.orderStatus.toLowerCase() === "shipped").length;
  const deliveredOrdersCount = allOrdersList.filter(o => o.orderStatus.toLowerCase() === "delivered").length;
  const completedOrdersCount = allOrdersList.filter(o => o.orderStatus.toLowerCase() === "completed" || o.orderStatus.toLowerCase() === "success").length;
  const canceledOrdersCount = allOrdersList.filter(o => o.orderStatus.toLowerCase() === "canceled" || o.orderStatus.toLowerCase() === "refunded" || o.orderStatus.toLowerCase() === "cancelled").length;

  const isStatsLoading = isStatsAuctionsLoading || isFinancialsLoading || isOrdersLoading;

  return (
    <PageWrapper className="bg-background min-h-screen">
      <DashboardShell>
        
        {/* Dashboard Header */}
        <PageHeader
          title="Seller Dashboard"
          subtitle="Manage your auctions, orders, and earnings in one place."
          actions={
            <>
              <Button
                variant="outline"
                className="rounded-xl h-10 px-5 font-semibold flex items-center gap-2 cursor-pointer"
              >
                <Download className="h-4 w-4 text-muted-foreground" />
                Export All
              </Button>
              <Link href={ROUTES.SELLER.CREATE_AUCTION}>
                <Button className="bg-primary hover:bg-primary/90 text-primary-foreground font-semibold h-10 px-5 rounded-xl flex items-center gap-2 cursor-pointer">
                  <Plus className="h-4.5 w-4.5" />
                  Create Auction
                </Button>
              </Link>
            </>
          }
        />

        {/* Overview Metric Strip */}
        <SellerDashboardStats
          activeAuctions={activeCount}
          pending={pendingCount}
          soldItems={soldCount}
          totalOrders={totalOrdersCount}
          grossRevenue={grossRevenue}
          netProfit={netProfit}
          isLoading={isStatsLoading}
        />

        {/* Tab Navigation + Content */}
        <div className="space-y-0">
          {/* Tab Headers */}
          <div className="flex items-center border border-border bg-card rounded-xl gap-0 px-4 mb-6">
            {TABS.map((tab) => {
              const Icon = tab.icon;
              const isActive = activeTab === tab.key;
              return (
                <button
                  key={tab.key}
                  onClick={() => setActiveTab(tab.key)}
                  className={cn(
                    "relative flex items-center gap-2 px-5 py-3 text-sm font-semibold transition-colors cursor-pointer",
                    isActive
                      ? "text-primary"
                      : "text-muted-foreground hover:text-foreground",
                  )}
                >
                  <Icon className="h-4 w-4" />
                  <span>{tab.label}</span>
                  {isActive && (
                    <span className="absolute bottom-0 left-0 right-0 h-[2px] bg-primary" />
                  )}
                </button>
              );
            })}
          </div>

          {/* Tab Content */}
          <div className="pt-0">
            {activeTab === "auctions" && (
              <SellerAuctionsTable
                auctions={auctionsList}
                totalCount={totalCount}
                currentPage={tablePage}
                totalPages={totalPages}
                isLoading={isAuctionsLoading}
                activeStatus={activeStatus}
                sortBy={sortBy}
                onStatusChange={handleStatusChange}
                onSortChange={handleSortChange}
                onPageChange={handlePageChange}
                onDeleteAuction={handleDeleteAuction}
                activeCount={activeCount}
                pendingCount={pendingCount}
                soldCount={soldCount}
                unsoldCount={unsoldCount}
              />
            )}

            {activeTab === "orders" && (
              <SellerOrdersTable
                orders={ordersTableResponse?.orders || []}
                totalCount={ordersTableResponse?.totalCount || 0}
                currentPage={orderPage}
                totalPages={Math.ceil((ordersTableResponse?.totalCount || 0) / 5) || 1}
                isLoading={isOrdersTableLoading}
                activeStatus={activeOrderStatus}
                sortBy={orderSortBy}
                onStatusChange={(status) => {
                  setActiveOrderStatus(status);
                  setOrderPage(1);
                }}
                onSortChange={(sort) => {
                  setOrderSortBy(sort);
                  setOrderPage(1);
                }}
                onPageChange={setOrderPage}
                allCount={allOrdersCount}
                pendingCount={pendingOrdersCount}
                shippedCount={shippedOrdersCount}
                deliveredCount={deliveredOrdersCount}
                completedCount={completedOrdersCount}
                canceledCount={canceledOrdersCount}
              />
            )}

            {activeTab === "financials" && (
              <SellerFinancialsTable
                orders={completedOrdersResponse?.orders || []}
                totalCount={completedOrdersResponse?.totalCount || financialsResponse?.completedOrdersCount || 0}
                currentPage={financialsPage}
                totalPages={Math.ceil((completedOrdersResponse?.totalCount || financialsResponse?.completedOrdersCount || 0) / 5) || 1}
                isLoading={isFinancialsLoading || isCompletedOrdersLoading}
                onPageChange={setFinancialsPage}
                grossRevenue={grossRevenue}
                platformFees={platformFees}
                netProfit={netProfit}
                completedCount={totalOrdersCount}
              />
            )}
          </div>
        </div>

      </DashboardShell>
    </PageWrapper>
  );
}
