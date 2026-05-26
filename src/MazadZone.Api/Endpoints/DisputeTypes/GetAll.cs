namespace MazadZone.Api.Endpoints.DisputeTypes;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MazadZone.Features.DisputeTypes.Queries.GetAll;

public static class GetAll
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandleAsync)
        //    .RequireAuthorization()
           .WithSummary("Get all Dispute Types")
           .WithDescription("Retrieves a complete list of all configured dispute types in the system.")
           .Produces<IReadOnlyList<DisputeTypeDto>>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetAllDisputeTypesQuery(), ct);

        return result.Match(
            onValue: values => Results.Ok(values),
            onError: errors => errors.ToProblem());
    }
}