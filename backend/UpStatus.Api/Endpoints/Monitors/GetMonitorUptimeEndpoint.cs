using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Checks;
using UpStatus.Application.Features.Checks.GetMonitorUptime;

namespace UpStatus.Api.Endpoints.Monitors;

public sealed class GetMonitorUptimeEndpoint : EndpointWithoutRequest<MonitorUptimeResponse>
{
    public override void Configure()
    {
        Get("/monitors/{id:guid}/uptime");
        Description(b => b.WithTags("Monitors"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var range = Query<string?>("range", isRequired: false) ?? "24h";

        var response = await new GetMonitorUptimeQuery(id, range).ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
