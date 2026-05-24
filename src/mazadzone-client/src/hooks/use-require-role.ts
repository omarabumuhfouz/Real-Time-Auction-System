import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuthStore } from "@/stores/auth.store";
import { useAppToast } from "@/lib/toast/app-toast";
import { isTokenExpired } from "@/lib/auth/token";
import { ROUTES } from "@/config/routes.config";

interface UseRequireRoleOptions {
  redirectToLogin?: string;
  redirectToUnauthorized?: string;
  loginMessage?: string;
  unauthorizedMessage?: string;
  bypassTesting?: boolean;
}

/**
 * Custom hook to protect pages on the client-side based on user role and token validity.
 */
export function useRequireRole(
  allowedRoles: string[],
  options: UseRequireRoleOptions = {}
) {
  const router = useRouter();
  
  // Auth states
  const user = useAuthStore((state) => state.user);
  const accessToken = useAuthStore((state) => state.accessToken);
  const isHydrated = useAuthStore((state) => state.isHydrated);
  
  const appToast = useAppToast();

  const {
    redirectToLogin = ROUTES.AUTH.LOGIN,
    redirectToUnauthorized = ROUTES.SELLER.BECOME,
    loginMessage = "Please log in to access this page.",
    unauthorizedMessage = "You do not have the required permissions to view this page.",
    bypassTesting = true, // Set to true to bypass checks during local testing/dev
  } = options;

  const isTokenValid = !!accessToken && !isTokenExpired(accessToken);
  const hasRole = !!user && allowedRoles.includes(user.role);
  const isAuthorized = isHydrated && (bypassTesting || (isTokenValid && hasRole));

  useEffect(() => {
    if (!isHydrated) return;
    if (bypassTesting) return;

    if (!isTokenValid) {
      appToast.error("Access Denied", loginMessage);
      router.push(redirectToLogin);
      return;
    }

    if (!hasRole) {
      appToast.warning("Seller Privilege Required", unauthorizedMessage);
      router.push(redirectToUnauthorized);
    }
  }, [
    isHydrated,
    isTokenValid,
    hasRole,
    bypassTesting,
    redirectToLogin,
    redirectToUnauthorized,
    loginMessage,
    unauthorizedMessage,
    router,
    appToast,
  ]);

  return {
    isAuthorized,
    isLoading: !isHydrated,
  };
}
