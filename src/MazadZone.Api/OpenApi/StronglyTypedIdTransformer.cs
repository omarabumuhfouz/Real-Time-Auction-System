using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace MazadZone.Api.Infrastructure.OpenApi;

public class StronglyTypedIdTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var type = context.JsonTypeInfo.Type;

        // You can make the check as strict as you want. 
        // Here we ensure it's from your Domain namespace and ends with "Id".
        if (type.IsValueType && 
            type.Namespace?.StartsWith("MazadZone.Domain") == true && 
            type.Name.EndsWith("Id"))
        {
            schema.Type = "string";
            schema.Format = "uuid";
        }

        return Task.CompletedTask;
    }
}