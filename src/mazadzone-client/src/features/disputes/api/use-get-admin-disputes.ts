import { useQuery } from "@tanstack/react-query";
import { AdminDispute, AdminDisputeCategory, AdminDisputeStatus, AdminDisputesFilters } from "../types/admin-disputes.types";

const mockDisputes: AdminDispute[] = [
  {
    id: "DSP-2024-0091",
    orderOrAuctionId: "ORD-2024-01562",
    orderOrAuctionName: "Rolex Submariner Date",
    parties: { claimant: "Ahmad Khan", respondent: "Sara Ali" },
    category: AdminDisputeCategory.ItemNotAsDescribed,
    status: AdminDisputeStatus.Open,
    submittedDate: "May 28, 2024 2:35 PM",
  },
  {
    id: "DSP-2024-0087",
    orderOrAuctionId: "AUC-2024-00125",
    orderOrAuctionName: "iPhone 15 Pro Max 256GB",
    parties: { claimant: "Bilal Hussain", respondent: "Usman Tariq" },
    category: AdminDisputeCategory.ItemNotReceived,
    status: AdminDisputeStatus.UnderReview,
    submittedDate: "May 27, 2024 10:18 AM",
  },
  {
    id: "DSP-2024-0082",
    orderOrAuctionId: "ORD-2024-01478",
    orderOrAuctionName: "Vintage Landscape Painting",
    parties: { claimant: "Ayesha Malik", respondent: "Nida Ahmed" },
    category: AdminDisputeCategory.DamagedItem,
    status: AdminDisputeStatus.AwaitingResponse,
    submittedDate: "May 26, 2024 6:12 PM",
  },
  {
    id: "DSP-2024-0079",
    orderOrAuctionId: "AUC-2024-00118",
    orderOrAuctionName: "Canon EOS R5 Camera",
    parties: { claimant: "Faisal Noor", respondent: "Ahmad Khan" },
    category: AdminDisputeCategory.DeliveryDelay,
    status: AdminDisputeStatus.Open,
    submittedDate: "May 26, 2024 11:03 AM",
  },
  {
    id: "DSP-2024-0074",
    orderOrAuctionId: "ORD-2024-01455",
    orderOrAuctionName: 'MacBook Pro 16" M2',
    parties: { claimant: "Imran Siddiqui", respondent: "Hassan Raza" },
    category: AdminDisputeCategory.PaymentIssue,
    status: AdminDisputeStatus.UnderReview,
    submittedDate: "May 25, 2024 9:09 AM",
  },
  {
    id: "DSP-2024-0070",
    orderOrAuctionId: "ORD-2024-01422",
    orderOrAuctionName: "Nike Air Jordan 4 Retro",
    parties: { claimant: "Zainab Fatima", respondent: "Sara Ali" },
    category: AdminDisputeCategory.ItemNotAsDescribed,
    status: AdminDisputeStatus.Resolved,
    submittedDate: "May 24, 2024 3:21 PM",
  },
  {
    id: "DSP-2024-0065",
    orderOrAuctionId: "AUC-2024-00105",
    orderOrAuctionName: "18K Gold Necklace",
    parties: { claimant: "Usman Tariq", respondent: "Ayesha Malik" },
    category: AdminDisputeCategory.RefundRequest,
    status: AdminDisputeStatus.Resolved,
    submittedDate: "May 23, 2024 12:45 PM",
  },
  {
    id: "DSP-2024-0061",
    orderOrAuctionId: "ORD-2024-01398",
    orderOrAuctionName: "Louis Vuitton Neverfull MM",
    parties: { claimant: "Nida Ahmed", respondent: "Bilal Hussain" },
    category: AdminDisputeCategory.ItemNotReceived,
    status: AdminDisputeStatus.Rejected,
    submittedDate: "May 22, 2024 4:33 PM",
  },
  {
    id: "DSP-2024-0058",
    orderOrAuctionId: "AUC-2024-00102",
    orderOrAuctionName: "Fender Stratocaster Guitar",
    parties: { claimant: "Hassan Raza", respondent: "Faisal Noor" },
    category: AdminDisputeCategory.DamagedItem,
    status: AdminDisputeStatus.AwaitingResponse,
    submittedDate: "May 22, 2024 9:16 AM",
  },
  {
    id: "DSP-2024-0053",
    orderOrAuctionId: "ORD-2024-01355",
    orderOrAuctionName: "Coins & Stamps Collection",
    parties: { claimant: "Ahmad Khan", respondent: "Zainab Fatima" },
    category: AdminDisputeCategory.PaymentIssue,
    status: AdminDisputeStatus.Open,
    submittedDate: "May 21, 2024 2:14 PM",
  }
];

export function useGetAdminDisputes(filters: AdminDisputesFilters) {
  return useQuery({
    queryKey: ["admin-disputes", filters],
    queryFn: async () => {
      // Simulate API call
      await new Promise((resolve) => setTimeout(resolve, 600));

      let data = [...mockDisputes];

      if (filters.search) {
        const query = filters.search.toLowerCase();
        data = data.filter(
          (d) =>
            d.id.toLowerCase().includes(query) ||
            d.orderOrAuctionId.toLowerCase().includes(query) ||
            d.parties.claimant.toLowerCase().includes(query) ||
            d.parties.respondent.toLowerCase().includes(query)
        );
      }

      if (filters.status && filters.status !== "All Statuses") {
        data = data.filter((d) => d.status === filters.status);
      }

      if (filters.category && filters.category !== "All Categories") {
        data = data.filter((d) => d.category === filters.category);
      }

      return {
        data,
        total: data.length,
        totalPages: 1,
      };
    },
  });
}
