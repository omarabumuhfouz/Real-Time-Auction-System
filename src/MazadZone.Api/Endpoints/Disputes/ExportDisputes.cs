namespace MazadZone.Api.Endpoints.Disputes;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MazadZone.Application.Features.Disputes.Queries.ExportDisputes;

// 1. The Request Record
public record ExportDisputesRequest(
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
    public ExportDisputesQuery ToQuery() => 
        new(SearchTerm, Status, CategoryId, FromDate, ToDate, SortColumn, IsDescending, PageNumber, PageSize);
}

// 2. The Endpoint
public static class ExportDisputes
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/export", HandleAsync)
           // .RequireAuthorization("AdminOnly") 
           .WithSummary("Export disputes to CSV")
           .WithDescription("Generates and downloads a CSV file of filtered disputes bypassing pagination limits.")
           .Produces(StatusCodes.Status200OK, contentType: "text/csv")
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status415UnsupportedMediaType)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] ExportDisputesRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        // Send the parameters directly into our dedicated export pipeline using ToQuery()
        var result = await sender.Send(request.ToQuery(), ct);

        return result.Match(
            fileResponse => Results.File(
                fileResponse.FileContents, 
                fileResponse.ContentType, 
                fileResponse.FileName),
            error => error.ToProblem());
    }
}