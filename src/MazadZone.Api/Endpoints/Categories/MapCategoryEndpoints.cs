namespace MazadZone.Api.Endpoints.Categories;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var categoryGroup = app.MapGroup("api/v{version:apiVersion}/categories")
                               .WithApiVersionSet(versionSet)
                               .MapToApiVersion(1, 0)
                               .WithTags("Category Management");

        // Registered Queries
        GetCategoryStatistics.MapEndpoint(categoryGroup);
        GetRoots.MapEndpoint(categoryGroup);       
        GetById.MapEndpoint(categoryGroup);
        GetTree.MapEndpoint(categoryGroup);
        GetSub.MapEndpoint(categoryGroup);        
        GetBreadcrumbs.MapEndpoint(categoryGroup);
        Search.MapEndpoint(categoryGroup); 

        // Registered Commands
        Create.MapEndpoint(categoryGroup);
        Update.MapEndpoint(categoryGroup);
        Delete.MapEndpoint(categoryGroup);
        Restore.MapEndpoint(categoryGroup);
        AddSubCategory.MapEndpoint(categoryGroup);
        MoveToParent.MapEndpoint(categoryGroup);
        MakeRoot.MapEndpoint(categoryGroup);
    }
}