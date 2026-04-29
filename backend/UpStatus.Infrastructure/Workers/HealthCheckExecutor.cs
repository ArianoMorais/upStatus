using FastEndpoints;
using Microsoft.Extensions.Logging;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Features.Checks.Record;

namespace UpStatus.Infrastructure.Workers;

public sealed class HealthCheckExecutor
{
    private readonly IMonitorRepository _monitors;
    private readonly IHttpProbeClient _probe;
    private readonly ICommandHandler<RecordCheckCommand, EmptyResponse> _recordHandler;
    private readonly ILogger<HealthCheckExecutor> _logger;

    public HealthCheckExecutor(
        IMonitorRepository monitors,
        IHttpProbeClient probe,
        ICommandHandler<RecordCheckCommand, EmptyResponse> recordHandler,
        ILogger<HealthCheckExecutor> logger)
    {
        _monitors = monitors;
        _probe = probe;
        _recordHandler = recordHandler;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid monitorId, CancellationToken ct)
    {
        var monitor = await _monitors.GetByIdAsync(monitorId, ct);

        if (monitor is null || monitor.IsPaused)
        {
            return;
        }

        try
        {
            var outcome = await _probe.ProbeAsync(monitor.Url, monitor.Config, ct);
            await _recordHandler.ExecuteAsync(new RecordCheckCommand(monitor.Id, outcome), ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao executar check do monitor {MonitorId}.", monitorId);
        }
    }
}
