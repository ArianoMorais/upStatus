using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Caching;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;
using UpStatus.Domain.Monitors;
using DomainMonitor = UpStatus.Domain.Monitors.Monitor;

namespace UpStatus.Application.Features.Monitors.Create;

public sealed class CreateMonitorCommandHandler : ICommandHandler<CreateMonitorCommand, CreateMonitorResponse>
{
    private readonly IMonitorRepository _monitors;
    private readonly ICacheService _cache;
    private readonly IDateTimeProvider _clock;

    public CreateMonitorCommandHandler(
        IMonitorRepository monitors,
        ICacheService cache,
        IDateTimeProvider clock)
    {
        _monitors = monitors;
        _cache = cache;
        _clock = clock;
    }

    public async Task<CreateMonitorResponse> ExecuteAsync(CreateMonitorCommand command, CancellationToken ct)
    {
        var url = command.Url.Trim();

        if (await _monitors.ExistsByUrlAsync(url, ignoreId: null, ct))
        {
            throw new BusinessException("monitors.url_already_exists", "Já existe um monitor cadastrado para esta URL.")
            {
                StatusCode = 409
            };
        }

        var monitor = new DomainMonitor
        {
            Id = Guid.NewGuid(),
            Name = command.Name.Trim(),
            Url = url,
            Config = command.Config.ToMonitorConfig(),
            Status = MonitorStatus.Unknown,
            IsPaused = false,
            CreatedAt = _clock.UtcNow,
            CreatedBy = command.CreatedBy
        };

        await _monitors.AddAsync(monitor, ct);

        await _cache.SetAsync(
            MonitorCacheKeys.Status(monitor.Id),
            monitor.Status.ToString(),
            ttl: null,
            ct);

        return new CreateMonitorResponse { Id = monitor.Id };
    }
}
