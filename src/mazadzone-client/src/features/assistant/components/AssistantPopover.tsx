"use client";

import React, { useState } from "react";
import { MessageSquare, X } from "lucide-react";
import { AssistantChatWindow } from "./AssistantChatWindow";

export function AssistantPopover() {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div className="fixed bottom-6 right-6 z-50 flex flex-col items-end gap-4 font-sans select-none">
      {/* Chat Window Panel */}
      {isOpen && (
        <div className="shadow-2xl animate-in fade-in-0 slide-in-from-bottom-6 zoom-in-95 duration-250">
          <AssistantChatWindow
            onClose={() => setIsOpen(false)}
            onMinimize={() => setIsOpen(false)}
          />
        </div>
      )}

      {/* Floating Trigger Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="relative flex size-14 cursor-pointer items-center justify-center rounded-full bg-primary text-primary-foreground shadow-lg transition-all duration-300 hover:scale-105 hover:bg-primary/95 hover:shadow-xl active:scale-95 active:shadow-md"
        aria-label="Toggle auction guide agent"
        aria-expanded={isOpen}
      >
        {isOpen ? (
          <X className="size-6 animate-in spin-in-90 duration-200" />
        ) : (
          <>
            <MessageSquare className="size-6 animate-in zoom-in duration-200" />
            {/* Subtle Pulse Badge indicating Active Status */}
            <span className="absolute -right-0.5 -top-0.5 flex size-3.5">
              <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-emerald-400 opacity-75"></span>
              <span className="relative inline-flex size-3.5 rounded-full bg-emerald-500 border-2 border-white"></span>
            </span>
          </>
        )}
      </button>
    </div>
  );
}
