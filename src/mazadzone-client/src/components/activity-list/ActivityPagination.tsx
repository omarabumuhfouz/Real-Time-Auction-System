"use client";

import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";

export interface ActivityPaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  className?: string;
}

/**
 * ActivityPagination Component
 * 
 * An optimized pagination bar for activity dashboards. Generates active sibling page links 
 * and handles pagination actions, automatically scrolling smoothly to the top of the container.
 * 
 * @param currentPage - The currently active page (1-indexed).
 * @param totalPages - The total number of pages available.
 * @param onPageChange - Callback function triggered when a page navigation link is selected.
 * @param hasPreviousPage - Boolean indicating whether there is a previous page available.
 * @param hasNextPage - Boolean indicating whether there is a next page available.
 */
export function ActivityPagination({
  currentPage,
  totalPages,
  onPageChange,
  hasPreviousPage,
  hasNextPage,
  className,
}: ActivityPaginationProps) {
  if (totalPages <= 1) return null;

  const handlePageClick = (page: number) => {
    onPageChange(page);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const getPageNumbers = () => {
    const pages: (number | "ellipsis")[] = [];
    const siblingCount = 1;

    for (let i = 1; i <= totalPages; i++) {
      if (
        i === 1 ||
        i === totalPages ||
        (i >= currentPage - siblingCount && i <= currentPage + siblingCount)
      ) {
        if (pages.length > 0) {
          const lastAdded = pages[pages.length - 1];
          if (typeof lastAdded === "number" && i - lastAdded > 1) {
            pages.push("ellipsis");
          }
        }
        pages.push(i);
      }
    }
    return Array.from(new Set(pages));
  };

  const pageNumbers = getPageNumbers();

  return (
    <div className={className}>
      <Pagination>
        <PaginationContent>
          <PaginationItem>
            <PaginationPrevious
              href="#"
              onClick={(e) => {
                e.preventDefault();
                if (hasPreviousPage) handlePageClick(currentPage - 1);
              }}
              className={!hasPreviousPage ? "pointer-events-none opacity-50" : "cursor-pointer"}
            />
          </PaginationItem>

          {pageNumbers.map((page, index) => (
            <PaginationItem key={index}>
              {page === "ellipsis" ? (
                <PaginationEllipsis />
              ) : (
                <PaginationLink
                  href="#"
                  isActive={page === currentPage}
                  onClick={(e) => {
                    e.preventDefault();
                    handlePageClick(page as number);
                  }}
                  className={
                    page === currentPage
                      ? "bg-primary text-primary-foreground hover:bg-primary/90 border-primary"
                      : "cursor-pointer"
                  }
                >
                  {page}
                </PaginationLink>
              )}
            </PaginationItem>
          ))}

          <PaginationItem>
            <PaginationNext
              href="#"
              onClick={(e) => {
                e.preventDefault();
                if (hasNextPage) handlePageClick(currentPage + 1);
              }}
              className={!hasNextPage ? "pointer-events-none opacity-50" : "cursor-pointer"}
            />
          </PaginationItem>
        </PaginationContent>
      </Pagination>
    </div>
  );
}
