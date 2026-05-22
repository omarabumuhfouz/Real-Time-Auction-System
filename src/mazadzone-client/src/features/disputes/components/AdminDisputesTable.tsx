import React, { useState } from "react";
import { MoreVertical } from "lucide-react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Checkbox } from "@/components/ui/checkbox";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { cn } from "@/lib/utils";
import { AdminDispute, AdminDisputeStatus } from "../types/admin-disputes.types";
import { ModerateUsersPagination } from "../../admin/components/users/ModerateUsersPagination";
import { ViewDisputeSheet } from "./ViewDisputeSheet";

interface AdminDisputesTableProps {
  data: AdminDispute[];
  isLoading: boolean;
  page: number;
  pageSize: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

export function AdminDisputesTable({ 
  data, 
  isLoading,
  page,
  pageSize,
  totalPages,
  onPageChange,
  onPageSizeChange
}: AdminDisputesTableProps) {
  const [selectedDispute, setSelectedDispute] = useState<AdminDispute | null>(null);

  const getStatusBadgeVariant = (status: AdminDisputeStatus) => {
    switch (status) {
      case AdminDisputeStatus.Open:
        return "info";
      case AdminDisputeStatus.UnderReview:
        return "review";
      case AdminDisputeStatus.AwaitingResponse:
        return "warning";
      case AdminDisputeStatus.Resolved:
        return "success";
      case AdminDisputeStatus.Rejected:
        return "destructive";
      default:
        return "outline";
    }
  };

  if (isLoading) {
    return <div className="p-8 text-center text-muted-foreground">Loading disputes...</div>;
  }

  if (!data.length) {
    return <div className="p-8 text-center text-muted-foreground">No disputes found matching criteria.</div>;
  }

  return (
    <div className="rounded-2xl border border-border bg-card overflow-hidden shadow-sm">
      <Table>
        <TableHeader>
          <TableRow className="hover:bg-transparent bg-muted/30">
            <TableHead className="w-12 text-center">
              <Checkbox className="rounded-[4px] border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary" />
            </TableHead>
            <TableHead className="font-bold text-foreground">Dispute ID</TableHead>
            <TableHead className="font-bold text-foreground">Order / Auction</TableHead>
            <TableHead className="font-bold text-foreground">Parties</TableHead>
            <TableHead className="font-bold text-foreground">Category</TableHead>
            <TableHead className="font-bold text-foreground">Status</TableHead>
            <TableHead className="font-bold text-foreground">Submitted Date</TableHead>
            <TableHead className="font-bold text-foreground text-right pr-6">Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {data.map((dispute) => (
            <TableRow key={dispute.id} className="group">
              <TableCell className="text-center align-middle">
                <Checkbox className="rounded-[4px] border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary" />
              </TableCell>
              <TableCell className="font-bold whitespace-nowrap">
                {dispute.id}
              </TableCell>
              <TableCell>
                <div className="flex flex-col">
                  <span className="text-sm font-bold">{dispute.orderOrAuctionId}</span>
                  <span className="text-sm text-muted-foreground">{dispute.orderOrAuctionName}</span>
                </div>
              </TableCell>
              <TableCell>
                <div className="flex flex-col">
                  <span className="text-sm font-bold">{dispute.parties.claimant}</span>
                  <span className="text-sm text-muted-foreground">vs</span>
                  <span className="text-sm font-bold">{dispute.parties.respondent}</span>
                </div>
              </TableCell>
              <TableCell className="text-sm font-medium">
                {dispute.category}
              </TableCell>
              <TableCell>
                <Badge variant={getStatusBadgeVariant(dispute.status) as any}>
                  {dispute.status}
                </Badge>
              </TableCell>
              <TableCell>
                <div className="flex flex-col">
                  <span className="text-sm">{dispute.submittedDate.split(' ').slice(0, 3).join(' ')}</span>
                  <span className="text-sm text-muted-foreground">{dispute.submittedDate.split(' ').slice(3).join(' ')}</span>
                </div>
              </TableCell>
              <TableCell className="text-right pr-4 align-middle">
                <div className="flex items-center justify-end gap-2 ml-auto w-[80px]">
                  <Button 
                    variant="outline" 
                    size="sm" 
                    className="h-8 w-full font-bold text-xs bg-card hover:bg-muted"
                    onClick={() => setSelectedDispute(dispute)}
                  >
                    View
                  </Button>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" size="icon" className="h-8 w-8 text-muted-foreground hover:text-foreground">
                        <MoreVertical className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem>View Details</DropdownMenuItem>
                      <DropdownMenuItem>Contact Parties</DropdownMenuItem>
                      <DropdownMenuItem className="text-destructive">Escalate</DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </div>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
      
      {!isLoading && data.length > 0 && (
        <ModerateUsersPagination
          page={page}
          pageSize={pageSize}
          totalPages={totalPages}
          onPageChange={onPageChange}
          onPageSizeChange={onPageSizeChange}
        />
      )}

      {selectedDispute && (
        <ViewDisputeSheet 
          dispute={selectedDispute}
          isOpen={!!selectedDispute} 
          onClose={() => setSelectedDispute(null)} 
        />
      )}
    </div>
  );
}
