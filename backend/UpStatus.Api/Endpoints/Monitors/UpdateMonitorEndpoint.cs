using FastEndpoints;
using UpStatus.Api.Contracts.Requests.Monitors;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Features.Monitors.Update;

namespace UpStatus.Api.Endpoints.Monitors;

public sealed class UpdateMonitorEndpoint : Endpoint<UpdateMonitorRequest, MonitorResponse>
{
    public override void Configure()
    {
        Put("/monitors/{id:guid}");
        Description(b => b.WithTags("Monitors"));
    }

    public override async Task HandleAsync(UpdateMonitorRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var command = new UpdateMonitorCommand(id, req.Name, req.Url, req.Config);
        var response = await command.ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
