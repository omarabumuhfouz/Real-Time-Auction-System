import { ChevronRight, FileText, ShieldCheck } from "lucide-react";

interface ItemDetailsTabProps {
  category: string;
  subcategory: string;
  description: string;
  condition: string;
  conditionDescription: string;
}

export function ItemDetailsTab({
  category,
  subcategory,
  description,
  condition,
  conditionDescription,
}: ItemDetailsTabProps) {
  return (
    <div className="flex flex-col">
      {/* Tabs Header */}
      <div className="flex border-b border-black/40 border-b-2 mb-6">
        <button className="relative px-4 py-2 text-sm font-bold text-foreground border-b-2 border-primary -mb-[2px] transition-colors">
          Item Details
        </button>
      </div>

      {/* Content Container */}
      <div className="space-y-6">
        {/* Category Breadcrumbs / Info Row */}
        <div className="flex items-center gap-2 text-[10px] font-black text-muted-foreground uppercase tracking-[0.2em] px-1">
          <span>{category}</span>
          <ChevronRight className="size-3 text-muted-foreground/40" />
          <span className="text-primary">{subcategory}</span>
        </div>

        {/* Description Block */}
        <div className="rounded-xl border border-border bg-card/50 p-6">
          <h2 className="mb-4 text-base font-bold text-foreground flex items-center gap-2">
            <FileText className="size-4 text-primary" />
            Description
          </h2>
          <p className="text-[15px] leading-relaxed text-foreground/80 font-bold whitespace-pre-wrap">
            {description}
          </p>
        </div>

        {/* Condition Block */}
        <div className="rounded-xl border border-border bg-card/50 p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-base font-bold text-foreground flex items-center gap-2">
              <ShieldCheck className="size-4 text-primary" />
              Condition
            </h2>
            <div className="flex items-center gap-2">
              <span className="text-[10px] font-bold text-muted-foreground uppercase tracking-wider">Status:</span>
              <span className="rounded-full bg-primary/10 px-3 py-0.5 text-xs font-bold text-primary ring-1 ring-primary/20">
                {condition}
              </span>
            </div>
          </div>
          <p className="text-[15px] leading-relaxed text-foreground/80 font-bold">
            {conditionDescription}
          </p>
        </div>
      </div>
    </div>
  );
}
