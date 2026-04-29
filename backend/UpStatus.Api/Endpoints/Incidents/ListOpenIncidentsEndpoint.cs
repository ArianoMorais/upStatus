using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Features.Incidents.ListOpen;

namespace UpStatus.Api.Endpoints.Incidents;

public sealed class ListOpenIncidentsEndpoint : EndpointWithoutRequest<IReadOnlyList<IncidentResponse>>
{
    public override void Configure()
    {
        Get("/incidents");
        Description(b => b.WithTags("Incidents"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await new ListOpenIncidentsQuery().ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
