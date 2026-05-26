using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Auctions;
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Auctions.Queries;
using MazadZone.Application.Features.Auctions.Queries.GetAuctions;
using MazadZone.Domain.Categories;
using MediatR;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;

namespace MazadZone.Api.Endpoints.Auctions;

public static class GetAuctions
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandleAsync)
            .WithName("GetAuctions")
            .WithOpenApi()
            .WithSummary("Search and filter auctions")
            .Produces<PagedList<AuctionsListDto>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] AuctionQueryParameters parameters,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        System.Console.WriteLine($"\n\n\nBound Parameter Status: {parameters.Status}\n\n\n");

        var query = new GetAuctionsQuery(
            parameters.Page == 0 ? 1 : parameters.Page,
            parameters.PageSize == 0 ? 12 : parameters.PageSize,
            parameters.SearchTerm,
            parameters.CategoryId,
            parameters.SubCategoryId,
            parameters.MinCurrentBid is null && parameters.MaxCurrentBid is null
                ? null
                : new CurrentBidAmountRange { Min = parameters.MinCurrentBid, Max = parameters.MaxCurrentBid },
            string.IsNullOrEmpty(parameters.Status) ? null : parameters.Status,
            string.IsNullOrEmpty(parameters.SortBy) ? "CreationDate" : parameters.SortBy,
            string.IsNullOrEmpty(parameters.SortDirection) ? "desc" : parameters.SortDirection,
            string.IsNullOrEmpty(parameters.ItemStatus) ? null : parameters.ItemStatus,
            string.IsNullOrEmpty(parameters.Condition) ? null : parameters.Condition);
        var result = await sender.Send(query, ct);
        return result.Match(onValue: value => Results.Ok(value), onError: e => e.ToProblem());
    }

    // Create a flat parameter group structure
    public class AuctionQueryParameters
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
        public CategoryId? CategoryId { get; set; }
        public CategoryId? SubCategoryId { get; set; }
        public decimal? MinCurrentBid { get; set; }
        public decimal? MaxCurrentBid { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public string? ItemStatus { get; set; }
        public string? Condition { get; set; }
    }
}
