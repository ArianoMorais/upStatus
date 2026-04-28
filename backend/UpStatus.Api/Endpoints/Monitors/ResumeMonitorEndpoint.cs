using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Features.Monitors.Resume;

namespace UpStatus.Api.Endpoints.Monitors;

public sealed class ResumeMonitorEndpoint : EndpointWithoutRequest<MonitorResponse>
{
    public override void Configure()
    {
        Post("/monitors/{id:guid}/resume");
        Description(b => b.WithTags("Monitors"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var response = await new ResumeMonitorCommand(id).ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
