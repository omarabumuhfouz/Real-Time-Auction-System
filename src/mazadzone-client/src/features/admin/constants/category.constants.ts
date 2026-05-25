import {
  Monitor,
  Shirt,
  Home,
  Image,
  Bike,
  Car,
  Gem,
  Music,
  FolderOpen,
  Wrench,
  ShoppingBag,
  Heart,
} from "lucide-react";
import { createElement } from "react";
import type { ComponentType } from "react";

export interface IconStyle {
  bg: string;
  text: string;
  border: string;
}

export interface CategoryIconOption {
  name: string;
  icon: React.ComponentType<{ className?: string }>;
  color: string; // Tailwind class string for icon preview in form
  styles: IconStyle; // Page styles for table and details view
}

const ICONS_BY_NAME: Record<string, ComponentType<{ className?: string }>> = {
  Monitor,
  Shirt,
  Home,
  Image,
  Bike,
  Car,
  Gem,
  Music,
  FolderOpen,
  Wrench,
  ShoppingBag,
  Heart,
};

// Helper component to resolve icons by name without importing the whole lucide set.
export function CategoryIcon({ name, className }: { name: string; className?: string }) {
  const IconComponent = ICONS_BY_NAME[name] ?? FolderOpen;
  return createElement(IconComponent, { className });
}

export const CATEGORY_ICONS: CategoryIconOption[] = [
  {
    name: "Monitor",
    icon: Monitor,
    color: "text-blue-500 bg-blue-500/10 border-blue-500/20",
    styles: { bg: "bg-blue-500/10 dark:bg-blue-500/20", text: "text-blue-600 dark:text-blue-400", border: "border-blue-500/20" },
  },
  {
    name: "Shirt",
    icon: Shirt,
    color: "text-purple-500 bg-purple-500/10 border-purple-500/20",
    styles: { bg: "bg-purple-500/10 dark:bg-purple-500/20", text: "text-purple-600 dark:text-purple-400", border: "border-purple-500/20" },
  },
  {
    name: "Home",
    icon: Home,
    color: "text-green-500 bg-green-500/10 border-green-500/20",
    styles: { bg: "bg-green-500/10 dark:bg-green-500/20", text: "text-green-600 dark:text-green-400", border: "border-green-500/20" },
  },
  {
    name: "Image",
    icon: Image,
    color: "text-orange-500 bg-orange-500/10 border-orange-500/20",
    styles: { bg: "bg-orange-500/10 dark:bg-orange-500/20", text: "text-orange-600 dark:text-orange-400", border: "border-orange-500/20" },
  },
  {
    name: "Bike",
    icon: Bike,
    color: "text-amber-500 bg-amber-500/10 border-amber-500/20",
    styles: { bg: "bg-amber-500/10 dark:bg-amber-500/20", text: "text-amber-600 dark:text-amber-500", border: "border-amber-500/20" },
  },
  {
    name: "Car",
    icon: Car,
    color: "text-red-500 bg-red-500/10 border-red-500/20",
    styles: { bg: "bg-red-500/10 dark:bg-red-500/20", text: "text-red-600 dark:text-red-400", border: "border-red-500/20" },
  },
  {
    name: "Gem",
    icon: Gem,
    color: "text-cyan-500 bg-cyan-500/10 border-cyan-500/20",
    styles: { bg: "bg-cyan-500/10 dark:bg-cyan-500/20", text: "text-cyan-600 dark:text-cyan-400", border: "border-cyan-500/20" },
  },
  {
    name: "Music",
    icon: Music,
    color: "text-pink-500 bg-pink-500/10 border-pink-500/20",
    styles: { bg: "bg-pink-500/10 dark:bg-pink-500/20", text: "text-pink-600 dark:text-pink-400", border: "border-pink-500/20" },
  },
  {
    name: "FolderOpen",
    icon: FolderOpen,
    color: "text-slate-500 bg-slate-500/10 border-slate-500/20",
    styles: { bg: "bg-slate-500/10 dark:bg-slate-500/20", text: "text-slate-600 dark:text-slate-400", border: "border-slate-500/20" },
  },
  {
    name: "Wrench",
    icon: Wrench,
    color: "text-emerald-500 bg-emerald-500/10 border-emerald-500/20",
    styles: { bg: "bg-emerald-500/10 dark:bg-emerald-500/20", text: "text-emerald-600 dark:text-emerald-400", border: "border-emerald-500/20" },
  },
  {
    name: "ShoppingBag",
    icon: ShoppingBag,
    color: "text-rose-500 bg-rose-500/10 border-rose-500/20",
    styles: { bg: "bg-rose-500/10 dark:bg-rose-500/20", text: "text-rose-600 dark:text-rose-400", border: "border-rose-500/20" },
  },
  {
    name: "Heart",
    icon: Heart,
    color: "text-red-600 bg-red-600/10 border-red-600/20",
    styles: { bg: "bg-red-600/10 dark:bg-red-600/20", text: "text-red-600 dark:text-red-400", border: "border-red-600/20" },
  },
];

export const DEFAULT_ICON_STYLE: IconStyle = {
  bg: "bg-slate-500/10 dark:bg-slate-500/20",
  text: "text-slate-600 dark:text-slate-400",
  border: "border-slate-500/20",
};

export const getCategoryStyles = (iconName: string): IconStyle => {
  const match = CATEGORY_ICONS.find((item) => item.name === iconName);
  return match ? match.styles : DEFAULT_ICON_STYLE;
};
