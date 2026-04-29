using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;
using UpStatus.Domain.Incidents;

namespace UpStatus.Application.Features.Incidents.Open;

public sealed class OpenIncidentCommandHandler : ICommandHandler<OpenIncidentCommand, IncidentResponse>
{
    private readonly IIncidentRepository _incidents;
    private readonly IMonitorRepository _monitors;
    private readonly IDateTimeProvider _clock;
    private readonly IHubNotifier _hub;

    public OpenIncidentCommandHandler(
        IIncidentRepository incidents,
        IMonitorRepository monitors,
        IDateTimeProvider clock,
        IHubNotifier hub)
    {
        _incidents = incidents;
        _monitors = monitors;
        _clock = clock;
        _hub = hub;
    }

    public async Task<IncidentResponse> ExecuteAsync(OpenIncidentCommand command, CancellationToken ct)
    {
        var monitor = await _monitors.GetByIdAsync(command.MonitorId, ct)
            ?? throw new BusinessException("monitors.not_found", "Monitor não encontrado.")
            {
                StatusCode = 404
            };

        var existing = await _incidents.GetOpenByMonitorAsync(monitor.Id, ct);

        if (existing is not null)
        {
            return existing.ToIncidentResponse();
        }

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            MonitorId = monitor.Id,
            Status = IncidentStatus.Open,
            StartedAt = _clock.UtcNow,
            Reason = command.Reason
        };

        await _incidents.AddAsync(incident, ct);

        var response = incident.ToIncidentResponse();
        await _hub.NotifyIncidentOpenedAsync(response, ct);
        return response;
    }
}
