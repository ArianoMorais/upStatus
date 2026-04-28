using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Features.Monitors.Pause;

namespace UpStatus.Api.Endpoints.Monitors;

public sealed class PauseMonitorEndpoint : EndpointWithoutRequest<MonitorResponse>
{
    public override void Configure()
    {
        Post("/monitors/{id:guid}/pause");
        Description(b => b.WithTags("Monitors"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var response = await new PauseMonitorCommand(id).ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
