using FastEndpoints;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Caching;
using UpStatus.Domain.Common;

namespace UpStatus.Application.Features.Monitors.Delete;

public sealed class DeleteMonitorCommandHandler : ICommandHandler<DeleteMonitorCommand, EmptyResponse>
{
    private readonly IMonitorRepository _monitors;
    private readonly ICacheService _cache;

    public DeleteMonitorCommandHandler(IMonitorRepository monitors, ICacheService cache)
    {
        _monitors = monitors;
        _cache = cache;
    }

    public async Task<EmptyResponse> ExecuteAsync(DeleteMonitorCommand command, CancellationToken ct)
    {
        var monitor = await _monitors.GetByIdAsync(command.Id, ct)
            ?? throw new BusinessException("monitors.not_found", "Monitor não encontrado.")
            {
                StatusCode = 404
            };

        await _monitors.DeleteAsync(monitor.Id, ct);
        await _cache.RemoveAsync(MonitorCacheKeys.Status(monitor.Id), ct);

        return EmptyResponse.Instance;
    }
}
