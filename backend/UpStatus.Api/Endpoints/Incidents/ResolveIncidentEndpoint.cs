using FastEndpoints;
using UpStatus.Api.Common;
using UpStatus.Api.Contracts.Requests.Incidents;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Features.Incidents.Resolve;

namespace UpStatus.Api.Endpoints.Incidents;

public sealed class ResolveIncidentEndpoint : Endpoint<ResolveIncidentRequest, IncidentResponse>
{
    public override void Configure()
    {
        Post("/incidents/{id:guid}/resolve");
        Description(b => b.WithTags("Incidents"));
    }

    public override async Task HandleAsync(ResolveIncidentRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var userId = CurrentUserAccessor.GetUserId(User);
        var userName = CurrentUserAccessor.GetUserName(User);

        var command = new ResolveIncidentCommand(id, userId, userName, req.Comment);
        var response = await command.ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
