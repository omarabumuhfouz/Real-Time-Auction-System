"use client";

import React, { useState, useEffect, useRef } from "react";
import Image from "next/image";
import Link from "next/link";
import { useRouter } from "next/navigation";
import {
  X,
  Minus,
  Search,
  Clock,
  Gavel,
  UserPlus,
  Tag,
  Send,
  CheckCheck,
  Shield,
  Lock,
} from "lucide-react";

import { useAuthStore } from "@/stores/auth.store";
import { useSendChatMessage } from "../api/assistant.queries";
import { useGetAuctions } from "@/features/auctions";
import { formatCurrency } from "@/utils/currency.utils";
import { ROUTES } from "@/config/routes.config";
import { Button } from "@/components/ui/button";

interface Message {
  id: string;
  sender: "user" | "assistant";
  text: string;
  timestamp: string;
  showAuctions?: boolean;
}

interface AssistantChatWindowProps {
  onClose: () => void;
  onMinimize?: () => void;
}

export function AssistantChatWindow({ onClose, onMinimize }: AssistantChatWindowProps) {
  const router = useRouter();
  const { isAuthenticated } = useAuthStore();
  const sendChatMutation = useSendChatMessage();
  const sendLockRef = useRef(false);

  // Local Chat State
  const [messages, setMessages] = useState<Message[]>([
    {
      id: "welcome",
      sender: "assistant",
      text: "Hi, I'm Mazad Assistant. I can help you find auctions, understand bidding, and answer MazadZone questions.",
      timestamp: "10:32 AM",
    },
  ]);
  const [inputValue, setInputValue] = useState("");
  const [isArabic, setIsArabic] = useState(false);

  const scrollRef = useRef<HTMLDivElement>(null);

  // Auto Scroll to bottom
  useEffect(() => {
    if (scrollRef.current) {
      scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
    }
  }, [messages, sendChatMutation.isPending]);

  // Fetch active auctions to populate dynamic recommendation carousel
  const { data: auctionsData } = useGetAuctions({
    status: "Active",
    pageSize: 5,
  });

  // Safe mock auctions as fallback to ensure visualization is perfect even if backend is empty
  const fallbackAuctions = [
    {
      id: "macbook-air-m2",
      title: "MacBook Air M2 13-inch",
      imageUrl: "",
      currentBid: 420,
      nextMinBid: 430,
      timeLeft: "2h 10m",
    },
    {
      id: "iphone-14-pro",
      title: "iPhone 14 Pro 128GB",
      imageUrl: "",
      currentBid: 310,
      nextMinBid: 320,
      timeLeft: "1h 05m",
    },
    {
      id: "sony-wh1000xm4",
      title: "Sony WH-1000XM4 Headphones",
      imageUrl: "",
      currentBid: 95,
      nextMinBid: 100,
      timeLeft: "45m 30s",
    },
  ];

  // Derive dynamic list of products for carousel scroller
  const displayAuctions =
    auctionsData?.items && auctionsData.items.length > 0
      ? auctionsData.items.map((item) => ({
          id: item.id,
          title: item.title,
          imageUrl: item.imageUrl,
          currentBid: item.pricing.currentBid ?? item.pricing.startingPrice,
          nextMinBid: (item.pricing.currentBid ?? item.pricing.startingPrice) + 10,
          timeLeft: "Active",
        }))
      : fallbackAuctions;

  // Handle message send logic
  const handleSendMessage = (textToSend: string) => {
    const trimmedMessage = textToSend.trim();
    if (!trimmedMessage || !isAuthenticated || sendChatMutation.isPending || sendLockRef.current) return;

    sendLockRef.current = true;

    const timeString = new Date().toLocaleTimeString([], {
      hour: "2-digit",
      minute: "2-digit",
    });

    // Add user bubble
    const userMsgId = `user-${Date.now()}`;
    const userMsg: Message = {
      id: userMsgId,
      sender: "user",
      text: trimmedMessage,
      timestamp: timeString,
    };

    setMessages((prev) => [...prev, userMsg]);
    setInputValue("");

    // Detect if search terms trigger scroller visualization
    const triggersAuctions =
      trimmedMessage.toLowerCase().includes("soon") ||
      trimmedMessage.toLowerCase().includes("electronics") ||
      trimmedMessage.toLowerCase().includes("macbook") ||
      trimmedMessage.toLowerCase().includes("iphone") ||
      trimmedMessage.toLowerCase().includes("browse") ||
      trimmedMessage.toLowerCase().includes("ending");

    // Call actual backend RAG API
    sendChatMutation.mutate(trimmedMessage, {
      onSuccess: (data) => {
        const assistantMsg: Message = {
          id: `assistant-${Date.now()}`,
          sender: "assistant",
          text: data.response || "I could not resolve that query.",
          timestamp: new Date().toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit",
          }),
          showAuctions: triggersAuctions,
        };
        setMessages((prev) => [...prev, assistantMsg]);
      },
      onError: (err: { statusCode?: number; message?: string }) => {
        let errorMsgText = "Sorry, I had trouble reaching the live auction agent right now. Please try again in a bit.";
        if (err?.statusCode === 401) {
          errorMsgText = "Your session has expired. Please sign in again to chat with the assistant.";
        } else if (err?.statusCode && err.statusCode < 500 && err?.message) {
          errorMsgText = `Sorry, I had trouble reaching the live auction agent: ${err.message}`;
        }

        const errMsg: Message = {
          id: `err-${Date.now()}`,
          sender: "assistant",
          text: errorMsgText,
          timestamp: new Date().toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit",
          }),
        };
        setMessages((prev) => [...prev, errMsg]);
      },
      onSettled: () => {
        sendLockRef.current = false;
      },
    });
  };

  const handleShortcutClick = (tagLabel: string) => {
    if (!isAuthenticated) return;
    handleSendMessage(tagLabel);
  };

  return (
    <div className="flex h-[580px] w-[420px] flex-col overflow-hidden rounded-[16px] border border-border bg-background shadow-2xl animate-in fade-in-50 slide-in-from-bottom-5 duration-300">
      {/* 1. BRAND HEADER (Deep Navy) */}
      <header className="flex h-[76px] items-center justify-between bg-dark px-4 py-3 text-dark-foreground">
        <div className="flex items-center gap-3">
          {/* Custom Avatar with Orange Hammer Circle */}
          <div className="flex size-11 items-center justify-center rounded-full bg-primary shadow-inner">
            <svg
              className="size-6 text-white"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M3 6l3 1m0 0l-3 9a5.002 5.002 0 006.001 0M6 7l3 9M6 7l6-2m6 2l3-1m-3 1l-3 9a5.002 5.002 0 006.001 0M18 7l3 9m-3-9l-6-2m0-2v2m0 16V5m0 16H9m3 0h3"
              />
            </svg>
          </div>
          <div>
            <h2 className="text-base font-bold tracking-tight text-white leading-tight">
              Mazad Assistant
            </h2>
            <p className="text-xs text-muted-foreground/80 leading-normal">
              Your auction guide
            </p>
            {/* Live Indicator */}
            <div className="mt-0.5 flex items-center gap-1.5">
              <span className="relative flex size-2">
                <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-emerald-400 opacity-75"></span>
                <span className="relative inline-flex size-2 rounded-full bg-emerald-500"></span>
              </span>
              <span className="text-[10px] font-semibold text-emerald-400">
                Live auction data
              </span>
            </div>
          </div>
        </div>

        {/* Right side controls */}
        <div className="flex items-center gap-2">
          {/* Language Toggle */}
          <div className="flex items-center gap-1 text-xs">
            <button
              onClick={() => setIsArabic(true)}
              className={`cursor-pointer transition-colors ${
                isArabic ? "text-primary font-bold" : "text-muted-foreground hover:text-white"
              }`}
            >
              عربي
            </button>
            <span className="text-muted-foreground/40">|</span>
            <button
              onClick={() => setIsArabic(false)}
              className={`cursor-pointer transition-colors ${
                !isArabic ? "text-primary font-bold" : "text-muted-foreground hover:text-white"
              }`}
            >
              EN
            </button>
          </div>

          <span className="ml-1 text-muted-foreground/30">|</span>

          {/* Minimize and Close */}
          <button
            onClick={onMinimize}
            className="rounded p-1 text-muted-foreground hover:bg-white/10 hover:text-white transition-colors"
            title="Minimize"
          >
            <Minus className="size-4" />
          </button>
          <button
            onClick={onClose}
            className="rounded p-1 text-muted-foreground hover:bg-white/10 hover:text-white transition-colors"
            title="Close"
          >
            <X className="size-4" />
          </button>
        </div>
      </header>

      {/* 2. CHAT DIALOGUE AREA */}
      <div
        ref={scrollRef}
        className="flex-1 overflow-y-auto bg-[#FBF9F6] p-4 space-y-4"
        style={{ scrollBehavior: "smooth" }}
      >
        {messages.map((msg) => {
          const isUser = msg.sender === "user";
          return (
            <div key={msg.id} className="space-y-3">
              {/* Message Bubble Container */}
              <div className={`flex w-full ${isUser ? "justify-end" : "justify-start"}`}>
                <div
                  className={`relative max-w-[85%] rounded-[12px] p-3 shadow-xs ${
                    isUser
                      ? "bg-primary text-primary-foreground rounded-tr-none"
                      : "bg-[#FFF9F2] text-foreground border border-[#F4EBE0] rounded-tl-none"
                  }`}
                >
                  <p className="text-sm font-normal leading-relaxed whitespace-pre-line">
                    {msg.text}
                  </p>
                  <div className="mt-1 flex items-center justify-end gap-1 text-[9px] opacity-70">
                    <span>{msg.timestamp}</span>
                    {isUser && <CheckCheck className="size-3 text-white/90" />}
                  </div>
                </div>
              </div>

              {/* If Assistant recommendations is active, show the carousel */}
              {msg.showAuctions && (
                <div className="space-y-3">
                  {/* Dynamic scroller container */}
                  <div className="no-scrollbar flex w-full gap-3 overflow-x-auto pb-2">
                    {displayAuctions.map((auc) => (
                      <div
                        key={auc.id}
                        className="flex w-[260px] shrink-0 flex-col rounded-[10px] border border-border bg-card p-2.5 shadow-sm"
                      >
                        {/* Compact Card Content */}
                        <div className="flex gap-2">
                          {/* Left Product Image Fallback or URL */}
                          <div className="relative size-[68px] shrink-0 overflow-hidden rounded-md bg-muted border border-border flex items-center justify-center">
                            {auc.imageUrl ? (
                              <Image
                                src={auc.imageUrl}
                                alt={auc.title}
                                fill
                                className="object-cover"
                              />
                            ) : (
                              <svg
                                className="size-7 text-muted-foreground"
                                fill="none"
                                stroke="currentColor"
                                viewBox="0 0 24 24"
                              >
                                <path
                                  strokeLinecap="round"
                                  strokeLinejoin="round"
                                  strokeWidth={1.5}
                                  d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"
                                />
                              </svg>
                            )}
                          </div>

                          {/* Right Product details */}
                          <div className="flex-1 min-w-0">
                            <h4 className="truncate text-xs font-bold text-foreground">
                              {auc.title}
                            </h4>
                            <div className="mt-1 flex items-baseline justify-between gap-1">
                              <span className="text-[10px] text-muted-foreground">Current Bid</span>
                              <span className="text-[11px] font-extrabold text-foreground">
                                {formatCurrency(auc.currentBid)}
                              </span>
                            </div>
                            <div className="flex items-baseline justify-between gap-1">
                              <span className="text-[10px] text-muted-foreground font-medium">
                                Next Min. Bid
                              </span>
                              <span className="text-[11px] font-bold text-primary">
                                {formatCurrency(auc.nextMinBid)}
                              </span>
                            </div>
                          </div>
                        </div>

                        {/* Timing pill and CTA button actions */}
                        <div className="mt-2.5 flex items-center justify-between gap-2 border-t border-dashed border-border pt-2">
                          <div className="flex items-center gap-1 rounded-full border border-orange-200 bg-orange-50 px-2 py-0.5 text-[9px] font-semibold text-primary">
                            <Clock className="size-2.5 text-primary animate-pulse" />
                            <span>{auc.timeLeft}</span>
                          </div>

                          <div className="flex gap-1">
                            <Link href={ROUTES.AUCTIONS.DETAIL(auc.id)}>
                              <Button
                                variant="outline"
                                className="h-[24px] cursor-pointer rounded-md border-border bg-white px-2 text-[9px] font-bold text-foreground hover:bg-muted"
                              >
                                View
                              </Button>
                            </Link>
                            <Link href={ROUTES.AUCTIONS.DETAIL(auc.id)}>
                              <Button className="h-[24px] cursor-pointer rounded-md bg-primary px-2 text-[9px] font-bold text-primary-foreground hover:bg-primary/95">
                                Place Bid
                              </Button>
                            </Link>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>

                  {/* Realtime Alert beneath */}
                  <div className="flex items-center justify-between rounded-md border border-[#F4EBE0] bg-[#FFFBF7] px-2.5 py-1.5 text-[10px] text-muted-foreground/90">
                    <span className="flex items-center gap-1 font-medium">
                      <Shield className="size-3 text-primary" />
                      Bids and times update in real time.
                    </span>
                    <span>10:33 AM</span>
                  </div>
                </div>
              )}
            </div>
          );
        })}

        {/* AI Answer Processing Loader */}
        {sendChatMutation.isPending && (
          <div className="flex w-full justify-start animate-pulse">
            <div className="rounded-[12px] bg-[#FFF9F2] p-3 text-sm text-muted-foreground border border-[#F4EBE0] rounded-tl-none flex items-center gap-1.5">
              <span className="size-1.5 rounded-full bg-primary/70 animate-bounce [animation-delay:-0.3s]"></span>
              <span className="size-1.5 rounded-full bg-primary/70 animate-bounce [animation-delay:-0.15s]"></span>
              <span className="size-1.5 rounded-full bg-primary/70 animate-bounce"></span>
            </div>
          </div>
        )}

        {/* 3. QUICK CHIPS RECOMMENDATIONS */}
        {isAuthenticated && messages.length === 1 && (
          <div className="flex flex-wrap gap-2 pt-2 animate-in fade-in duration-500">
            <button
              onClick={() => handleShortcutClick("Browse auctions")}
              className="flex items-center gap-1 rounded-full border border-border bg-white px-3 py-1.5 text-xs text-foreground font-medium shadow-xs hover:bg-muted hover:border-muted-foreground/30 transition-all cursor-pointer"
            >
              <Search className="size-3.5 text-muted-foreground" />
              Browse auctions
            </button>
            <button
              onClick={() => handleShortcutClick("Show me electronics ending soon")}
              className="flex items-center gap-1 rounded-full border border-border bg-white px-3 py-1.5 text-xs text-foreground font-medium shadow-xs hover:bg-muted hover:border-muted-foreground/30 transition-all cursor-pointer"
            >
              <Clock className="size-3.5 text-muted-foreground" />
              Ending soon
            </button>
            <button
              onClick={() => handleShortcutClick("How bidding works")}
              className="flex items-center gap-1 rounded-full border border-border bg-white px-3 py-1.5 text-xs text-foreground font-medium shadow-xs hover:bg-muted hover:border-muted-foreground/30 transition-all cursor-pointer"
            >
              <Gavel className="size-3.5 text-muted-foreground" />
              How bidding works
            </button>
            <button
              onClick={() => handleShortcutClick("Become a seller")}
              className="flex items-center gap-1 rounded-full border border-border bg-white px-3 py-1.5 text-xs text-foreground font-medium shadow-xs hover:bg-muted hover:border-muted-foreground/30 transition-all cursor-pointer"
            >
              <UserPlus className="size-3.5 text-muted-foreground" />
              Become a seller
            </button>
            <button
              onClick={() => handleShortcutClick("My bids")}
              className="flex items-center gap-1 rounded-full border border-border bg-white px-3 py-1.5 text-xs text-foreground font-medium shadow-xs hover:bg-muted hover:border-muted-foreground/30 transition-all cursor-pointer"
            >
              <Tag className="size-3.5 text-muted-foreground" />
              My bids
            </button>
          </div>
        )}

        {/* 4. GUEST FALLBACK CONTAINER */}
        {!isAuthenticated && (
          <div className="mx-auto my-6 max-w-[90%] rounded-[12px] border border-orange-200 bg-orange-50/70 p-4 text-center shadow-xs animate-in zoom-in-95 duration-300">
            <div className="mx-auto flex size-12 items-center justify-center rounded-full bg-orange-100 text-primary">
              <Lock className="size-5" />
            </div>
            <h3 className="mt-3 text-sm font-bold text-foreground">Sign in to chat</h3>
            <p className="mt-1 text-xs text-muted-foreground leading-relaxed">
              Mazad Assistant requires active user credentials to answer bidding questions and fetch
              real-time custom recommendations.
            </p>
            <button
              onClick={() => {
                onClose();
                router.push(ROUTES.AUTH.LOGIN);
              }}
              className="mt-4 w-full cursor-pointer rounded-lg bg-primary py-2 text-xs font-bold tracking-wide text-primary-foreground shadow-xs hover:bg-primary/95 transition-all"
            >
              Sign In to Your Account
            </button>
          </div>
        )}
      </div>

      {/* 3. DIALOGUE INPUT BAR */}
      <div className="border-t border-border bg-white p-3">
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleSendMessage(inputValue);
          }}
          className="flex items-center gap-2"
        >
          {/* Main Rounded Input Frame */}
          <div className="flex flex-1 items-center gap-2 rounded-[24px] border border-input bg-white px-4 py-1.5 transition-colors focus-within:border-primary focus-within:ring-2 focus-within:ring-primary/20">
            {/* Actual Input */}
            <input
              type="text"
              value={inputValue}
              onChange={(e) => setInputValue(e.target.value)}
              disabled={!isAuthenticated || sendChatMutation.isPending}
              placeholder="Ask about auctions, bidding, or selling..."
              className="w-full bg-transparent py-1 text-sm font-medium text-foreground outline-hidden placeholder:text-muted-foreground/60 disabled:cursor-not-allowed disabled:opacity-50"
            />
          </div>

          {/* Send Button */}
          <button
            type="submit"
            disabled={!isAuthenticated || !inputValue.trim() || sendChatMutation.isPending}
            className="flex size-[38px] cursor-pointer items-center justify-center rounded-full bg-primary text-white shadow-xs transition-transform hover:scale-105 active:scale-95 disabled:scale-100 disabled:cursor-not-allowed disabled:bg-muted disabled:text-muted-foreground/50"
          >
            <Send className="size-4" />
          </button>
        </form>
      </div>
    </div>
  );
}
