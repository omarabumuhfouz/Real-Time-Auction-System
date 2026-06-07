"use client";

import { useState, useEffect, useMemo, useRef } from "react";
import { useRouter } from "next/navigation";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { VisuallyHidden } from "radix-ui";
import { Button } from "@/components/ui/button";
import { Trophy, Sparkles, ShoppingBag, CreditCard, Loader2, RefreshCw, CheckCircle2 } from "lucide-react";
import { formatCurrency } from "@/utils/currency.utils";
import { useGetMyOrders, CompletePaymentModal } from "@/features/orders";
import { useWinDialogStore } from "../store/win-dialog.store";
import { ROUTES } from "@/config/routes.config";
import { cn } from "@/lib/utils";

// ---------------------------------------------------------------------------
// Canvas Confetti Celebration Component
// ---------------------------------------------------------------------------
export function CelebrationConfetti() {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    let animationFrameId: number;
    let width = (canvas.width = canvas.offsetWidth || window.innerWidth);
    let height = (canvas.height = canvas.offsetHeight || window.innerHeight);

    const handleResize = () => {
      if (!canvas) return;
      width = canvas.width = canvas.offsetWidth || window.innerWidth;
      height = canvas.height = canvas.offsetHeight || window.innerHeight;
    };
    window.addEventListener("resize", handleResize);

    const colors = [
      "#FBBF24", // amber/gold
      "#F472B6", // hot pink
      "#22D3EE", // cyan
      "#F97316", // orange
      "#4ADE80", // green
      "#A78BFA", // violet
    ];

    interface Particle {
      x: number;
      y: number;
      size: number;
      color: string;
      speedX: number;
      speedY: number;
      rotation: number;
      rotationSpeed: number;
      opacity: number;
    }

    const particles: Particle[] = Array.from({ length: 100 }).map(() => ({
      x: Math.random() * width,
      y: Math.random() * -height - 20,
      size: Math.random() * 8 + 6,
      color: colors[Math.floor(Math.random() * colors.length)],
      speedX: Math.random() * 4 - 2,
      speedY: Math.random() * 4 + 3,
      rotation: Math.random() * 360,
      rotationSpeed: Math.random() * 6 - 3,
      opacity: 1,
    }));

    const startTime = Date.now();

    const animate = () => {
      ctx.clearRect(0, 0, width, height);

      const elapsed = Date.now() - startTime;
      const fadeProgress = elapsed > 3500 ? Math.max(0, 1 - (elapsed - 3500) / 1000) : 1;

      if (fadeProgress === 0) {
        return;
      }

      particles.forEach((p) => {
        p.y += p.speedY;
        p.x += p.speedX + Math.sin(p.y / 30) * 0.5;
        p.rotation += p.rotationSpeed;

        if (p.y > height) {
          if (elapsed < 3000) {
            p.y = -20;
            p.x = Math.random() * width;
          }
        }

        ctx.save();
        ctx.translate(p.x, p.y);
        ctx.rotate((p.rotation * Math.PI) / 180);
        ctx.fillStyle = p.color;
        ctx.globalAlpha = p.opacity * fadeProgress;
        
        if (p.size % 2 === 0) {
          ctx.fillRect(-p.size / 2, -p.size / 2, p.size, p.size * 1.5);
        } else {
          ctx.beginPath();
          ctx.arc(0, 0, p.size / 2, 0, Math.PI * 2);
          ctx.fill();
        }
        ctx.restore();
      });

      animationFrameId = requestAnimationFrame(animate);
    };

    animate();

    return () => {
      window.removeEventListener("resize", handleResize);
      cancelAnimationFrame(animationFrameId);
    };
  }, []);

  return (
    <canvas
      ref={canvasRef}
      className="absolute inset-0 pointer-events-none w-full h-full z-50"
    />
  );
}

