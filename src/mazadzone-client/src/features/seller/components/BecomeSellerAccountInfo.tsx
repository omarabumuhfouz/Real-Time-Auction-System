"use client";

import Link from "next/link";
import { User, Mail, Phone, MapPin, IdCard, Info, Check } from "lucide-react";
import { useAuthStore } from "@/stores/auth.store";
import { ROUTES } from "@/config/routes.config";
import { useGetProfileSettings } from "@/features/profile";

export function BecomeSellerAccountInfo() {
  const { user } = useAuthStore();
  const userId = user?.id;

  const { data: userInfo, isLoading, isError, refetch } = useGetProfileSettings(userId || "");

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between border-b border-border/40 pb-4">
          <div className="h-6 w-48 bg-muted animate-pulse rounded-md" />
          <div className="h-6 w-20 bg-muted animate-pulse rounded-full" />
        </div>
        <div className="border border-border/80 rounded-2xl overflow-hidden bg-muted/10 divide-y divide-border/60">
          {[1, 2, 3, 4, 5].map((i) => (
            <div key={i} className="flex flex-col sm:flex-row sm:items-center justify-between py-4 px-6 gap-2 sm:gap-4">
              <div className="flex items-center gap-3 w-48 shrink-0">
                <div className="h-5 w-5 bg-muted animate-pulse rounded-full" />
                <div className="h-4 w-28 bg-muted animate-pulse rounded-md" />
              </div>
              <div className="h-5 w-40 bg-muted animate-pulse rounded-md" />
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (isError || !userInfo) {
    return (
      <div className="space-y-6">
        {/* Title block */}
        <div className="flex items-center justify-between border-b border-border/40 pb-4">
          <h3 className="text-lg font-bold text-foreground tracking-tight">
            Your Account Information
          </h3>
        </div>
        {/* Custom modern error box */}
        <div className="border border-destructive/20 rounded-2xl p-6 bg-destructive/5 text-center space-y-4">
          <p className="text-sm font-semibold text-destructive">
            Failed to load your account information from the profile settings API.
          </p>
          <button
            type="button"
            onClick={() => void refetch()}
            className="px-5 py-2.5 bg-primary hover:bg-primary/95 text-white text-xs font-bold rounded-xl transition-all cursor-pointer shadow-sm"
          >
            Retry Loading
          </button>
        </div>
      </div>
    );
  }

  // Nicely formatted address using backend fields
  const addressFormatted = (
    <div className="text-left sm:text-right">
      <p className="font-semibold text-foreground">{userInfo.city || "Not provided"}</p>
      {(userInfo.street || userInfo.building || userInfo.landmark) && (
        <p className="text-xs text-muted-foreground font-medium mt-0.5">
          {[
            userInfo.street && `St. ${userInfo.street}`,
            userInfo.building && `Bldg. ${userInfo.building}`,
            userInfo.landmark && `(${userInfo.landmark})`,
          ]
            .filter(Boolean)
            .join(", ")}
        </p>
      )}
    </div>
  );

  // Dynamic values based on logged in user profile settings
  const accountDetails = [
    {
      id: "name",
      label: "Full Name",
      value: userInfo.fullName || user?.fullName || "Not provided",
      icon: <User className="h-5 w-5 text-muted-foreground shrink-0" />,
    },
    {
      id: "email",
      label: "Email Address",
      value: userInfo.email || user?.email || "Not provided",
      icon: <Mail className="h-5 w-5 text-muted-foreground shrink-0" />,
    },
    {
      id: "phone",
      label: "Phone Number",
      value: userInfo.phoneNumber || "Not provided",
      icon: <Phone className="h-5 w-5 text-muted-foreground shrink-0" />,
    },
    {
      id: "address",
      label: "Address",
      value: addressFormatted,
      icon: <MapPin className="h-5 w-5 text-muted-foreground shrink-0" />,
    },
    {
      id: "nationalId",
      label: "National ID",
      value: userInfo.nationalId || "Not provided",
      icon: <IdCard className="h-5 w-5 text-muted-foreground shrink-0" />,
    },
  ];

  return (
    <div className="space-y-6">
      {/* Title block with Verified badge */}
      <div className="flex items-center justify-between flex-wrap gap-4 border-b border-border/40 pb-4">
        <h3 className="text-lg font-bold text-foreground tracking-tight">
          Your Account Information
        </h3>
        <div className="flex items-center gap-1.5 bg-emerald-500/10 text-emerald-700 dark:text-emerald-400 px-3 py-1 rounded-full text-xs font-bold border border-emerald-500/20">
          <Check className="h-3.5 w-3.5 stroke-3" />
          Verified
        </div>
      </div>

      {/* Structured Read-only List Panel */}
      <div className="border border-border/80 rounded-2xl overflow-hidden bg-muted/10 divide-y divide-border/60">
        {accountDetails.map((detail) => (
          <div
            key={detail.id}
            className="flex flex-col sm:flex-row sm:items-center justify-between py-4 px-6 gap-2 sm:gap-4 transition-colors hover:bg-muted/20"
          >
            {/* Left Cell: Icon & Label */}
            <div className="flex items-center gap-3 w-48 shrink-0">
              {detail.icon}
              <span className="text-sm font-semibold text-muted-foreground">
                {detail.label}
              </span>
            </div>

            {/* Right Cell: Value */}
            <div className="text-left sm:text-right font-semibold text-foreground text-sm sm:text-[15px] break-all">
              {detail.value}
            </div>
          </div>
        ))}
      </div>

      {/* Warn Banner */}
      <div className="flex items-start gap-3 bg-accent/20 border border-primary/10 rounded-xl p-4 text-xs md:text-sm text-amber-800 dark:text-amber-300">
        <Info className="h-5 w-5 text-primary shrink-0 mt-0.5" />
        <p className="leading-relaxed font-medium">
          These details were provided during registration and cannot be edited here. You can update them from your{" "}
          <Link
            href={ROUTES.PROFILE.VIEW}
            className="text-primary hover:underline font-bold transition-all"
          >
            Profile Settings
          </Link>
          .
        </p>
      </div>
    </div>
  );
}

