import { CheckCircle2, Star, ThumbsUp, ShoppingBag, Calendar } from "lucide-react";
import type { SellerProfile } from "../types/seller.types";

interface SellerProfileHeaderProps {
  profile: SellerProfile;
}

export function SellerProfileHeader({ profile }: SellerProfileHeaderProps) {
  // Render stars helper
  const renderStars = (rating: number) => {
    const fullStars = Math.floor(rating);
    const hasHalf = rating % 1 !== 0;
    return (
      <div className="flex items-center gap-0.5">
        {Array.from({ length: 5 }).map((_, i) => (
          <Star
            key={i}
            className={`size-4 ${
              i < fullStars
                ? "fill-yellow-400 text-yellow-400"
                : i === fullStars && hasHalf
                ? "fill-yellow-400 text-yellow-400 opacity-50 text-yellow-400"
                : "text-muted-foreground/30"
            }`}
          />
        ))}
      </div>
    );
  };

  return (
    <div className="rounded-xl border border-border bg-card p-6 shadow-xs transition-shadow hover:shadow-sm">
      <div className="flex flex-col gap-6 lg:flex-row lg:items-start lg:justify-between">
        {/* Left Side: Avatar & Core Information */}
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start">
          <div className="relative size-24 shrink-0 overflow-hidden rounded-full border border-border bg-muted ring-4 ring-border/20">
            {profile.avatarUrl ? (
              // eslint-disable-next-line @next/next/no-img-element
              <img
                src={profile.avatarUrl}
                alt={profile.fullName}
                className="h-full w-full object-cover"
              />
            ) : (
              <div className="flex h-full w-full items-center justify-center bg-primary/10 text-xl font-bold text-primary">
                {profile.avatarInitial}
              </div>
            )}
          </div>

          <div className="flex flex-col gap-2">
            <div className="flex flex-wrap items-center gap-2.5">
              <h1 className="text-2xl font-bold text-foreground tracking-tight">
                {profile.fullName}
              </h1>
              {profile.isVerified && (
                <span className="inline-flex items-center gap-1 rounded-full border border-primary/20 bg-primary/10 px-2.5 py-0.5 text-[11px] font-bold text-primary">
                  <CheckCircle2 className="size-3.5 fill-primary text-primary-foreground" />
                  Verified
                </span>
              )}
            </div>

            {/* Ratings & Quick metadata list */}
            <div className="flex flex-wrap items-center gap-y-1.5 gap-x-3 text-sm text-muted-foreground">
              <div className="flex items-center gap-1.5">
                {renderStars(profile.rating)}
                <span className="font-bold text-foreground">{profile.rating}</span>
                <span className="text-muted-foreground">
                  ({profile.reviewsCount} reviews)
                </span>
              </div>
              <span className="text-muted-foreground/30">|</span>
              <div className="flex items-center gap-1">
                <Calendar className="size-3.5" />
                <span>Member since {profile.memberSince}</span>
              </div>
              <span className="text-muted-foreground/30">|</span>
              <div className="flex items-center gap-1">
                <ShoppingBag className="size-3.5" />
                <span>{profile.salesCount} sales</span>
              </div>
            </div>

            <p className="mt-2 max-w-xl text-sm leading-relaxed text-muted-foreground">
              {profile.bio}
            </p>
          </div>
        </div>

        {/* Right Side: Dashboard stat block boxes */}
        <div className="grid grid-cols-2 gap-3 sm:grid-cols-4 lg:shrink-0">
          {/* Rating Block */}
          <div className="flex flex-col items-center justify-center rounded-xl border border-border bg-card p-4 text-center min-w-[110px] shadow-xs">
            <div className="flex items-center gap-1.5 text-emerald-600 font-bold text-lg">
              <CheckCircle2 className="size-4 shrink-0 text-emerald-500 fill-emerald-50" />
              <span>{profile.rating}</span>
            </div>
            <span className="mt-1 text-xs text-muted-foreground font-medium">Seller Rating</span>
          </div>

          {/* Reviews Block */}
          <div className="flex flex-col items-center justify-center rounded-xl border border-border bg-card p-4 text-center min-w-[110px] shadow-xs">
            <div className="flex items-center gap-1.5 text-sky-600 font-bold text-lg">
              <ThumbsUp className="size-4 shrink-0 text-sky-500" />
              <span>{profile.reviewsCount}</span>
            </div>
            <span className="mt-1 text-xs text-muted-foreground font-medium">Reviews</span>
          </div>

          {/* Sales Block */}
          <div className="flex flex-col items-center justify-center rounded-xl border border-border bg-card p-4 text-center min-w-[110px] shadow-xs">
            <div className="flex items-center gap-1.5 text-primary font-bold text-lg">
              <ShoppingBag className="size-4 shrink-0 text-primary" />
              <span>{profile.salesCount}</span>
            </div>
            <span className="mt-1 text-xs text-muted-foreground font-medium">Sales</span>
          </div>

          {/* Member Since Block */}
          <div className="flex flex-col items-center justify-center rounded-xl border border-border bg-card p-4 text-center min-w-[110px] shadow-xs">
            <div className="flex items-center gap-1.5 text-purple-600 font-bold text-lg">
              <Calendar className="size-4 shrink-0 text-purple-500" />
              <span className="text-[13px] leading-tight whitespace-nowrap">{profile.memberSince}</span>
            </div>
            <span className="mt-1 text-xs text-muted-foreground font-medium">Member Since</span>
          </div>
        </div>
      </div>
    </div>
  );
}
