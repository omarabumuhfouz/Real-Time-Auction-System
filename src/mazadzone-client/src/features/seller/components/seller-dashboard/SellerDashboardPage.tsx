"use client";

import { useState } from "react";
import Link from "next/link";
import { useUrlFilters } from "@/hooks/use-url-filters";
import { LayoutGrid, Plus, Loader2, Download, Gavel, Box, DollarSign } from "lucide-react";
import { Button } from "@/components/ui/button";
import { PageWrapper } from "@/components/layout/page-wrapper";
import { ROUTES } from "@/config/routes.config";
import { useRequireRole } from "@/hooks/use-require-role";
import {
  useDeleteAuction,
  useGetSellerDashboardAuctions,
  useGetSellerDashboardFinancials,
  useGetSellerDashboardOrders,
} from "@/features/auctions";
import { SellerDashboardStats } from "./SellerDashboardStats";
import { SellerAuctionsTable } from "./auctions/SellerAuctionsTable";
import { SellerOrdersTable } from "./orders/SellerOrdersTable";
import { SellerFinancialsTable } from "./financials/SellerFinancialsTable";

export function SellerDashboardPage() {
  const { searchParams, setFilters } = useUrlFilters<{ status: string; sortBy: string; page: number }>();
  const [activeTab, setActiveTab] = useState<"auctions" | "orders" | "financials">("auctions");
  
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

  // 1. Fetch current page of seller auctions
  const {
    data: auctionsResponse,
    isLoading: isAuctionsLoading,
  } = useGetSellerDashboardAuctions({
    Page: tablePage,
    PageSize: 5,
    Status: activeStatus === "All" ? undefined : activeStatus,
    SortBy: sortBy === "EndTime" ? "endDateUtc" : sortBy === "CreationDate" ? "creationDate" : sortBy,
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

  // Aggregate stats metrics
  const activeCount = auctionsResponse?.activeAuctions || 0;
  const pendingCount = auctionsResponse?.pending || 0;
  const soldCount = auctionsResponse?.soldItems || 0;
  const unsoldCount = auctionsResponse?.unsold || 0;
  const totalOrdersCount = ordersResponse?.totalCount || financialsResponse?.completedOrdersCount || 0;
  const grossRevenue = financialsResponse?.totalGrossRevenue || 0;
  const platformFees = financialsResponse?.totalPlatformFees || 0;
  const netProfit = financialsResponse?.totalNetProfit || 0;

  // Auctions calculations
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

  const isStatsLoading = isAuctionsLoading || isFinancialsLoading || isOrdersLoading;

  return (
    <PageWrapper className="py-8 px-4 md:px-6 bg-background min-h-screen">
      <div className="max-w-[1600px] mx-auto w-full space-y-8 animate-fade-in">
        
        {/* Dashboard Header */}
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-6 pb-2">
          <div className="flex items-center gap-4 text-left">
            <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-2xl bg-white dark:bg-card border border-border shadow-xs">
              <LayoutGrid className="h-7 w-7 text-orange-500" />
            </div>
            <div>
              <h1 className="text-3xl font-black tracking-tight text-foreground md:text-4xl">
                Seller Dashboard
              </h1>
              <p className="text-sm text-muted-foreground font-medium mt-1">
                Manage your auctions, orders, and earnings in one place.
              </p>
            </div>
          </div>

          <div className="flex items-center gap-3 self-start md:self-auto">
            <Button
              variant="outline"
              className="bg-white hover:bg-slate-50 dark:bg-card dark:hover:bg-accent/15 rounded-xl h-11 px-5 font-bold flex items-center justify-center gap-2 border-border/80 shadow-xs cursor-pointer"
            >
              <Download className="h-4.5 w-4.5 text-muted-foreground" />
              Export All
            </Button>
            <Link href={ROUTES.SELLER.CREATE_AUCTION}>
              <Button className="bg-orange-500 hover:bg-orange-600 text-white font-bold h-11 px-5 rounded-xl flex items-center justify-center gap-2 shadow-sm cursor-pointer transition-all duration-200">
                <Plus className="h-5 w-5" />
                Create Auction
              </Button>
            </Link>
          </div>
        </div>

        {/* Section 1: Stats Grid */}
        <SellerDashboardStats
          activeAuctions={activeCount}
          pending={pendingCount}
          soldItems={soldCount}
          totalOrders={totalOrdersCount}
          grossRevenue={grossRevenue}
          netProfit={netProfit}
          isLoading={isStatsLoading}
        />

        {/* Section 2: Tab System Container */}
        <div className="space-y-0">
          {/* Custom Navigation Tab Headers */}
          <div className="grid grid-cols-3 bg-white dark:bg-card border border-border/80 rounded-t-2xl rounded-b-none overflow-hidden shadow-xs border-b border-b-border/60">
            {/* Auctions Tab */}
            <button
              onClick={() => setActiveTab("auctions")}
              className={`flex items-center justify-center gap-2 py-4 text-base font-bold transition-all relative border-r border-border/50 cursor-pointer ${
                activeTab === "auctions"
                  ? "text-orange-500 bg-orange-500/5 dark:bg-orange-500/10"
                  : "text-muted-foreground hover:text-orange-500 hover:bg-orange-500/5 dark:hover:bg-orange-500/10"
              }`}
            >
              <Gavel className="h-5 w-5" />
              <span>Auctions</span>
              {activeTab === "auctions" && (
                <div className="absolute bottom-0 left-0 right-0 h-[3px] bg-orange-500 z-10" />
              )}
            </button>

            {/* Orders Tab */}
            <button
              onClick={() => setActiveTab("orders")}
              className={`flex items-center justify-center gap-2 py-4 text-base font-bold transition-all relative border-r border-border/50 cursor-pointer ${
                activeTab === "orders"
                  ? "text-orange-500 bg-orange-500/5 dark:bg-orange-500/10"
                  : "text-muted-foreground hover:text-orange-500 hover:bg-orange-500/5 dark:hover:bg-orange-500/10"
              }`}
            >
              <Box className="h-5 w-5" />
              <span>Orders</span>
              {activeTab === "orders" && (
                <div className="absolute bottom-0 left-0 right-0 h-[3px] bg-orange-500 z-10" />
              )}
            </button>

            {/* Financials Tab */}
            <button
              onClick={() => setActiveTab("financials")}
              className={`flex items-center justify-center gap-2 py-4 text-base font-bold transition-all relative cursor-pointer ${
                activeTab === "financials"
                  ? "text-orange-500 bg-orange-500/5 dark:bg-orange-500/10"
                  : "text-muted-foreground hover:text-orange-500 hover:bg-orange-500/5 dark:hover:bg-orange-500/10"
              }`}
            >
              <DollarSign className="h-5 w-5" />
              <span>Financials</span>
              {activeTab === "financials" && (
                <div className="absolute bottom-0 left-0 right-0 h-[3px] bg-orange-500 z-10" />
              )}
            </button>
          </div>

          {/* Section 3: Tab Content Router */}
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
    </PageWrapper>
  );
}
