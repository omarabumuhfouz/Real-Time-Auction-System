import { useQuery } from "@tanstack/react-query";
import type { PublicUserProfile } from "../types/user-profile.types";

const MOCK_DELAY_MS = 300;
const simulateDelay = () => new Promise((resolve) => setTimeout(resolve, MOCK_DELAY_MS));

// Mock public profiles for different roles
const MOCK_PROFILES: Record<string, PublicUserProfile> = {
  "seller-123": {
    id: "seller-123",
    fullName: "Mohammed Al-Rashid",
    email: "mohammed.alrashid@mazadzone.com",
    phoneNumber: "+966 50 123 4567",
    avatarUrl: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=facearea&facepad=2&w=256&h=256&q=80",
    avatarInitial: "MA",
    roles: ["Bidder", "Seller"],
    isVerified: true,
    memberSince: "Jan 2023",
    status: "Active",
    bio: "Trusted seller with a strong track record of quality products and excellent service.",
    biddingActivityCount: 12,
    bidsPlacedCount: 45,
    wonAuctionsCount: 3,
    completedPurchasesCount: 3,
    sellerRating: 4.8,
    reviewsCount: 7,
    salesCount: 89,
  },
  "1": {
    id: "1",
    fullName: "Mohammed Al-Rashid",
    email: "mohammed.alrashid@mazadzone.com",
    phoneNumber: "+966 50 123 4567",
    avatarUrl: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=facearea&facepad=2&w=256&h=256&q=80",
    avatarInitial: "MA",
    roles: ["Bidder", "Seller"],
    isVerified: true,
    memberSince: "Jan 2023",
    status: "Active",
    bio: "Trusted seller with a strong track record of quality products and excellent service.",
    biddingActivityCount: 12,
    bidsPlacedCount: 45,
    wonAuctionsCount: 3,
    completedPurchasesCount: 3,
    sellerRating: 4.8,
    reviewsCount: 7,
    salesCount: 89,
  },
  "bidder-456": {
    id: "bidder-456",
    fullName: "Sarah Mansour",
    email: "sarah.mansour@gmail.com",
    phoneNumber: "+966 55 987 6543",
    avatarUrl: null,
    avatarInitial: "SM",
    roles: ["Bidder"],
    isVerified: true,
    memberSince: "Mar 2024",
    status: "Active",
    bio: "Passionate bidder interested in tech gadgets and vintage art items.",
    biddingActivityCount: 8,
    bidsPlacedCount: 18,
    wonAuctionsCount: 2,
    completedPurchasesCount: 2,
  },
  "2": {
    id: "2",
    fullName: "Sarah Mansour",
    email: "sarah.mansour@gmail.com",
    phoneNumber: "+966 55 987 6543",
    avatarUrl: null,
    avatarInitial: "SM",
    roles: ["Bidder"],
    isVerified: true,
    memberSince: "Mar 2024",
    status: "Active",
    bio: "Passionate bidder interested in tech gadgets and vintage art items.",
    biddingActivityCount: 8,
    bidsPlacedCount: 18,
    wonAuctionsCount: 2,
    completedPurchasesCount: 2,
  },
  "admin-789": {
    id: "admin-789",
    fullName: "Khaled Al-Otaibi",
    email: "khaled.admin@mazadzone.com",
    phoneNumber: "+966 56 321 7654",
    avatarUrl: null,
    avatarInitial: "KA",
    roles: ["Bidder", "Seller", "Admin"],
    isVerified: true,
    memberSince: "Jun 2022",
    status: "Active",
    bio: "Official MazadZone Administrator. Happy bidding!",
    biddingActivityCount: 42,
    bidsPlacedCount: 154,
    wonAuctionsCount: 12,
    completedPurchasesCount: 10,
    sellerRating: 5.0,
    reviewsCount: 3,
    salesCount: 15,
  },
};

/**
 * Fetches the public user profile details for a given userId.
 * Falls back to dynamically generating bidder or seller profiles for any undefined IDs.
 */
export async function getPublicUserProfile(userId: string): Promise<PublicUserProfile> {
  await simulateDelay();

  /*
  // --- REAL API CALL (Uncomment when backend is ready) ---
  const response = await api.get<PublicUserProfile>(`/users/${userId}/public-profile`);
  return response.data;
  */

  const profile = MOCK_PROFILES[userId];
  if (profile) {
    return profile;
  }

  // Fallback generation for other IDs
  const isSeller = userId.includes("seller");
  const isAdmin = userId.includes("admin");
  const name = userId
    .replace(/^(user-|bidder-|reviewer-|seller-|admin-)/, "")
    .replace(/-/g, " ")
    .replace(/\b\w/g, (c) => c.toUpperCase());

  const biddingActivity = 5;
  const bidsPlaced = 12;

  return {
    id: userId,
    fullName: name || "Anonymous User",
    email: `${userId}@mazadzone.com`,
    phoneNumber: "+966 50 000 0000",
    avatarUrl: null,
    avatarInitial: (name || "AU")
      .split(" ")
      .map((n) => n[0])
      .join("")
      .substring(0, 2)
      .toUpperCase(),
    roles: isSeller ? ["Bidder", "Seller"] : isAdmin ? ["Bidder", "Seller", "Admin"] : ["Bidder"],
    isVerified: false,
    memberSince: "Feb 2024",
    status: "Active",
    bio: `A registered ${isSeller ? "seller" : isAdmin ? "administrator" : "bidder"} on MazadZone.`,
    biddingActivityCount: biddingActivity,
    bidsPlacedCount: bidsPlaced,
    wonAuctionsCount: 1,
    completedPurchasesCount: 1,
    sellerRating: isSeller || isAdmin ? 4.5 : undefined,
    reviewsCount: isSeller || isAdmin ? 3 : undefined,
    salesCount: isSeller || isAdmin ? 5 : undefined,
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
