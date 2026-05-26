import { useState } from "react";
import { Star, Download, ExternalLink, Quote, FileText, User, Store, X } from "lucide-react";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
  SheetClose,
} from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Checkbox } from "@/components/ui/checkbox";
import { Dialog, DialogContent, DialogTrigger, DialogTitle, DialogPortal, DialogOverlay } from "@/components/ui/dialog";
import * as DialogPrimitive from "@radix-ui/react-dialog";
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from "@/components/ui/carousel";
import { AdminDispute, AdminDisputeStatus } from "../types/admin-disputes.types";
import { useGetAuctionById } from "@/features/auctions/api/auction.queries";

interface ViewDisputeSheetProps {
  dispute: AdminDispute;
  isOpen: boolean;
  onClose: () => void;
}

export function ViewDisputeSheet({ dispute, isOpen, onClose }: ViewDisputeSheetProps) {
  const [resolutionStatus, setResolutionStatus] = useState<string>("");
  const [resolutionNote, setResolutionNote] = useState("");
  const [isUnderReview, setIsUnderReview] = useState(dispute.status === AdminDisputeStatus.UnderReview);

  // Fetch auction details
  const { data: auction } = useGetAuctionById(dispute.orderOrAuctionId);

  // Mock attachments since they aren't in the interface
  const attachments = [
    "https://images.unsplash.com/photo-1601524909162-71cff7dfdfb7?auto=format&fit=crop&w=800&q=80",
    "https://images.unsplash.com/photo-1512499617640-c74ae3a79d37?auto=format&fit=crop&w=800&q=80",
  ];

  const handleUnderReviewChange = (checked: boolean) => {
    setIsUnderReview(checked);
    if (checked) {
      setResolutionStatus("");
    }
  };

  const handleRadioChange = (val: string) => {
    setResolutionStatus(val);
    if (val === "resolved" || val === "rejected") {
      setIsUnderReview(false);
    }
  };

  const getStatusBadgeVariant = (status: AdminDisputeStatus | string) => {
    switch (status) {
      case AdminDisputeStatus.Open:
        return "info";
      case AdminDisputeStatus.UnderReview:
        return "review";
      case AdminDisputeStatus.AwaitingResponse:
        return "warning";
      case AdminDisputeStatus.Resolved:
        return "success";
      case AdminDisputeStatus.Rejected:
        return "destructive";
      default:
        return "outline";
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={(open) => !open && onClose()}>
      <SheetContent className="sm:max-w-6xl! w-[95vw]! overflow-y-auto p-0 flex flex-col bg-background">
        <div className="p-6 md:p-8 flex flex-col gap-6 w-full">
          {/* Header */}
          <SheetHeader className="flex flex-row items-start justify-between text-left space-y-0">
            <div className="flex flex-col gap-1.5">
              <SheetTitle className="text-2xl font-bold">View Dispute</SheetTitle>
              <SheetDescription className="text-muted-foreground font-medium">
                Review the case details and resolve the dispute.
              </SheetDescription>
            </div>
          </SheetHeader>

          {/* Top Banner */}
          <div className="grid grid-cols-2 md:grid-cols-3 gap-6 p-5 rounded-xl border border-border bg-card shadow-sm">
            <div className="flex flex-col gap-1.5">
              <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Dispute ID</span>
              <span className="font-bold text-foreground">{dispute.id}</span>
            </div>
            <div className="flex flex-col gap-1.5">
              <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Category</span>
              <span className="font-bold text-foreground">{dispute.category}</span>
            </div>
            <div className="flex flex-col gap-1.5 col-span-2 md:col-span-1 md:items-end">
              <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Current Status</span>
              <Badge variant={getStatusBadgeVariant(dispute.status) as any} className="w-fit text-sm px-2.5 py-0.5">
                {dispute.status}
              </Badge>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 items-start w-full">
            {/* Left Column */}
            <div className="flex flex-col gap-6 lg:col-span-2">
              {/* 1. Auction Details */}
              <div className="flex flex-col gap-3">
                <h3 className="font-bold text-foreground">1. Auction & Product Details</h3>
                <div className="border border-border rounded-xl p-5 bg-card shadow-sm flex flex-col sm:flex-row gap-5 items-start">
                  <div className="h-24 w-24 rounded-lg bg-muted border border-border overflow-hidden shrink-0 flex items-center justify-center">
                    {auction?.imageUrl ? (
                      <img src={auction.imageUrl} alt={auction.title} className="w-full h-full object-cover" />
                    ) : (
                      <div className="bg-muted-foreground/20 w-full h-full flex items-center justify-center">
                        <span className="text-xs font-semibold text-muted-foreground">Image</span>
                      </div>
                    )}
                  </div>
                  <div className="flex flex-col gap-3 flex-1 w-full">
                    <h4 className="font-bold text-lg text-foreground">
                      {auction ? auction.title : dispute.orderOrAuctionName}
                    </h4>
                    <div className="flex flex-col gap-1.5 w-full">
                      <div className="flex justify-between items-center w-full">
                        <span className="text-sm text-muted-foreground font-medium">Final Auction Price</span>
                        <span className="font-bold text-foreground">
                          ${auction ? (auction.pricing.currentBid ?? auction.pricing.startingPrice).toLocaleString() : "650.00"}
                        </span>
                      </div>
                      <div className="flex justify-between items-center w-full">
                        <span className="text-sm text-muted-foreground font-medium">Auction Ended</span>
                        <span className="font-medium text-foreground">
                          {auction ? new Date(auction.timing.endDate).toLocaleString() : "May 1, 2025 at 10:45 PM"}
                        </span>
                      </div>
                    </div>
                    <Button variant="outline" size="sm" className="w-fit mt-2 font-bold gap-2 bg-card hover:bg-muted">
                      View Auction
                      <ExternalLink className="size-4" />
                    </Button>
                  </div>
                </div>
              </div>

              {/* 2. Dispute Parties */}
              <div className="flex flex-col gap-3">
                <h3 className="font-bold text-foreground">2. Dispute Parties</h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 border border-border rounded-xl p-5 bg-card shadow-sm">
                  <div className="flex items-center justify-between md:pr-6 md:border-r md:border-border">
                    <div className="flex items-center gap-4">
                      <div className="h-12 w-12 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 shrink-0">
                        <User className="size-6" />
                      </div>
                      <div className="flex flex-col gap-0.5">
                        <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Buyer</span>
                        <span className="font-bold text-sm text-foreground">{dispute.parties.claimant}</span>
                        <span className="text-xs text-muted-foreground">buyer@example.com</span>
                      </div>
                    </div>
                    <div className="flex flex-col items-end gap-1">
                      <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Rating</span>
                      <div className="flex items-center gap-1 font-bold text-foreground bg-muted/50 px-2 py-1 rounded-md">
                        <Star className="size-3.5 fill-warning text-warning" />
                        4.6
                      </div>
                    </div>
                  </div>
                  <div className="flex items-center justify-between md:pl-2">
                    <div className="flex items-center gap-4">
                      <div className="h-12 w-12 rounded-full bg-green-100 flex items-center justify-center text-green-600 shrink-0">
                        <Store className="size-6" />
                      </div>
                      <div className="flex flex-col gap-0.5">
                        <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Seller</span>
                        <span className="font-bold text-sm text-foreground">{dispute.parties.respondent}</span>
                        <span className="text-xs text-muted-foreground">seller@example.com</span>
                      </div>
                    </div>
                    <div className="flex flex-col items-end gap-1">
                      <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Rating</span>
                      <div className="flex items-center gap-1 font-bold text-foreground bg-muted/50 px-2 py-1 rounded-md">
                        <Star className="size-3.5 fill-warning text-warning" />
                        4.2
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              {/* 3. Dispute Description */}
              <div className="flex flex-col gap-3">
                <h3 className="font-bold text-foreground">3. Dispute Description</h3>
                <div className="border border-border rounded-xl p-5 bg-card shadow-sm flex items-start gap-4">
                  <Quote className="size-6 text-muted-foreground shrink-0 rotate-180" />
                  <p className="text-sm font-medium leading-relaxed text-foreground">
                    The item received is not as described in the auction. The back cover has scratches and the battery health is only 85%, while the listing stated like new condition and 100% battery health. I would like a refund or partial compensation.
                  </p>
                </div>
              </div>

              {/* 4. Attachments */}
              <div className="flex flex-col gap-3">
                <h3 className="font-bold text-foreground">4. Attachments / Messages</h3>
                <div className="border border-border rounded-xl p-5 bg-card shadow-sm flex flex-col gap-4">
                  <div className="flex flex-wrap items-center gap-3">
                    <Dialog>
                      {attachments.map((imgUrl, idx) => (
                        <DialogTrigger key={idx} asChild>
                          <div className="h-24 w-24 rounded-lg bg-muted border border-border overflow-hidden cursor-pointer hover:opacity-80 transition-opacity">
                            <img src={imgUrl} alt={`Attachment ${idx + 1}`} className="w-full h-full object-cover" />
                          </div>
                        </DialogTrigger>
                      ))}
                      <DialogPortal>
                        <DialogOverlay className="bg-black/90 backdrop-blur-sm z-[100]" />
                        <DialogPrimitive.Content className="fixed left-1/2 top-1/2 z-[100] grid w-[100vw] max-w-none -translate-x-1/2 -translate-y-1/2 gap-4 outline-none data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 bg-transparent border-none shadow-none">
                          <DialogPrimitive.Close className="absolute right-6 top-6 z-[110] rounded-full bg-black/40 p-2.5 text-white/80 opacity-100 transition-all hover:bg-black/60 hover:text-white hover:scale-105 focus:outline-none focus:ring-2 focus:ring-white/50">
                            <X className="h-6 w-6" />
                            <span className="sr-only">Close</span>
                          </DialogPrimitive.Close>
                          <DialogTitle className="sr-only">Image Attachment Viewer</DialogTitle>
                          <Carousel className="w-[100vw] h-[100vh] flex items-center justify-center relative">
                            <CarouselContent className="ml-0 h-full w-full">
                              {attachments.map((imgUrl, idx) => (
                                <CarouselItem key={idx} className="pl-0 flex items-center justify-center w-full h-full relative p-4">
                                  <div className="relative w-full h-full flex items-center justify-center overflow-auto">
                                    <img src={imgUrl} alt={`Attachment Full ${idx + 1}`} className="w-auto h-auto max-w-full max-h-[90vh] object-contain drop-shadow-2xl" />
                                  </div>
                                </CarouselItem>
                              ))}
                            </CarouselContent>
                            <CarouselPrevious className="left-8 absolute h-12 w-12 border-transparent bg-white/10 hover:bg-white/20 text-white" />
                            <CarouselNext className="right-8 absolute h-12 w-12 border-transparent bg-white/10 hover:bg-white/20 text-white" />
                          </Carousel>
                        </DialogPrimitive.Content>
                      </DialogPortal>
                    </Dialog>
                  </div>
                </div>
              </div>
            </div>

            {/* Right Column */}
            <div className="flex flex-col gap-3 lg:col-span-1 sticky top-8">
              <h3 className="font-bold text-foreground">Resolve Dispute</h3>
              <div className="border border-border rounded-xl p-5 bg-card shadow-sm flex flex-col gap-6">

                {/* Status Selection */}
                <div className="flex flex-col gap-4">
                  <span className="text-sm font-bold text-foreground">
                    Status Selection <span className="text-destructive">*</span>
                  </span>

                  <div className="flex items-center space-x-2 border border-border p-3 rounded-lg bg-muted/10">
                    <Checkbox
                      id="under-review"
                      checked={isUnderReview}
                      onCheckedChange={handleUnderReviewChange}
                      className="data-[state=checked]:bg-primary data-[state=checked]:border-primary"
                    />
                    <Label htmlFor="under-review" className="text-sm font-semibold cursor-pointer">
                      Mark as Under Review
                    </Label>
                  </div>

                  <RadioGroup
                    value={resolutionStatus}
                    onValueChange={handleRadioChange}
                    className="flex flex-col gap-4"
                  >
                    <div className="flex items-start space-x-3">
                      <RadioGroupItem value="resolved" id="resolved" className="mt-1 text-primary border-muted-foreground data-[state=checked]:border-primary" />
                      <div className="flex flex-col gap-1">
                        <Label htmlFor="resolved" className="font-bold cursor-pointer">
                          Resolved (Accepted / Resolved)
                        </Label>
                        <span className="text-xs text-muted-foreground font-medium">
                          In favour of the complainant
                        </span>
                      </div>
                    </div>
                    <div className="flex items-start space-x-3">
                      <RadioGroupItem value="rejected" id="rejected" className="mt-1 text-primary border-muted-foreground data-[state=checked]:border-primary" />
                      <div className="flex flex-col gap-1">
                        <Label htmlFor="rejected" className="font-bold cursor-pointer">
                          Rejected
                        </Label>
                        <span className="text-xs text-muted-foreground font-medium">
                          Closed in favour of the other party
                        </span>
                      </div>
                    </div>
                  </RadioGroup>
                </div>

                {/* Resolution Note */}
                <div className="flex flex-col gap-2">
                  <span className="text-sm font-bold text-foreground">
                    Resolution Note <span className="text-destructive">*</span>
                  </span>
                  <Textarea
                    value={resolutionNote}
                    onChange={(e) => setResolutionNote(e.target.value)}
                    placeholder="Enter details about the resolution..."
                    className="min-h-[160px] bg-background border-input resize-none focus-visible:ring-primary focus-visible:border-primary"
                    maxLength={1000}
                  />
                  <div className="flex justify-end">
                    <span className="text-xs font-semibold text-muted-foreground">
                      Characters: {resolutionNote.length}/1000
                    </span>
                  </div>
                </div>

                {/* Actions */}
                <div className="flex items-center gap-3 mt-4">
                  <Button
                    variant="outline"
                    className="flex-1 font-bold bg-card hover:bg-muted h-11"
                    onClick={onClose}
                  >
                    Cancel
                  </Button>
                  <Button
                    className="flex-1 font-bold bg-primary text-primary-foreground hover:bg-primary/90 h-11"
                    onClick={onClose} // In a real app, this would submit the form
                  >
                    Confirm Resolution
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
}
