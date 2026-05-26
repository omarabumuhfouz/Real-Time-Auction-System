import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import type { PublicUserProfile, UserRole } from "../types/user-profile.types";
import type { BidderProfileDto } from "./profile.contracts";
import type { PublicSellerProfileResponse } from "@/features/seller/api/seller.contracts";

/**
 * Fetches the public user profile details for a given userId.
 */
export async function getPublicUserProfile(userId: string): Promise<PublicUserProfile> {
  const bidderRes = await api.get<BidderProfileDto>(`/api/v1/bidders/${userId}`);
  const bidder = bidderRes.data;

  let seller: PublicSellerProfileResponse | null = null;
  try {
    const sellerRes = await api.get<PublicSellerProfileResponse>(`/api/v1/sellers/${userId}/public`);
    seller = sellerRes.data;
  } catch {
    // If the seller API returns 404, it means the user is not a seller, which is fine.
  }

  const roles: UserRole[] = ["Bidder"];
  if (seller) {
    roles.push("Seller");
  }

  return {
    id: bidder.id,
    fullName: bidder.fullName,
    email: bidder.email,
    phoneNumber: bidder.phoneNumber,
    avatarUrl: null,
    avatarInitial: (bidder.fullName || "AU")
      .split(" ")
      .map((n) => n[0])
      .join("")
      .substring(0, 2)
      .toUpperCase(),
    roles,
    isVerified: seller ? seller.isVerified : true, // Bidders are generally verified by default or we can leave true
    memberSince: new Date(bidder.memberSince || new Date()).toLocaleDateString("en-US", { year: "numeric", month: "short" }),
    status: "Active",
    bio: seller 
      ? `Active MazadZone registered seller since ${new Date(seller.joinedOnUtc).toLocaleDateString()}.` 
      : `A registered bidder on MazadZone.`,
    biddingActivityCount: bidder.totalBidsPlaced || 0,
    bidsPlacedCount: bidder.totalBidsPlaced || 0,
    wonAuctionsCount: 0,
    completedPurchasesCount: 0,
    sellerRating: seller?.rating,
    reviewsCount: seller?.reviewsCount,
    salesCount: 0, 
  };
}

/**
 * React Query hook to get public user profile by userId.
 */
export function useGetPublicUserProfile(userId: string) {
  return useQuery<PublicUserProfile>({
    queryKey: ["public-profile", userId],
    queryFn: () => getPublicUserProfile(userId),
    enabled: !!userId,
  });
}
