"use client";

import { useMounted } from "@/hooks/use-mounted";
import { useEffect, useState } from "react";
import { differenceInSeconds } from "date-fns";

/**
 * Hook that provides a real-time countdown to an auction's end date.
 */
export function useAuctionCountdown(endDate: string) {

  const isMounted = useMounted();
  const [remainingSeconds, setRemainingSeconds] = useState(() =>
    calculateRemaining(endDate),
  );

  /**
   * Updates every second and returns remaining seconds + expiry state.
  */
  useEffect(() => {
    if (!isMounted) return;
    const interval = setInterval(() => {
      const remaining = calculateRemaining(endDate);
      setRemainingSeconds(remaining);

      if (remaining <= 0) {
        clearInterval(interval);
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [endDate, isMounted]);

  return {
    remainingSeconds: Math.max(0, remainingSeconds),
    isExpired: isMounted ? (remainingSeconds <= 0) : false,
    isMounted,
  };
}

function calculateRemaining(endDate: string): number {
  return differenceInSeconds(new Date(endDate), new Date());
}
