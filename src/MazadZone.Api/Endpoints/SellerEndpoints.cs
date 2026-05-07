using MazadZone.Domain.Auctions; // Assuming BidderId is here
using MazadZone.Application.Features.Sellers.Commands.BecomeSeller;
using MazadZone.Api.Contracts.Sellers;
using MechanicShop.Api.Extensions;
using MazadZone.Application.Features.Sellers.Commands.UpdateBankDetails;
using MazadZone.Application.Features.Sellers.Commands.Verify;
using MazadZone.Application.Features.Sellers.Queries.GetPrivateDetails;
using MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;
using MazadZone.Application.Features.Sellers.Queries.GetUnverifiedSellers;

namespace MazadZone.Api.Features.Sellers;

public static class SellerEndpoints
{
    public static void MapSellerEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                                    .HasApiVersion(new ApiVersion(1, 0))
                                    .ReportApiVersions()
                                    .Build();

        var sellerGroup = app.MapGroup("api/v{version:apiVersion}/sellers")
                       .WithApiVersionSet(versionSet)
                       .MapToApiVersion(1, 0)
                       .WithTags("Sellers Queries");



        sellerGroup.MapPost("{id}/become-seller", BecomeSellerAsync)
            .WithSummary("Promotes a bidder to a seller")
            .WithDescription("Registers a bidder as a seller by providing bank details.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        sellerGroup.MapPatch("{id}/bank-details", UpdateBankDetailsAsync)
            .WithSummary("Updates seller bank account details")
            .WithDescription("Allows an existing seller to update their bank account number for payouts.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        sellerGroup.MapPost("{id}/verify", VerifySellerAsync)
            .WithSummary("Verifies a seller")
            .WithDescription("Updates the seller status to verified, allowing them to participate in auctions.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        sellerGroup.MapGet("{id}/private", GetPrivateSellerDetailsAsync)
            .WithSummary("Retrieves private details of a seller")
            .WithDescription("Gets full profile information including bank details and contact info. Requires authorization.")
            .Produces<PrivateSellerDetailsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        sellerGroup.MapGet("{id}/public", GetPublicSellerProfileAsync)
            .WithSummary("Retrieves a seller's public profile")
            .WithDescription("Gets public-facing information about a seller, such as their name and rating.")
            .Produces<PublicSellerProfileResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        sellerGroup.MapGet("unverified", GetUnverifiedSellersAsync)
            .WithSummary("Retrieves all unverified sellers")
            .WithDescription("Fetches a list of sellers who have registered but have not yet been verified by an administrator.")
            .Produces<IReadOnlyList<UnverifiedSellerSummaryResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

    }

    private static async Task<IResult> BecomeSellerAsync(
        BidderId id, // Minimal API uses your TryParse conversion here
        [FromBody] BecomeSellerRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new BecomeSellerCommand(id, request.BankAccountNumber);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());

    }

    private static async Task<IResult> UpdateBankDetailsAsync(
        SellerId id, // Direct conversion from {id} route param
        [FromBody] UpdateBankDetailsRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBankDetailsCommand(id, request.NewAccountNumber);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> VerifySellerAsync(
        SellerId id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        // The id is automatically bound from the route {id} thanks to your conversion binding
        var command = new VerifyCommand(id);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> GetPrivateSellerDetailsAsync(
        SellerId id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        // SellerId id is automatically bound from {id} route param
        var query = new GetPrivateSellerDetailsQuery(id);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            onValue: response => Results.Ok(response),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> GetPublicSellerProfileAsync(
        SellerId id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        // The id is automatically bound from the route {id} via conversion binding
        var query = new GetPublicSellerProfileQuery(id);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            onValue: response => Results.Ok(response),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> GetUnverifiedSellersAsync(
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        // No parameters needed for this query
        var query = new GetUnverifiedSellersQuery();

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            onValue: sellers => Results.Ok(sellers),
            onError: errors => errors.ToProblem());
    }

}