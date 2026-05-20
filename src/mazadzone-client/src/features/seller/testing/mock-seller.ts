import type { SellerProfile, SellerReview, ReviewReply } from "../types/seller.types";

export const MOCK_SELLER_PROFILE: SellerProfile = {
  id: "seller-123",
  fullName: "Mohammed Al-Rashid",
  email: "mohammed.alrashid@mazadzone.com",
  role: "seller",
  avatarUrl: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=facearea&facepad=2&w=256&h=256&q=80",
  avatarInitial: "MA",
  isVerified: true,
  rating: 4.8,
  reviewsCount: 7,
  memberSince: "Jan 2023",
  salesCount: 89,
  bio: "Trusted seller with a strong track record of quality products and excellent service.",
};

const INITIAL_SELLER_REVIEWS: SellerReview[] = [
  {
    id: "rev-1",
    reviewerName: "Sarah M.",
    reviewerInitial: "SM",
    rating: 5,
    comment: "Excellent seller! Item was exactly as described and shipped quickly. Would definitely buy again.",
    createdAt: "1 day ago",
    reply: {
      comment: "Thank you so much for your kind words! We're thrilled you're happy with your purchase. Enjoy your new Fortuner!",
      createdAt: "1 day ago",
    },
  },
  {
    id: "rev-2",
    reviewerName: "Khaled A.",
    reviewerInitial: "KA",
    rating: 5,
    comment: "Very professional communication. The camera lens was in pristine condition, well packaged.",
    createdAt: "2 days ago",
    reply: null,
  },
  {
    id: "rev-3",
    reviewerName: "Fatima H.",
    reviewerInitial: "FH",
    rating: 4,
    comment: "Great response time. The delivery was delayed by a day, but the item itself is perfect.",
    createdAt: "5 days ago",
    reply: {
      comment: "Thank you for your feedback! We apologize for the courier delay and are working on choosing faster delivery partners.",
      createdAt: "4 days ago",
    },
  },
  {
    id: "rev-4",
    reviewerName: "John D.",
    reviewerInitial: "JD",
    rating: 5,
    comment: "Highly recommended! Honest description and smooth transaction.",
    createdAt: "1 week ago",
    reply: null,
  },
  {
    id: "rev-5",
    reviewerName: "Omar F.",
    reviewerInitial: "OF",
    rating: 3,
    comment: "The laptop works fine but had a few more scratches than expected from the photos.",
    createdAt: "2 weeks ago",
    reply: {
      comment: "Hello Omar, we apologize for the oversight. We try to capture all details, and we'll take clearer closeups next time.",
      createdAt: "2 weeks ago",
    },
  },
  {
    id: "rev-6",
    reviewerName: "Layla S.",
    reviewerInitial: "LS",
    rating: 5,
    comment: "Perfect experience. Quick response to all my questions.",
    createdAt: "3 weeks ago",
    reply: null,
  },
  {
    id: "rev-7",
    reviewerName: "Zainab R.",
    reviewerInitial: "ZR",
    rating: 4,
    comment: "Good item and fair price. Recommended seller.",
    createdAt: "1 month ago",
    reply: null,
  },
];

// Mutable state to simulate backend updates
let mockReviews = [...INITIAL_SELLER_REVIEWS];

export function getMockSellerProfile(id: string): SellerProfile | undefined {
  if (id === MOCK_SELLER_PROFILE.id || id === "1") {
    return {
      ...MOCK_SELLER_PROFILE,
      id,
    };
  }
  
  // Fallback for other IDs so the page is resilient
  return {
    ...MOCK_SELLER_PROFILE,
    id,
    fullName: id.replace(/-/g, " ").replace(/\b\w/g, c => c.toUpperCase()),
    email: `${id}@mazadzone.com`,
    avatarInitial: id.substring(0, 2).toUpperCase(),
    isVerified: false,
    rating: 4.5,
    reviewsCount: 12,
    salesCount: 8,
    bio: "This is a newly registered seller on MazadZone.",
  };
}

export function getMockSellerReviews(sellerId: string): SellerReview[] {
  if (sellerId === MOCK_SELLER_PROFILE.id || sellerId === "1") {
    return mockReviews;
  }
  return [];
}

export function addMockReviewReply(reviewId: string, comment: string): ReviewReply | null {
  const review = mockReviews.find((r) => r.id === reviewId);
  if (!review) return null;

  const reply = {
    comment,
    createdAt: "Just now",
  };

  review.reply = reply;
  return reply;
}

export function resetMockReviews(): void {
  mockReviews = [...INITIAL_SELLER_REVIEWS];
}
