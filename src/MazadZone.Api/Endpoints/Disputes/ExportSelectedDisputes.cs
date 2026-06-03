namespace MazadZone.Api.Endpoints.Disputes;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MazadZone.Application.Features.Disputes.Queries.ExportSelectedDisputes;

public static class ExportSelectedDisputes
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/export/selected", HandleAsync)
           .WithSummary("Export specific disputes by IDs to CSV")
           .WithDescription("Accepts a JSON array of Dispute IDs and generates a downloaded CSV file.")
           .Produces(StatusCodes.Status200OK, contentType: "text/csv")
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] List<Guid> selectedIds,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ExportSelectedDisputesQuery(selectedIds), ct);

        return result.Match(
            fileResponse => Results.File(
                fileResponse.FileContents, 
                fileResponse.ContentType, 
                fileResponse.FileName),
            error => error.ToProblem());
    }
}