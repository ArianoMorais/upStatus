using FastEndpoints;
using UpStatus.Api.Common;
using UpStatus.Api.Contracts.Requests.Incidents;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Features.Incidents.AddComment;

namespace UpStatus.Api.Endpoints.Incidents;

public sealed class AddIncidentCommentEndpoint : Endpoint<AddIncidentCommentRequest, IncidentResponse>
{
    public override void Configure()
    {
        Post("/incidents/{id:guid}/comments");
        Description(b => b.WithTags("Incidents"));
    }

    public override async Task HandleAsync(AddIncidentCommentRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var userId = CurrentUserAccessor.GetUserId(User);
        var userName = CurrentUserAccessor.GetUserName(User);

        var command = new AddIncidentCommentCommand(id, userId, userName ?? string.Empty, req.Message);
        var response = await command.ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
