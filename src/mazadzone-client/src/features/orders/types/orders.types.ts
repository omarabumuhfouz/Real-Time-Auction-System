export type OrderStatus = "Pending" | "Confirmed" | "Processing" | "Shipped" | "Delivered" | "Cancelled";

export interface OrderActivity {
  id: string;
  orderNumber: string;
  status: OrderStatus;
  deliveryStatus: string;
  finalBid: number;
  date: string;
  auction: {
    id: string;
    title: string;
    imageUrl: string;
  };
}
