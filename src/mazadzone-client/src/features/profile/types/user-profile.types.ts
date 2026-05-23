export type UserRole = "Bidder" | "Seller" | "Admin";

export interface PublicUserProfile {
  id: string;
  fullName: string;
  email: string;
  phoneNumber?: string;
  avatarUrl: string | null;
  avatarInitial: string;
  roles: UserRole[];
  isVerified: boolean;
  memberSince: string;
  status?: string;
  bio?: string;
  
  // Bidder-only / Shared stats
  biddingActivityCount: number;
  bidsPlacedCount: number;
  wonAuctionsCount: number;
  completedPurchasesCount: number;

  // Seller-specific stats (if "Seller" is in roles)
  sellerRating?: number;
  reviewsCount?: number;
  salesCount?: number;
}
