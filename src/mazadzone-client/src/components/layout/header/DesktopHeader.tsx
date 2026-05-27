import Link from "next/link";
import { Search, Gavel, Package, User, ChevronDown, LayoutDashboard } from "lucide-react";
import { ROUTES } from "@/config/routes.config";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { CATEGORIES } from "./header.constants";
import type { AuthUser } from "@/stores/auth.store";
import { NotificationPopover, useGetUnreadCount, useNotificationStore } from "@/features/notifications";
import { useAuthStore } from "@/stores/auth.store";

export interface DesktopHeaderProps {
  isAuthenticated: boolean;
  user: AuthUser | null;
  role: string | undefined;
  logout: () => void;
  mounted: boolean;
  pathname: string;
}

export const DesktopHeader = ({
  isAuthenticated,
  logout,
  mounted,
  pathname,
}: DesktopHeaderProps) => {
  const userId = useAuthStore((state) => state.user?.id);
  const { data: serverUnreadCount = 0 } = useGetUnreadCount(userId || "", {
    enabled: isAuthenticated,
  });
  const hasLocalNotifications = useNotificationStore(
    (state) => state.notifications.length > 0,
  );
  const localUnreadCount = useNotificationStore((state) =>
    state.notifications.reduce(
      (count, notification) => count + (notification.isRead ? 0 : 1),
      0,
    ),
  );

  const unreadCount = hasLocalNotifications ? localUnreadCount : serverUnreadCount;


  return (
    <>
      {/* Desktop Search Bar (Part of Top Row) */}
      <div className="hidden md:flex flex-1 max-w-xl ml-5 mr-7 relative mx-0 pr-0">
        <Input
          className="w-full bg-white text-black  text-2xl pl-5  rounded-2xl h-13 border-none focus-visible:ring-2 focus-visible:ring-primary"
          placeholder="Search..."
        />
        <Search className="absolute right-3 top-1/2 -translate-y-1/2 h-6 w-6 text-gray-400" />
      </div>

      {/* Right Nav (Desktop) */}
      <div className="hidden md:flex items-center gap-6">
        <div className="flex items-center text-lg font-medium text-primary-foreground">
          <span className="cursor-pointer hover:text-white transition-colors">AR</span>
          <span className="mx-2 text-gray-500">|</span>
          <span className="cursor-pointer text-primary">EN</span>
        </div>

        <div
          className={cn(
            "flex items-center gap-6",
            !mounted && "invisible pointer-events-none select-none",
          )}
          aria-hidden={!mounted}
        >
          {!isAuthenticated ? (
            <div className="flex items-center gap-2">
              <Link href={ROUTES.AUTH.LOGIN} className="text-lg font-medium hover:text-primary transition-colors">
                Sign In
              </Link>
              <Link href={ROUTES.AUTH.REGISTER} className="text-lg font-medium bg-primary text-white px-4 py-2 rounded-md hover:bg-primary/90 transition-colors">
                Sign Up
              </Link>
            </div>
          ) : (
            <>
              <Link href={ROUTES.BIDDING.MY_BIDS} className={cn("hidden lg:flex items-center gap-2 text-xl font-medium transition-colors hover:text-primary", pathname === ROUTES.BIDDING.MY_BIDS ? "text-primary" : "")}>
                <Gavel className="h-7 w-7" />
                My Bids
              </Link>

              <Link href={ROUTES.ORDERS.LIST} className={cn("hidden lg:flex items-center gap-2 text-xl font-medium transition-colors hover:text-primary", pathname === ROUTES.ORDERS.LIST ? "text-primary" : "")}>
                <Package className="h-7 w-7" />
                My Orders
              </Link>

              <NotificationPopover unreadCount={unreadCount} />




              <DropdownMenu>
                <DropdownMenuTrigger className="flex items-center gap-1 hover:text-primary transition-colors outline-none">
                  <User className="h-7 w-7" />
                  <ChevronDown className="h-7 w-7" />
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" className="w-48 bg-white text-black border-none rounded-md shadow-lg">
                  <DropdownMenuItem asChild>
                    <Link href={ROUTES.PROFILE.VIEW} className="cursor-pointer font-medium">Profile Settings</Link>
                  </DropdownMenuItem>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem onClick={logout} className="cursor-pointer text-red-600 font-medium focus:text-red-600">
                    Logout
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </>
          )}
        </div>
      </div>

    </>
  );
};

export const DesktopBottomRow = ({
  mounted,
  isSeller,
  handleCategoryClick,
  handleSellClick,
  router, // We might need router for the dashboard button
}: {
  mounted: boolean;
  isSeller: boolean;
  handleCategoryClick: (category: string) => void;
  handleSellClick: () => void;
  router: { push: (path: string) => void };
}) => {
  return (
    <div className="hidden md:flex mx-auto h-14 max-w-[1408px] items-center justify-between pt-4 px-4 md:px-0">
      <nav className="flex items-center gap-6 whitespace-nowrap overflow-x-auto no-scrollbar pt-1.5">
        {CATEGORIES.map((category) => (
          <button
            key={category}
            onClick={() => handleCategoryClick(category)}
            className="text-lg font-normal text-primary-foreground hover:text-primary hover:border-primary transition-colors"
          >
            {category}
          </button>
        ))}
      </nav>

      <div className="flex items-center gap-9 shrink-0 ml-6">
        {mounted && isSeller && (
          <Button
            variant="outline"
            onClick={() => router.push(ROUTES.SELLER.AUCTIONS)}
            className="bg-transparent border-white/20 text-white  text-lg hover:bg-primary/20 hover:text-primary hover:border-primary gap-2 h-12 rounded-xl"
          >
            <LayoutDashboard className="h-5 w-5" />
            Seller Dashboard
          </Button>
        )}

        <Button
          onClick={handleSellClick}
          className="bg-primary hover:bg-primary/90 text-white font-medium px-6 h-12 w-24 rounded-xl shadow-md text-lg"
        >
          Sell
        </Button>
      </div>
    </div>
  );
};
