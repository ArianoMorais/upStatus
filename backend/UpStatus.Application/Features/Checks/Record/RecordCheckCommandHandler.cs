using FastEndpoints;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Caching;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Checks;
using UpStatus.Domain.Monitors;

namespace UpStatus.Application.Features.Checks.Record;

public sealed class RecordCheckCommandHandler : ICommandHandler<RecordCheckCommand, EmptyResponse>
{
    private readonly IMonitorRepository _monitors;
    private readonly ICheckRepository _checks;
    private readonly ICacheService _cache;
    private readonly IDateTimeProvider _clock;
    private readonly IIncidentTrigger _incidents;
    private readonly IHubNotifier _hub;

    public RecordCheckCommandHandler(
        IMonitorRepository monitors,
        ICheckRepository checks,
        ICacheService cache,
        IDateTimeProvider clock,
        IIncidentTrigger incidents,
        IHubNotifier hub)
    {
        _monitors = monitors;
        _checks = checks;
        _cache = cache;
        _clock = clock;
        _incidents = incidents;
        _hub = hub;
    }

    public async Task<EmptyResponse> ExecuteAsync(RecordCheckCommand command, CancellationToken ct)
    {
        var monitor = await _monitors.GetByIdAsync(command.MonitorId, ct);

        if (monitor is null || monitor.IsPaused)
        {
            return EmptyResponse.Instance;
        }

        var now = _clock.UtcNow;
        var outcome = command.Outcome;
        var previousStatus = monitor.Status;

        var check = new Check
        {
            Id = Guid.NewGuid(),
            MonitorId = monitor.Id,
            Timestamp = now,
            Result = outcome.Result,
            StatusCode = outcome.StatusCode,
            LatencyMs = outcome.LatencyMs,
            ErrorMessage = outcome.ErrorMessage
        };

        await _checks.AddAsync(check, ct);

        var newStatus = MapStatus(outcome.Result);
        monitor.LastCheckedAt = now;
        monitor.Status = newStatus;
        await _monitors.UpdateAsync(monitor, ct);

        await _cache.SetAsync(
            MonitorCacheKeys.Status(monitor.Id),
            newStatus.ToString(),
            ttl: null,
            ct);

        await _hub.NotifyCheckRecordedAsync(check.ToCheckResponse(), ct);

        if (newStatus != previousStatus)
        {
            await _hub.NotifyMonitorStatusChangedAsync(monitor.ToMonitorResponse(), previousStatus.ToString(), ct);
        }

        await _incidents.EvaluateAsync(monitor, check, ct);

        return EmptyResponse.Instance;
    }

    private static MonitorStatus MapStatus(CheckResult result) => result switch
    {
        CheckResult.Success => MonitorStatus.Up,
        CheckResult.Degraded => MonitorStatus.Degraded,
        CheckResult.Failure => MonitorStatus.Down,
        CheckResult.Timeout => MonitorStatus.Down,
        _ => MonitorStatus.Unknown
    };
}
