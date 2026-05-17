"use client";

import Link from "next/link";

export function UnauthenticatedState() {
  return (
    <div className="flex flex-col items-center justify-center py-16 px-4 text-center bg-white border border-gray-100 rounded-2xl shadow-sm">
      <div className="h-16 w-16 bg-primary/10 text-primary rounded-full flex items-center justify-center mb-4">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          strokeWidth={1.5}
          stroke="currentColor"
          className="w-8 h-8"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z"
          />
        </svg>
      </div>
      <h3 className="text-xl font-bold text-gray-900 mb-2">Sign In to View Bids</h3>
      <p className="text-gray-500 max-w-md mb-6 text-md">
        Please sign in to your MazadZone account to track and monitor your leading and outbid activities.
      </p>
      <Link
        href="/login"
        className="px-6 h-12 bg-primary hover:bg-primary/90 text-white rounded-xl font-bold flex items-center justify-center transition-colors shadow-md cursor-pointer"
      >
        Sign In
      </Link>
    </div>
  );
}
