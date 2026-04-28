using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Features.Monitors.List;

namespace UpStatus.Api.Endpoints.Monitors;

public sealed class ListMonitorsEndpoint : EndpointWithoutRequest<IReadOnlyList<MonitorResponse>>
{
    public override void Configure()
    {
        Get("/monitors");
        Description(b => b.WithTags("Monitors"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await new ListMonitorsQuery().ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
