/**
 * Notification types for the in-app notification center.
 *
 * NOTE: `NotificationType` here refers to the domain event type
 * (what triggered the notification), not the feedback severity.
 * Feedback severity is handled by `FeedbackType` in `feedback.types.ts`.
 */

/**
 * Domain event type for a notification.
 *
 * Drives icon selection and visual categorisation in the notification center.
 * Backend should send one of these string values.
 */
export type NotificationType =
  // Bidding
  | "bid_accepted"
  | "bid_placed"
  | "outbid"
  // Auctions
  | "auction_ending"
  | "auction_won"
  | "auction_cancelled"
  // Orders
  | "order_shipped"
  | "order_received"
  // Payments
  | "payment_failed"
  | "payment_authorized"
  // Disputes
  | "dispute_opened"
  | "dispute_resolved"
  // Feedback
  | "feedback_received"
  // Account
  | "account_verified"
  | "seller_approved"
  // Messaging
  | "new_message"
  // Fallback
  | "general";

/**
 * A single notification from the backend API.
 *
 * Displayed in the notification center (bell popover).
 * This is the shape the backend returns via GET /notifications.
 */
export interface Notification {
  id: string;
  title: string;
  message: string;
  type: NotificationType;
  createdAt: string;
  isRead: boolean;
  /** Deep link to the relevant page (e.g. "/auctions/123"). */
  link?: string;
}

export interface NotificationResponse {
  items: Notification[];
  totalCount: number;
  pageSize: number;
  pageNumber: number;
  totalPages: number;
}
