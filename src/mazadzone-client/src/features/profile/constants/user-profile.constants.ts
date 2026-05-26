import { Star, CheckCircle2, BadgeCheck, Shield, Calendar } from "lucide-react";
import type { PublicUserProfile } from "../types/user-profile.types";
import type { ComponentType } from "react";

export interface ProfileStatItem {
  key: string;
  label: string;
  value: string | number;
  icon: ComponentType<{ className?: string }>;
  iconClassName: string;
  textClassName: string;
  borderHoverClassName: string;
  valueClassName?: string;
}

/**
 * Returns the stat highlights configuration list dynamically based on user profile and role details.
 */
export const getProfileStatItems = (profile: PublicUserProfile): ProfileStatItem[] => {
  const isSeller = profile.roles.includes("Seller");

  const statItems: ProfileStatItem[] = isSeller && profile.sellerRating !== undefined
    ? [
        {
          key: "rating",
          label: "Rating",
          value: profile.sellerRating,
          icon: Star,
          iconClassName: "fill-amber-500 text-amber-500",
          textClassName: "text-amber-500",
          borderHoverClassName: "hover:border-amber-500/30",
        },
        {
          key: "reviews",
          label: "Reviews",
          value: profile.reviewsCount ?? 0,
          icon: CheckCircle2,
          iconClassName: "text-sky-500 fill-sky-50 dark:text-sky-400 dark:fill-sky-950",
          textClassName: "text-sky-600 dark:text-sky-400",
          borderHoverClassName: "hover:border-sky-500/30",
        },
        {
          key: "sales",
          label: "Sales",
          value: profile.salesCount ?? 0,
          icon: BadgeCheck,
          iconClassName: "text-primary",
          textClassName: "text-primary",
          borderHoverClassName: "hover:border-primary/30",
        },
      ]
    : [
        {
          key: "verified",
          label: "Verified",
          value: "Yes",
          icon: CheckCircle2,
          iconClassName: "fill-emerald-500 text-emerald-500",
          textClassName: "text-emerald-600 dark:text-emerald-400",
          borderHoverClassName: "hover:border-emerald-500/30",
        },
        {
          key: "status",
          label: "Status",
          value: "Active",
          icon: BadgeCheck,
          iconClassName: "text-sky-500",
          textClassName: "text-sky-600 dark:text-sky-400",
          borderHoverClassName: "hover:border-sky-500/30",
        },
        {
          key: "role",
          label: "Role",
          value: "Bidder",
          icon: Shield,
          iconClassName: "text-primary",
          textClassName: "text-primary",
          borderHoverClassName: "hover:border-primary/30",
        },
      ];

  return [
    ...statItems,
    {
      key: "joined",
      label: "Joined",
      value: profile.memberSince,
      icon: Calendar,
      iconClassName: "text-purple-500",
      textClassName: "text-purple-600 dark:text-purple-400",
      borderHoverClassName: "hover:border-purple-500/30",
      valueClassName: "text-[13px] leading-tight whitespace-nowrap",
    },
  ];
};
