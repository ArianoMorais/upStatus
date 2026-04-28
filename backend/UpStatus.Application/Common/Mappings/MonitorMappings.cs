using UpStatus.Api.Contracts.Requests.Monitors;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Domain.Monitors;
using DomainMonitor = UpStatus.Domain.Monitors.Monitor;

namespace UpStatus.Application.Common.Mappings;

public static class MonitorMappings
{
    public static MonitorResponse ToMonitorResponse(this DomainMonitor monitor) => new()
    {
        Id = monitor.Id,
        Name = monitor.Name,
        Url = monitor.Url,
        Status = monitor.Status.ToString(),
        IsPaused = monitor.IsPaused,
        CreatedAt = monitor.CreatedAt,
        LastCheckedAt = monitor.LastCheckedAt,
        CreatedBy = monitor.CreatedBy,
        Config = monitor.Config.ToMonitorConfigResponse()
    };

    public static MonitorConfigResponse ToMonitorConfigResponse(this MonitorConfig config) => new()
    {
        IntervalSeconds = config.IntervalSeconds,
        TimeoutMs = config.TimeoutMs,
        FailuresToOpenIncident = config.FailuresToOpenIncident,
        SuccessesToCloseIncident = config.SuccessesToCloseIncident,
        DegradedLatencyMs = config.DegradedLatencyMs,
        ExpectedStatusCode = config.ExpectedStatusCode,
        HttpMethod = config.HttpMethod
    };

    public static MonitorConfig ToMonitorConfig(this MonitorConfigRequest request) => new()
    {
        IntervalSeconds = request.IntervalSeconds,
        TimeoutMs = request.TimeoutMs,
        FailuresToOpenIncident = request.FailuresToOpenIncident,
        SuccessesToCloseIncident = request.SuccessesToCloseIncident,
        DegradedLatencyMs = request.DegradedLatencyMs,
        ExpectedStatusCode = request.ExpectedStatusCode,
        HttpMethod = request.HttpMethod.ToUpperInvariant()
    };
}
