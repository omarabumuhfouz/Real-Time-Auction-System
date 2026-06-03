namespace MazadZone.Api.Endpoints.Disputes;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MediatR;
using MazadZone.Application.Features.Disputes.Queries;
using MazadZone.Application.Features.Disputes.Queries.GetFiltered;

public record GetFilteredDisputesRequest(
    [FromQuery] string? SearchTerm,
    [FromQuery] string? Status,
    [FromQuery] Guid? CategoryId,
    [FromQuery] DateTime? FromDate = null,
    [FromQuery] DateTime? ToDate = null,
    [FromQuery] string? SortColumn = "SubmittedDate",
    [FromQuery] bool IsDescending = true,
    [FromQuery] int PageNumber = 1,
    [FromQuery] int PageSize = 10

)
{
    public GetFilteredDisputesQuery ToQuery() => 
        new(SearchTerm, Status, CategoryId, FromDate, ToDate, SortColumn, IsDescending, PageNumber, PageSize);
}

// 2. The Endpoint
public static class GetFiltered
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Assuming your group route is already "/disputes"
        app.MapGet("/", HandleAsync)
           // .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Get a filtered and sorted list of disputes")
           .WithDescription("Retrieves a read-only list of disputes. Supports searching by name/category, filtering by status/date, and sorting.")
           .Produces<IReadOnlyList<DisputeListItemDto>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Triggers if FluentValidation fails
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetFilteredDisputesRequest request, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(request.ToQuery(), ct);

        return result.Match(
            onValue: disputes => Results.Ok(disputes),
            onError: errors => errors.ToProblem());
    }
}