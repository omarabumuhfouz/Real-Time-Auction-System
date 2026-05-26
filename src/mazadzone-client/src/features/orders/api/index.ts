// TanStack Query queries
export {
  ORDERS_KEYS,
  useGetMyOrders,
  useGetOrderDetails,
} from "./order.queries";

// TanStack Query mutations
export {
  useCompleteOrderPayment,
  useCreateOrder,
  useConfirmOrder,
  useShipOrder,
  useDeliverOrder,
  useCancelOrder,
} from "./order.mutations";

// Pure REST API methods
export {
  searchOrders,
  getOrderDetails,
  createOrder,
  payRemainingAmount,
  confirmOrder,
  shipOrder,
  deliverOrder,
  cancelOrder,
} from "./order.api";

// Pure mappers
export {
  mapOrderSummaryDtoToActivity,
  mapOrderDetailsDtoToActivity,
  mapCheckoutAddressToAddressDto,
} from "./order.mappers";
