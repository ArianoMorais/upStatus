using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Caching;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;
using UpStatus.Domain.Monitors;

namespace UpStatus.Application.Features.Monitors.Pause;

public sealed class PauseMonitorCommandHandler : ICommandHandler<PauseMonitorCommand, MonitorResponse>
{
    private readonly IMonitorRepository _monitors;
    private readonly ICacheService _cache;

    public PauseMonitorCommandHandler(IMonitorRepository monitors, ICacheService cache)
    {
        _monitors = monitors;
        _cache = cache;
    }

    public async Task<MonitorResponse> ExecuteAsync(PauseMonitorCommand command, CancellationToken ct)
    {
        var monitor = await _monitors.GetByIdAsync(command.Id, ct)
            ?? throw new BusinessException("monitors.not_found", "Monitor não encontrado.")
            {
                StatusCode = 404
            };

        if (monitor.IsPaused)
        {
            return monitor.ToMonitorResponse();
        }

        monitor.IsPaused = true;
        monitor.Status = MonitorStatus.Paused;

        await _monitors.UpdateAsync(monitor, ct);

        await _cache.SetAsync(
            MonitorCacheKeys.Status(monitor.Id),
            monitor.Status.ToString(),
            ttl: null,
            ct);

        return monitor.ToMonitorResponse();
    }
}
