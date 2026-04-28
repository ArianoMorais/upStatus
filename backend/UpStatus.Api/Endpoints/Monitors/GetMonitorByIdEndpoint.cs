using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Features.Monitors.GetById;

namespace UpStatus.Api.Endpoints.Monitors;

public sealed class GetMonitorByIdEndpoint : EndpointWithoutRequest<MonitorResponse>
{
    public override void Configure()
    {
        Get("/monitors/{id:guid}");
        Description(b => b.WithTags("Monitors"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var response = await new GetMonitorByIdQuery(id).ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
