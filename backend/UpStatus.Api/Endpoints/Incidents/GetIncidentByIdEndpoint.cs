using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Features.Incidents.GetById;

namespace UpStatus.Api.Endpoints.Incidents;

public sealed class GetIncidentByIdEndpoint : EndpointWithoutRequest<IncidentResponse>
{
    public override void Configure()
    {
        Get("/incidents/{id:guid}");
        Description(b => b.WithTags("Incidents"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var response = await new GetIncidentByIdQuery(id).ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
