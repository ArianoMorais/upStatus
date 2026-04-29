using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;

namespace UpStatus.Application.Features.Incidents.Acknowledge;

public sealed record AcknowledgeIncidentCommand : ICommand<IncidentResponse>
{
    public Guid IncidentId { get; }
    public Guid UserId { get; }

    public AcknowledgeIncidentCommand(Guid incidentId, Guid userId)
    {
        IncidentId = incidentId;
        UserId = userId;
    }
}
