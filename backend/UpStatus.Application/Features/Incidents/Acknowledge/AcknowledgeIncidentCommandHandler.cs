using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;
using UpStatus.Domain.Incidents;

namespace UpStatus.Application.Features.Incidents.Acknowledge;

public sealed class AcknowledgeIncidentCommandHandler : ICommandHandler<AcknowledgeIncidentCommand, IncidentResponse>
{
    private readonly IIncidentRepository _incidents;
    private readonly IDateTimeProvider _clock;
    private readonly IHubNotifier _hub;

    public AcknowledgeIncidentCommandHandler(
        IIncidentRepository incidents,
        IDateTimeProvider clock,
        IHubNotifier hub)
    {
        _incidents = incidents;
        _clock = clock;
        _hub = hub;
    }

    public async Task<IncidentResponse> ExecuteAsync(AcknowledgeIncidentCommand command, CancellationToken ct)
    {
        var incident = await _incidents.GetByIdAsync(command.IncidentId, ct)
            ?? throw new BusinessException("incidents.not_found", "Incidente não encontrado.")
            {
                StatusCode = 404
            };

        if (incident.Status == IncidentStatus.Resolved)
        {
            throw new BusinessException("incidents.already_resolved", "Incidente já foi resolvido.")
            {
                StatusCode = 409
            };
        }

        if (incident.Status == IncidentStatus.Acknowledged)
        {
            return incident.ToIncidentResponse();
        }

        incident.Status = IncidentStatus.Acknowledged;
        incident.AcknowledgedAt = _clock.UtcNow;
        incident.AcknowledgedBy = command.UserId;

        await _incidents.UpdateAsync(incident, ct);

        var response = incident.ToIncidentResponse();
        await _hub.NotifyIncidentAcknowledgedAsync(response, ct);
        return response;
    }
}
