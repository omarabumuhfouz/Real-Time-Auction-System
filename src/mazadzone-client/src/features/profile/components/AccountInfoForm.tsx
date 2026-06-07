"use client";

import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2, CheckCircle2, AlertTriangle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { profileSchema, type ProfileFormValues } from "../validations/profile.schemas";
import { useGetProfileSettings, useUpdateProfile } from "../api/profile.queries";
import type { UserProfile } from "../types/profile.types";
import { cn } from "@/lib/utils";
import { getAccountInfoFields } from "../constants/profile.constants";
import { useAuthStore } from "@/stores/auth.store";

interface AccountInfoFormProps {
  profile: UserProfile;
}

export function AccountInfoForm({ profile }: AccountInfoFormProps) {
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const { user } = useAuthStore();
  const { data: profileSettings } = useGetProfileSettings(user?.id || "");

  const updateMutation = useUpdateProfile();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isDirty },
  } = useForm<ProfileFormValues>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      fullName: profile.fullName,
      email: profile.email,
      phoneNumber: profile.phoneNumber ?? "",
    },
  });

  // Sync form values if profile fetched values change
  useEffect(() => {
    reset({
      fullName: profile.fullName,
      email: profile.email,
      phoneNumber: profile.phoneNumber ?? "",
    });
  }, [profile, reset]);

  const onSubmit = async (values: ProfileFormValues) => {
    setSuccessMessage(null);
    setErrorMessage(null);

    try {
      await updateMutation.mutateAsync({
        ...values,
      });
      setSuccessMessage("Account information saved successfully!");
      // Hide success message after 4 seconds
      setTimeout(() => setSuccessMessage(null), 4000);
    } catch (err) {
      setErrorMessage("Failed to update profile information. Please try again.");
    }
  };

  const handleCancel = () => {
    reset();
    setErrorMessage(null);
    setSuccessMessage(null);
  };

  const fields = getAccountInfoFields(updateMutation.isPending, profile);
  const displayFullName = profileSettings?.extractedFullName || profile.fullName;
  const avatarUrl = profile.avatarUrl ?? "";
  const avatarInitial = profile.avatarInitial ?? "A";

  return (
    <div className="rounded-xl border border-border bg-card p-6 shadow-xs">
      <h2 className="text-xl font-bold text-foreground mb-6">Account Information</h2>

      {/* Success/Error Alerts */}
      {successMessage && (
        <div className="mb-6 flex items-center gap-2.5 rounded-lg border border-success bg-success/5 p-4 text-sm text-foreground">
          <CheckCircle2 className="size-5 text-emerald-500 shrink-0" />
          <span className="font-medium text-emerald-800 dark:text-emerald-400">
            {successMessage}
          </span>
        </div>
      )}

      {errorMessage && (
        <div className="mb-6 flex items-center gap-2.5 rounded-lg border border-destructive bg-destructive/5 p-4 text-sm text-foreground">
          <AlertTriangle className="size-5 text-destructive shrink-0" />
          <span className="font-medium text-destructive-foreground">
            {errorMessage}
          </span>
        </div>
      )}

      {/* Avatar Container */}
      <div className="flex items-center gap-5 mb-8">
        <div className="flex size-20 items-center justify-center rounded-full bg-muted border border-border text-2xl font-bold text-muted-foreground overflow-hidden">
          {avatarUrl ? (
            // eslint-disable-next-line @next/next/no-img-element
            <img
              src={avatarUrl}
              alt="Profile Avatar"
              className="size-full object-cover"
            />
          ) : (
            avatarInitial
          )}
        </div>

        <div className="flex flex-col items-start gap-1">
          <span className="text-xl font-bold text-foreground">
            {displayFullName}
          </span>
        </div>
      </div>

      {/* Edit Form */}
      <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6">
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
          {fields.map((field) => (
            <div key={field.id} className="flex flex-col gap-1.5">
              <Label htmlFor={field.id} className="text-sm font-semibold text-foreground">
                {field.label}
              </Label>
              {field.name ? (
                <Input
                  id={field.id}
                  type={field.type ?? "text"}
                  placeholder={field.placeholder}
                  {...register(field.name)}
                  disabled={field.disabled}
                  readOnly={field.readOnly}
                  className={cn(
                    "border-input bg-card focus-visible:ring-primary focus-visible:border-primary",
                    field.readOnly && "bg-muted/50 text-muted-foreground",
                    field.type === "date" && "text-foreground"
                  )}
                />
              ) : (
                <Input
                  id={field.id}
                  value={field.value}
                  disabled={field.disabled}
                  className="border-input bg-muted/65 text-muted-foreground cursor-not-allowed select-none"
                />
              )}
              {field.name && errors[field.name] && (
                <span className="text-xs font-medium text-destructive">
                  {errors[field.name]?.message}
                </span>
              )}
              {field.hint && (
                <span className="text-xs text-muted-foreground font-medium">
                  {field.hint}
                </span>
              )}
            </div>
          ))}
        </div>

        {/* Form Submission Buttons */}
        <div className="flex items-center gap-3 mt-4">
          <Button
            type="submit"
            disabled={updateMutation.isPending}
            className="font-semibold px-6 py-2.5 h-auto text-sm cursor-pointer flex items-center justify-center gap-1.5"
          >
            {updateMutation.isPending && <Loader2 className="size-4 animate-spin" />}
            Save Changes
          </Button>
          <Button
            type="button"
            onClick={handleCancel}
            disabled={updateMutation.isPending}
            variant="outline"
            className="font-semibold px-6 py-2.5 h-auto text-sm cursor-pointer"
          >
            Cancel
          </Button>
        </div>
      </form>
    </div>
  );
}
