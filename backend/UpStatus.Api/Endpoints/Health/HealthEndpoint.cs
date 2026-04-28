using FastEndpoints;

namespace UpStatus.Api.Endpoints.Health;

public sealed class HealthEndpoint : EndpointWithoutRequest<HealthResponse>
{
    public override void Configure()
    {
        Get("/health");
        AllowAnonymous();
        Description(b => b.WithTags("Health"));
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return Send.OkAsync(new HealthResponse("ok", DateTime.UtcNow), cancellation: ct);
    }
}

public sealed record HealthResponse(string Status, DateTime Timestamp);
