using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.SellerDashboard.DTOs;
using MazadZone.Domain.Primitives.Results;
using MazadZone.Domain.Users.ValueObjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.Text;
using System;

namespace MazadZone.Application.Features.SellerDashboard.Queries.ExportData;

public sealed record ExportSellerDataQuery(UserId SellerId, string ExportType, SellerDashboardFilter Filter) : IQuery<byte[]>;

public sealed class ExportSellerDataQueryHandler : IQueryHandler<ExportSellerDataQuery, byte[]>
{
    private readonly ISellerDashboardQueries _queries;

    public ExportSellerDataQueryHandler(ISellerDashboardQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<byte[]>> Handle(ExportSellerDataQuery request, CancellationToken cancellationToken)
    {
        var filterWithoutPaging = request.Filter with { Page = 1, PageSize = int.MaxValue };

        var sb = new StringBuilder();

        switch (request.ExportType.ToLower())
        {
            case "auctions":
                var auctionsRes = await _queries.GetSellerAuctionsAsync(request.SellerId, filterWithoutPaging, cancellationToken);
                sb.AppendLine("AuctionId,Title,Category,Status,BidsCount,LastBidAmount,EndDateUtc");
                if (auctionsRes?.Auctions != null)
                {
                    foreach (var a in auctionsRes.Auctions)
                    {
                        sb.AppendLine($"{a.AuctionId},\"{Escape(a.Title)}\",\"{Escape(a.Category)}\",{a.Status},{a.BidsCount},{a.LastBidAmount},{a.EndDateUtc:O}");
                    }
                }
                break;

            case "orders":
                var ordersRes = await _queries.GetSellerOrdersAsync(request.SellerId, filterWithoutPaging, cancellationToken);
                sb.AppendLine("OrderId,AuctionId,AuctionTitle,BuyerName,OrderStatus,OrderDateUtc,TotalAmount");
                if (ordersRes?.Orders != null)
                {
                    foreach (var o in ordersRes.Orders)
                    {
                        sb.AppendLine($"{o.OrderId},{o.AuctionId},\"{Escape(o.AuctionTitle)}\",\"{Escape(o.BuyerName)}\",{o.OrderStatus},{o.OrderDateUtc:O},{o.TotalAmount}");
                    }
                }
                break;

            case "financials":
                var finRes = await _queries.GetSellerFinancialsAsync(request.SellerId, filterWithoutPaging, cancellationToken);
                sb.AppendLine("TotalGrossRevenue,TotalPlatformFees,TotalNetProfit,CompletedOrdersCount");
                if (finRes != null)
                {
                    sb.AppendLine($"{finRes.TotalGrossRevenue},{finRes.TotalPlatformFees},{finRes.TotalNetProfit},{finRes.CompletedOrdersCount}");
                }
                break;

            default:
                return Result.Failure<byte[]>(Error.Validation("Export.InvalidType", "Invalid export type. Supported types: auctions, orders, financials"));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private string Escape(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Replace("\"", "\"\"");
    }
}
