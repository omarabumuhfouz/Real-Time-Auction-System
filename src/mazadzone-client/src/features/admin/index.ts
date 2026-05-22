/**
 * Admin feature — public API.
 */

export { AdminLayout } from "./components/layout/AdminLayout";
export { AdminDashboardPage } from "./components/dashboard/AdminDashboardPage";
export { AdminHeader } from "./components/layout/AdminHeader";
export { DashboardOverviewSkeleton } from "./components/dashboard/skeletons";
export { ModerateUsersPage } from "./components/users/ModerateUsersPage";
export { ModerateAuctionsPage } from "./components/auctions/ModerateAuctionsPage";
export { useGetAdminOverviewStats, useGetAuctionActivityTrend, useGetUserGrowthTrend } from "./api/queries";
export { useSuspendUser, useBanUser, useRestoreUser } from "./api/user-mutations";
export { useCancelAuction, useForceEndAuction } from "./api/auction-mutations";
export type * from "./types/admin.types";


