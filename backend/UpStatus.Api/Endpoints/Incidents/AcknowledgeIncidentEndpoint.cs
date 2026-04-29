using FastEndpoints;
using UpStatus.Api.Common;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Features.Incidents.Acknowledge;

namespace UpStatus.Api.Endpoints.Incidents;

public sealed class AcknowledgeIncidentEndpoint : EndpointWithoutRequest<IncidentResponse>
{
    public override void Configure()
    {
        Post("/incidents/{id:guid}/acknowledge");
        Description(b => b.WithTags("Incidents"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var userId = CurrentUserAccessor.GetUserId(User);

        var response = await new AcknowledgeIncidentCommand(id, userId).ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
