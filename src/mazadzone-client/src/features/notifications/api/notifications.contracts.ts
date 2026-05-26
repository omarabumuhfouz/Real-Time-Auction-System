/**
 * Local contract representations of backend API contracts for the Notifications feature.
 */

export interface NotificationId {
  value?: string;
}

export interface NotificationDto {
  id: NotificationId;
  title: string;
  message: string;
  userId: { value?: string };
  createdOnUtc: string;
  isRead: boolean;
  modifiedOnUtc: string | null;
}

export interface PagedListOfNotificationDto {
  pageNumber: number;
  pageSize: number;
  totalPages?: number;
  totalCount: number;
  items: NotificationDto[];
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

export interface NotificationsListDto {
  notifications: PagedListOfNotificationDto;
}
