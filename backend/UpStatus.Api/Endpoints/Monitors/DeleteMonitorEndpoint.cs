using FastEndpoints;
using UpStatus.Application.Features.Monitors.Delete;

namespace UpStatus.Api.Endpoints.Monitors;

public sealed class DeleteMonitorEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/monitors/{id:guid}");
        Description(b => b.WithTags("Monitors"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        await new DeleteMonitorCommand(id).ExecuteAsync(ct);
        await Send.NoContentAsync(ct);
    }
}
