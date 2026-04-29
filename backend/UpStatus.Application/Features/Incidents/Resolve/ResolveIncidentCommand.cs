using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;

namespace UpStatus.Application.Features.Incidents.Resolve;

public sealed record ResolveIncidentCommand : ICommand<IncidentResponse>
{
    public Guid IncidentId { get; }
    public Guid? UserId { get; }
    public string? UserName { get; }
    public string? Comment { get; }

    public ResolveIncidentCommand(Guid incidentId, Guid? userId, string? userName, string? comment)
    {
        IncidentId = incidentId;
        UserId = userId;
        UserName = userName;
        Comment = comment;
    }
}
