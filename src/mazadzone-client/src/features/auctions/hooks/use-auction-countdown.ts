"use client";

import { useMounted } from "@/hooks/use-mounted";
import { useEffect, useState } from "react";
import { differenceInSeconds } from "date-fns";
import { parseUtcDate } from "@/utils/date.utils";

/**
 * Hook that provides a real-time countdown to an auction's end date.
 */
export function useAuctionCountdown(endDate: Date | string) {
  const isMounted = useMounted();
  const endTimestamp = endDate ? parseUtcDate(endDate).getTime() : 0;

  const [remainingSeconds, setRemainingSeconds] = useState(() =>
    calculateRemaining(endTimestamp),
  );

  useEffect(() => {
    setRemainingSeconds(calculateRemaining(endTimestamp));

    if (!isMounted || endTimestamp <= 0) return;

    const interval = setInterval(() => {
      const remaining = calculateRemaining(endTimestamp);
      setRemainingSeconds(remaining);

      if (remaining <= 0) {
        clearInterval(interval);
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [endTimestamp, isMounted]);

  return {
    remainingSeconds: Math.max(0, remainingSeconds),
    isExpired: isMounted ? (remainingSeconds <= 0) : false,
    isMounted,
  };
}

function calculateRemaining(endTimestamp: number): number {
  if (endTimestamp <= 0) return 0;
  return differenceInSeconds(new Date(endTimestamp), new Date());
}
