using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Checks;
using UpStatus.Application.Features.Checks.GetMonitorChecks;

namespace UpStatus.Api.Endpoints.Monitors;

public sealed class GetMonitorChecksEndpoint : EndpointWithoutRequest<MonitorChecksResponse>
{
    public override void Configure()
    {
        Get("/monitors/{id:guid}/checks");
        Description(b => b.WithTags("Monitors"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var from = Query<DateTime?>("from", isRequired: false);
        var to = Query<DateTime?>("to", isRequired: false);
        var limit = Query<int?>("limit", isRequired: false) ?? 0;

        var response = await new GetMonitorChecksQuery(id, from, to, limit).ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
