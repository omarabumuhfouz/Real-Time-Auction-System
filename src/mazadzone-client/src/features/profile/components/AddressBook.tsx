"use client";

import { useState } from "react";
import { Pencil, MapPin, Loader2 } from "lucide-react";
import { AddressDialog } from "./AddressDialog";
import { Button } from "@/components/ui/button";
import {
  useGetAddresses,
  useUpdateAddress,
  useGetProfileSettings,
} from "../api/profile.queries";
import type { Address } from "../types/profile.types";
import { useAuthStore } from "@/stores/auth.store";

export function AddressBook() {
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [addressToEdit, setAddressToEdit] = useState<Address | null>(null);

  const { user } = useAuthStore();
  const { data: profileSettings } = useGetProfileSettings(user?.id || "");

  const { data: addresses = [], isLoading, isError, refetch } = useGetAddresses();
  const updateMutation = useUpdateAddress();

  const handleEditClick = (address: Address) => {
    setAddressToEdit(address);
    setIsDialogOpen(true);
  };

  const handleDialogClose = () => {
    setIsDialogOpen(false);
    setAddressToEdit(null);
  };

  const handleDialogSubmit = async (values: Omit<Address, "id">) => {
    try {
      if (addressToEdit) {
        await updateMutation.mutateAsync({
          id: addressToEdit.id,
          updates: values,
        });
      }
      handleDialogClose();
    } catch (err) {
      console.error("Failed to submit address:", err);
    }
  };

  if (isError) {
    return (
      <div className="rounded-xl border border-border bg-card p-6 shadow-xs text-center">
        <p className="text-sm text-destructive font-medium">Failed to load addresses.</p>
        <Button
          type="button"
          onClick={() => void refetch()}
          variant="link"
          className="mt-3 text-xs font-semibold text-primary hover:underline cursor-pointer h-auto p-0"
        >
          Try Again
        </Button>
      </div>
    );
  }

  const isPending = updateMutation.isPending;

  return (
    <div className="rounded-xl border border-border bg-card p-6 shadow-xs">
      {/* Header Row */}
      <div className="flex items-center justify-between border-b border-border/60 pb-5 mb-6">
        <h2 className="text-xl font-bold text-foreground">Address Book</h2>
      </div>

      {/* Address List */}
      {isLoading ? (
        <div className="flex flex-col items-center justify-center py-12 gap-3 text-muted-foreground">
          <Loader2 className="size-8 animate-spin text-primary" />
          <span className="text-sm font-medium">Loading addresses...</span>
        </div>
      ) : addresses.length === 0 ? (
        <div className="flex flex-col items-center justify-center py-12 text-center text-muted-foreground border border-dashed border-border rounded-lg bg-muted/10">
          <MapPin className="size-8 text-muted-foreground/50 mb-2" />
          <p className="text-sm font-medium">No registered address found.</p>
        </div>
      ) : (
        <div className="flex flex-col gap-4">
          {addresses.map((address) => (
            <div
              key={address.id}
              className="rounded-lg border border-border p-5 bg-card hover:shadow-xs transition-shadow flex flex-col gap-2"
            >
              {/* Name & Badge Row */}
              <div className="flex items-center justify-between">
                <span className="font-bold text-base text-foreground leading-tight">
                  {address.title}
                </span>
                {address.isDefault && (
                  <span className="bg-primary text-primary-foreground text-xs font-bold px-2.5 py-0.5 rounded-full">
                    Default
                  </span>
                )}
              </div>

              {/* User details (Full name & Phone) */}
              <div className="flex flex-col gap-0.5 mt-0.5">
                <span className="text-sm font-semibold text-foreground/90">
                  {profileSettings?.fullName ?? "Not provided"}
                </span>
                {profileSettings?.phoneNumber && (
                  <span className="text-xs text-muted-foreground">
                    {profileSettings.phoneNumber}
                  </span>
                )}
              </div>

              {/* Address Details */}
              <p className="text-sm leading-relaxed text-muted-foreground mt-0.5">
                {address.streetAddress}, {address.building}
                {address.landmark && `, ${address.landmark}`}
                {`, ${address.city}`}
              </p>

              {/* Action Buttons Row */}
              <div className="flex items-center gap-2 mt-4">
                <Button
                  type="button"
                  onClick={() => handleEditClick(address)}
                  variant="outline"
                  className="gap-1.5 px-3 py-1.5 h-auto text-xs font-bold"
                >
                  <Pencil className="size-3.5" />
                  <span>Edit</span>
                </Button>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Address Edit Modal */}
      <AddressDialog
        isOpen={isDialogOpen}
        onClose={handleDialogClose}
        addressToEdit={addressToEdit}
        onSubmit={handleDialogSubmit}
        isPending={isPending}
      />
    </div>
  );
}