// ---------------------------------------------------------------------------
// Main Win Celebration Dialog Component
// ---------------------------------------------------------------------------
export function WinDialog() {
  const router = useRouter();
  const { isOpen, data, closeWinDialog } = useWinDialogStore();
  const [isPaymentOpen, setIsPaymentOpen] = useState(false);
  const [retryCount, setRetryCount] = useState(0);

  // Retrieve won orders list to lookup matching order
  const {
    data: wonOrdersResponse,
    isLoading: isOrdersLoading,
    isError,
    refetch,
  } = useGetMyOrders(
    { pageSize: 50 },
    { enabled: isOpen && !!data?.auctionId }
  );

  // Match current won auction with its corresponding pending order
  const matchingOrder = useMemo(() => {
    if (!data?.auctionId || !wonOrdersResponse?.items) return null;
    return wonOrdersResponse.items.find(
      (item) => item.auction.id === data.auctionId
    );
  }, [wonOrdersResponse, data?.auctionId]);

  // Automated background retry mechanism to account for server delays in order creation
  useEffect(() => {
    if (!isOpen) {
      setRetryCount(0);
      return;
    }

    if (data?.auctionId && !isOrdersLoading && !matchingOrder && retryCount < 4) {
      const timer = setTimeout(() => {
        setRetryCount((prev) => prev + 1);
        void refetch();
      }, 3000);
      return () => clearTimeout(timer);
    }
  }, [isOpen, data?.auctionId, isOrdersLoading, matchingOrder, retryCount, refetch]);

  if (!isOpen || !data) return null;

  return (
    <>
      <Dialog open={isOpen} onOpenChange={(open) => !open && closeWinDialog()}>
        <DialogContent
          className="w-full max-w-md bg-card border border-border p-6 shadow-2xl rounded-2xl gap-0 z-[100] focus-visible:outline-none overflow-hidden relative"
          showCloseButton={true}
        >
          <VisuallyHidden.Root>
            <DialogTitle>Auction Won Celebration Dialog</DialogTitle>
          </VisuallyHidden.Root>

          {/* Confetti canvas */}
          <CelebrationConfetti />

          <div className="flex flex-col items-center text-center relative z-10 pt-2">
            {/* Pulsing trophy wrapper */}
            <div className="relative">
              <div className="absolute -inset-1 rounded-full bg-gradient-to-r from-amber-500 to-yellow-400 opacity-30 blur-md animate-pulse" />
              <div className="relative mx-auto flex h-20 w-20 items-center justify-center rounded-full bg-gradient-to-tr from-amber-400 to-yellow-300 shadow-md shadow-amber-500/20 md:h-24 md:w-24">
                <Trophy className="h-10 w-10 text-amber-950 md:h-12 md:w-12 animate-bounce" />
              </div>
            </div>

            <h2 className="mt-5 text-2xl font-extrabold tracking-tight bg-gradient-to-r from-amber-500 via-orange-500 to-yellow-500 bg-clip-text text-transparent">
              Congratulations! 🎉
            </h2>
            <p className="mt-1 text-base font-semibold text-foreground">
              You won the auction!
            </p>
            <p className="mt-1.5 text-xs text-muted-foreground max-w-xs leading-relaxed">
              You had the winning bid. Secure your item by completing the payment now.
            </p>

            {/* Won Item Info block */}
            <div className="w-full mt-5 rounded-2xl border border-border bg-muted/30 p-4 flex gap-4 items-center text-left">
              {matchingOrder?.auction.imageUrl ? (
                <img
                  src={matchingOrder.auction.imageUrl}
                  alt={data.title}
                  className="h-14 w-14 rounded-xl object-cover border border-border/50 shadow-xs"
                />
              ) : (
                <div className="h-14 w-14 rounded-xl bg-muted flex items-center justify-center border border-border/50 shrink-0">
                  <Sparkles className="h-6 w-6 text-amber-500" />
                </div>
              )}
              <div className="flex-1 min-w-0">
                <h4 className="text-sm font-bold text-foreground truncate">
                  {matchingOrder?.auction.title || data.title}
                </h4>
                <p className="text-xs text-muted-foreground mt-0.5">
                  Winning Bid:{" "}
                  <span className="font-extrabold text-primary text-sm">
                    {formatCurrency(data.bidAmount)}
                  </span>
                </p>
              </div>
            </div>

            {/* Dynamic Status / Actions area */}
            <div className="w-full mt-6 space-y-3">
              {isOrdersLoading || (!matchingOrder && retryCount < 4) ? (
                // Loading Order details
                <div className="flex flex-col items-center justify-center py-3 space-y-2">
                  <Loader2 className="h-6 w-6 text-primary animate-spin" />
                  <p className="text-xs font-semibold text-muted-foreground">
                    Securing your checkout details...
                  </p>
                </div>
              ) : isError || (!matchingOrder && retryCount >= 4) ? (
                // Retry block if sync has failed
                <div className="flex flex-col items-center justify-center space-y-3 py-2 bg-destructive/5 rounded-xl border border-destructive/10 p-3">
                  <p className="text-xs text-destructive font-medium leading-relaxed">
                    Checkout generation is taking a moment on the server.
                  </p>
                  <Button
                    type="button"
                    onClick={() => {
                      setRetryCount(0);
                      void refetch();
                    }}
                    variant="outline"
                    size="sm"
                    className="gap-2 cursor-pointer font-bold rounded-lg"
                  >
                    <RefreshCw className="h-3.5 w-3.5" />
                    Retry Order Sync
                  </Button>
                </div>
              ) : matchingOrder && matchingOrder.status === "Pending" ? (
                // Ready for checkout payment
                <Button
                  type="button"
                  onClick={() => setIsPaymentOpen(true)}
                  className="w-full bg-primary hover:bg-primary/95 text-white font-bold rounded-xl h-12 flex items-center justify-center gap-2 cursor-pointer shadow-md shadow-primary/20 transition-all hover:scale-[1.01]"
                >
                  <CreditCard className="h-5 w-5" />
                  Complete Payment
                </Button>
              ) : matchingOrder ? (
                // Order is already paid / processed
                <div className="rounded-xl bg-success/10 border border-success/20 p-3.5 text-center text-sm font-semibold text-success flex items-center justify-center gap-2">
                  <CheckCircle2 className="h-5 w-5" />
                  Payment Completed
                </div>
              ) : null}

              {/* Navigation button to Purchases */}
              <Button
                type="button"
                variant="outline"
                onClick={() => {
                  closeWinDialog();
                  router.push(ROUTES.ORDERS.LIST);
                }}
                className="w-full hover:bg-muted text-foreground font-semibold rounded-xl h-11 flex items-center justify-center gap-2 cursor-pointer transition-colors"
              >
                <ShoppingBag className="h-4 w-4" />
                Go to Purchases
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>

      {/* Render the payment checkout sheets directly if the payment button is clicked */}
      {matchingOrder && (
        <CompletePaymentModal
          orderId={matchingOrder.id}
          orderNumber={matchingOrder.orderNumber}
          finalBid={matchingOrder.finalBid}
          title={matchingOrder.auction.title}
          imageUrl={matchingOrder.auction.imageUrl}
          isOpen={isPaymentOpen}
          onClose={() => {
            setIsPaymentOpen(false);
            closeWinDialog();
          }}
        />
      )}
    </>
  );
}
