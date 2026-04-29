using UpStatus.Api.Contracts.Responses.Checks;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Api.Contracts.Responses.Monitors;

namespace UpStatus.Application.Common.Abstractions;

public interface IHubNotifier
{
    Task NotifyMonitorStatusChangedAsync(MonitorResponse monitor, string previousStatus, CancellationToken ct);

    Task NotifyCheckRecordedAsync(CheckResponse check, CancellationToken ct);

    Task NotifyIncidentOpenedAsync(IncidentResponse incident, CancellationToken ct);

    Task NotifyIncidentResolvedAsync(IncidentResponse incident, CancellationToken ct);

    Task NotifyIncidentAcknowledgedAsync(IncidentResponse incident, CancellationToken ct);

    Task NotifyIncidentCommentedAsync(IncidentResponse incident, IncidentCommentResponse comment, CancellationToken ct);
}

public sealed class NullHubNotifier : IHubNotifier
{
    public Task NotifyMonitorStatusChangedAsync(MonitorResponse monitor, string previousStatus, CancellationToken ct) => Task.CompletedTask;

    public Task NotifyCheckRecordedAsync(CheckResponse check, CancellationToken ct) => Task.CompletedTask;

    public Task NotifyIncidentOpenedAsync(IncidentResponse incident, CancellationToken ct) => Task.CompletedTask;

    public Task NotifyIncidentResolvedAsync(IncidentResponse incident, CancellationToken ct) => Task.CompletedTask;

    public Task NotifyIncidentAcknowledgedAsync(IncidentResponse incident, CancellationToken ct) => Task.CompletedTask;

    public Task NotifyIncidentCommentedAsync(IncidentResponse incident, IncidentCommentResponse comment, CancellationToken ct) => Task.CompletedTask;
}
