import React from "react";
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { PAGE_SIZE_OPTIONS } from "../../constants/moderate-users.constants";

interface ModerateUsersPaginationProps {
  page: number;
  pageSize: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

export function ModerateUsersPagination({
  page,
  pageSize,
  totalPages,
  onPageChange,
  onPageSizeChange,
}: ModerateUsersPaginationProps) {
  // Helper to compute visible page numbers centering the active page
  const getVisiblePages = () => {
    let start = Math.max(1, page - 2);
    let end = Math.min(totalPages, page + 2);

    if (page <= 3) {
      end = Math.min(totalPages, 5);
    }
    if (page >= totalPages - 2) {
      start = Math.max(1, totalPages - 4);
    }

    const pages: number[] = [];
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return { pages, start, end };
  };

  const { pages: visiblePages, start: startPage, end: endPage } = getVisiblePages();
  const showFirstPage = startPage > 1;
  const showFirstEllipsis = startPage > 2;
  const showLastEllipsis = endPage < totalPages - 1;
  const showLastPage = endPage < totalPages;

  return (
    <div className="flex flex-col sm:flex-row items-center justify-between p-4 border-t border-border bg-muted/10 gap-4">
      {/* Pagination Controls */}
      <Pagination className="mx-0 w-auto">
        <PaginationContent className="gap-1.5">
          <PaginationItem>
            <PaginationPrevious
              href="#"
              onClick={(e) => {
                e.preventDefault();
                if (page > 1) onPageChange(page - 1);
              }}
              className={page <= 1 ? "pointer-events-none opacity-50 h-8! px-2! text-xs! [&_svg]:h-3.5 [&_svg]:w-3.5" : "cursor-pointer h-8! px-2! text-xs! [&_svg]:h-3.5 [&_svg]:w-3.5"}
            />
          </PaginationItem>

          {showFirstPage && (
            <>
              <PaginationItem>
                <PaginationLink
                  href="#"
                  onClick={(e) => {
                    e.preventDefault();
                    onPageChange(1);
                  }}
                  className="h-8! w-8! text-xs! cursor-pointer"
                >
                  1
                </PaginationLink>
              </PaginationItem>
              {showFirstEllipsis && (
                <PaginationItem>
                  <PaginationEllipsis className="h-8! w-8! [&_svg]:h-3.5 [&_svg]:w-3.5" />
                </PaginationItem>
              )}
            </>
          )}

          {visiblePages.map((p) => (
            <PaginationItem key={p}>
              <PaginationLink
                href="#"
                isActive={p === page}
                onClick={(e) => {
                  e.preventDefault();
                  if (p !== page) onPageChange(p);
                }}
                className={
                  p === page
                    ? "h-8! w-8! text-xs! bg-primary text-primary-foreground hover:bg-primary/90 border-primary"
                    : "h-8! w-8! text-xs! cursor-pointer"
                }
              >
                {p}
              </PaginationLink>
            </PaginationItem>
          ))}

          {showLastEllipsis && (
            <PaginationItem>
              <PaginationEllipsis className="h-8! w-8! [&_svg]:h-3.5 [&_svg]:w-3.5" />
            </PaginationItem>
          )}

          {showLastPage && (
            <PaginationItem>
              <PaginationLink
                href="#"
                onClick={(e) => {
                  e.preventDefault();
                  onPageChange(totalPages);
                }}
                className="h-8! w-8! text-xs! cursor-pointer"
              >
                {totalPages}
              </PaginationLink>
            </PaginationItem>
          )}

          <PaginationItem>
            <PaginationNext
              href="#"
              onClick={(e) => {
                e.preventDefault();
                if (page < totalPages) onPageChange(page + 1);
              }}
              className={page >= totalPages ? "pointer-events-none opacity-50 h-8! px-2! text-xs! [&_svg]:h-3.5 [&_svg]:w-3.5" : "cursor-pointer h-8! px-2! text-xs! [&_svg]:h-3.5 [&_svg]:w-3.5"}
            />
          </PaginationItem>
        </PaginationContent>
      </Pagination>

      {/* Rows per page selector */}
      <div className="flex items-center gap-2 text-xs text-muted-foreground">
        <span>Rows per page</span>
        <Select value={pageSize.toString()} onValueChange={(val) => onPageSizeChange(Number(val))}>
          <SelectTrigger className="h-8 px-2.5 font-semibold text-foreground border-border w-[70px]">
            <SelectValue />
          </SelectTrigger>
          <SelectContent sideOffset={5}>
            {PAGE_SIZE_OPTIONS.map((size) => (
              <SelectItem key={size} value={size.toString()}>
                {size}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>
    </div>
  );
}
