using Microsoft.AspNetCore.SignalR;
using UpStatus.Api.Contracts.Responses.Checks;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Common.Abstractions;

namespace UpStatus.Api.Hubs;

public sealed class HubNotifier : IHubNotifier
{
    private readonly IHubContext<MonitoringHub> _hub;

    public HubNotifier(IHubContext<MonitoringHub> hub)
    {
        _hub = hub;
    }

    public Task NotifyMonitorStatusChangedAsync(MonitorResponse monitor, string previousStatus, CancellationToken ct) =>
        _hub.Clients.All.SendAsync(
            MonitoringHub.MonitorStatusChanged,
            new { monitor, previousStatus },
            ct);

    public Task NotifyCheckRecordedAsync(CheckResponse check, CancellationToken ct) =>
        _hub.Clients.All.SendAsync(MonitoringHub.CheckRecorded, check, ct);

    public Task NotifyIncidentOpenedAsync(IncidentResponse incident, CancellationToken ct) =>
        _hub.Clients.All.SendAsync(MonitoringHub.IncidentOpened, incident, ct);

    public Task NotifyIncidentResolvedAsync(IncidentResponse incident, CancellationToken ct) =>
        _hub.Clients.All.SendAsync(MonitoringHub.IncidentResolved, incident, ct);

    public Task NotifyIncidentAcknowledgedAsync(IncidentResponse incident, CancellationToken ct) =>
        _hub.Clients.All.SendAsync(MonitoringHub.IncidentAcknowledged, incident, ct);

    public Task NotifyIncidentCommentedAsync(IncidentResponse incident, IncidentCommentResponse comment, CancellationToken ct) =>
        _hub.Clients.All.SendAsync(
            MonitoringHub.IncidentCommented,
            new { incident, comment },
            ct);
}
