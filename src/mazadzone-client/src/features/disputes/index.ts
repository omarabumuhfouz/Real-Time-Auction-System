/**
 * Disputes feature — public API.
 *
 * TODO: Implement dispute filing, dispute detail view,
 * admin resolution interface, and dispute API hooks.
 */

export { DisputeDialog } from "./components/DisputeDialog";
export { useFileDispute } from "./api";
export * from "./types/disputes.types";
export * from "./validations/disputes.schemas";

// Admin
export { AdminDisputesPage } from "./components/AdminDisputesPage";
export * from "./types/admin-disputes.types";
export { useGetAdminDisputes } from "./api/use-get-admin-disputes";
