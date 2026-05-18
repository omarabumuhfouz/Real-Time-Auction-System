import { Notification } from "../types/notification.types";

let mockNotifications: Notification[] = [
  {
    id: "notif-1",
    title: "Bid Leading!",
    message: "Your bid of 350 JD on 'iPhone 15 Pro Max' is currently the leading bid.",
    type: "bid_accepted",
    createdAt: new Date(Date.now() - 5 * 60 * 1000).toISOString(), // 5 mins ago
    isRead: false,
    link: "/auctions/auc-001",
  },
  {
    id: "notif-2",
    title: "New Message Received",
    message: "Seller John Doe sent you a message regarding 'MacBook Pro M3'.",
    type: "new_message",
    createdAt: new Date(Date.now() - 30 * 60 * 1000).toISOString(), // 30 mins ago
    isRead: false,
    link: "/profile",
  },
  {
    id: "notif-3",
    title: "Auction Ending Soon!",
    message: "The auction for 'Sony PlayStation 5' you bid on is ending in 15 minutes. Place a bid to stay in the lead!",
    type: "auction_ending",
    createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString(), // 2 hours ago
    isRead: true,
    link: "/auctions/auc-003",
  },
  {
    id: "notif-4",
    title: "Order Shipped!",
    message: "Your won item 'AirPods Pro 2' has been shipped by the seller. Track your order now.",
    type: "order_shipped",
    createdAt: new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString(), // 1 day ago
    isRead: true,
    link: "/orders",
  },
  {
    id: "notif-5",
    title: "Welcome to MazadZone!",
    message: "Start browsing live auctions, bid in real-time, and sell your own items today.",
    type: "general",
    createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000).toISOString(), // 3 days ago
    isRead: true,
  },
];

export const getMockNotifications = (): Notification[] => {
  return mockNotifications;
};

export const updateMockNotificationReadStatus = (id: string, isRead: boolean): void => {
  mockNotifications = mockNotifications.map((n) =>
    n.id === id ? { ...n, isRead } : n
  );
};

export const updateAllMockNotificationsReadStatus = (): void => {
  mockNotifications = mockNotifications.map((n) => ({ ...n, isRead: true }));
};
