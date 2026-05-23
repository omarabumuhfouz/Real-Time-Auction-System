"use client";

import { useState } from "react";
import { Loader2, Plus, MoreVertical, MapPin, AlertTriangle } from "lucide-react";
import { useGetAddresses, useCreateAddress, useGetProfile } from "../api/profile.queries";
import { AddressDialog } from "./AddressDialog";
import { type Address } from "../types/profile.types";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";

export interface AddressSelectStepProps {
  selectedAddressId?: string;
  onSelectAddress: (address: {
    id: string;
    label: string;
    fullName: string;
    phoneNumber: string;
    streetAddress: string;
    building: string;
    city: string;
    isDefault: boolean;
  }) => void;
  onCancel: () => void;
  title?: string;
  subtitle?: string;
}

export function AddressSelectStep({
  selectedAddressId,
  onSelectAddress,
  onCancel,
  title = "Choose Shipping Address",
  subtitle = "Select where you want your item to be shipped.",
}: AddressSelectStepProps) {
  const { data: profileAddresses = [], isLoading, isError } = useGetAddresses();
  const createMutation = useCreateAddress();
  
  const [localSelectedId, setLocalSelectedId] = useState<string>(selectedAddressId || "");
  const [isAddressDialogOpen, setIsAddressDialogOpen] = useState(false);

  const { data: profile } = useGetProfile();

  const mapAddress = (addr: Address) => ({
    id: addr.id,
    label: addr.title,
    fullName: profile?.fullName || "",
    phoneNumber: profile?.phoneNumber || "",
    streetAddress: addr.streetAddress,
    building: addr.building,
    city: addr.city,
    isDefault: addr.isDefault,
  });

  const handleUseAddress = () => {
    const matched = profileAddresses.find((a) => a.id === localSelectedId);
    if (matched) {
      onSelectAddress(mapAddress(matched));
    }
  };

  const handleAddNewAddressSubmit = async (values: Omit<Address, "id">) => {
    try {
      const newAddress = await createMutation.mutateAsync(values);
      setLocalSelectedId(newAddress.id);
      setIsAddressDialogOpen(false);
    } catch (err) {
      console.error("Failed to add address in shared select flow:", err);
    }
  };

  if (isError) {
    return (
      <div className="space-y-4 text-center py-6">
        <div className="h-12 w-12 rounded-full bg-red-100 dark:bg-red-950/20 text-red-500 flex items-center justify-center mx-auto">
          <AlertTriangle className="h-6 w-6" />
        </div>
        <h4 className="font-bold text-foreground">Failed to Load Addresses</h4>
        <p className="text-xs text-muted-foreground">
          There was an error retrieving your address list. Please try again.
        </p>
        <button
          type="button"
          onClick={onCancel}
          className="text-sm font-bold text-primary hover:underline cursor-pointer"
        >
          Go Back
        </button>
      </div>
    );
  }

  return (
    <div className="space-y-6 text-left">
      {/* Header */}
      <div>
        <h3 className="text-xl font-bold text-foreground">{title}</h3>
        <p className="text-sm text-muted-foreground mt-1">{subtitle}</p>
      </div>

      {/* Address List Section */}
      {isLoading ? (
        <div className="space-y-3 py-6">
          {[1, 2].map((i) => (
            <div
              key={i}
              className="h-24 rounded-xl border border-border bg-muted/20 animate-pulse flex items-center p-4 gap-4"
            >
              <div className="h-5 w-5 rounded-full bg-muted animate-pulse" />
              <div className="space-y-2 flex-1">
                <div className="h-4 w-24 bg-muted rounded animate-pulse" />
                <div className="h-3 w-48 bg-muted rounded animate-pulse" />
              </div>
            </div>
          ))}
        </div>
      ) : profileAddresses.length === 0 ? (
        <div className="flex flex-col items-center justify-center py-10 text-center border border-dashed border-border rounded-xl bg-muted/10 space-y-3">
          <MapPin className="h-8 w-8 text-muted-foreground/60" />
          <div>
            <p className="text-sm font-bold text-foreground">No saved addresses</p>
            <p className="text-xs text-muted-foreground max-w-[200px] mx-auto mt-1">
              Add a shipping address to complete your order setup.
            </p>
          </div>
          <button
            type="button"
            onClick={() => setIsAddressDialogOpen(true)}
            className="flex items-center gap-1 text-xs font-bold text-primary hover:underline cursor-pointer"
          >
            <Plus className="h-3.5 w-3.5" /> Add Address
          </button>
        </div>
      ) : (
        <RadioGroup
          value={localSelectedId}
          onValueChange={setLocalSelectedId}
          className="grid gap-3 max-h-[300px] overflow-y-auto pr-1"
        >
          {profileAddresses.map((addr) => {
            const isSelected = localSelectedId === addr.id;
            return (
              <div
                key={addr.id}
                onClick={() => setLocalSelectedId(addr.id)}
                className={`flex items-start justify-between p-4 rounded-xl border transition-all cursor-pointer ${
                  isSelected
                    ? "border-primary bg-primary/5"
                    : "border-border hover:bg-muted/10"
                }`}
              >
                <div className="flex items-start gap-3">
                  <RadioGroupItem
                    value={addr.id}
                    id={`addr-${addr.id}`}
                    className="mt-1 border-muted-foreground data-[state=checked]:border-primary data-[state=checked]:text-primary"
                  />
                  <div className="space-y-0.5 text-xs md:text-sm">
                    <Label
                      htmlFor={`addr-${addr.id}`}
                      className="font-bold text-foreground cursor-pointer flex items-center gap-2"
                    >
                      {addr.title}
                      {addr.isDefault && (
                        <span className="text-[9px] font-black uppercase tracking-wider bg-primary/10 text-primary px-1.5 py-0.5 rounded border border-primary/20">
                          Default
                        </span>
                      )}
                    </Label>
                    <p className="text-muted-foreground mt-0.5">
                      {addr.building}, {addr.streetAddress}, {addr.city}
                    </p>
                    {profile?.phoneNumber && (
                      <p className="text-muted-foreground text-xs">{profile.phoneNumber}</p>
                    )}
                  </div>
                </div>
                <button
                  type="button"
                  className="text-muted-foreground hover:text-foreground p-1 transition-colors rounded-full hover:bg-muted"
                  onClick={(e) => {
                    e.stopPropagation();
                  }}
                >
                  <MoreVertical className="h-4 w-4" />
                </button>
              </div>
            );
          })}
        </RadioGroup>
      )}

      {/* Add New Link at the bottom */}
      {!isLoading && profileAddresses.length > 0 && (
        <button
          type="button"
          onClick={() => setIsAddressDialogOpen(true)}
          className="flex items-center gap-1.5 text-sm font-bold text-primary hover:underline cursor-pointer mt-1"
        >
          <Plus className="h-4 w-4 stroke-[2.5]" />
          Add new address
        </button>
      )}

      {/* Action Buttons */}
      <div className="pt-4 space-y-3 border-t border-border/40">
        <Button
          type="button"
          onClick={handleUseAddress}
          disabled={!localSelectedId}
          className="w-full h-12 rounded-xl text-base font-extrabold"
        >
          Use Selected Address
        </Button>
        <button
          type="button"
          onClick={onCancel}
          className="w-full text-center text-sm font-semibold text-muted-foreground hover:text-foreground cursor-pointer transition-colors"
        >
          Cancel
        </button>
      </div>

      <AddressDialog
        isOpen={isAddressDialogOpen}
        onClose={() => setIsAddressDialogOpen(false)}
        onSubmit={handleAddNewAddressSubmit}
        isPending={createMutation.isPending}
      />
    </div>
  );
}
