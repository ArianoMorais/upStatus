using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;

namespace UpStatus.Application.Features.Incidents.AddComment;

public sealed record AddIncidentCommentCommand : ICommand<IncidentResponse>
{
    public Guid IncidentId { get; }
    public Guid UserId { get; }
    public string UserName { get; }
    public string Message { get; }

    public AddIncidentCommentCommand(Guid incidentId, Guid userId, string userName, string message)
    {
        IncidentId = incidentId;
        UserId = userId;
        UserName = userName;
        Message = message;
    }
}
