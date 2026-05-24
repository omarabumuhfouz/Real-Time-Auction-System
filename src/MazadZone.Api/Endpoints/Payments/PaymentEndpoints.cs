namespace MazadZone.Api.Endpoints.Payments;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var paymentGroup = app.MapGroup("api/v{version:apiVersion}/payments")
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1, 0)
            .WithTags("Payments");

        PayRemaining.MapEndpoint(paymentGroup);
    }
}
