using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Checks;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;

namespace UpStatus.Application.Features.Checks.GetMonitorChecks;

public sealed class GetMonitorChecksQueryHandler : ICommandHandler<GetMonitorChecksQuery, MonitorChecksResponse>
{
    private const int DefaultLimit = 100;
    private const int MaxLimit = 1000;

    private readonly IMonitorRepository _monitors;
    private readonly ICheckRepository _checks;
    private readonly IDateTimeProvider _clock;

    public GetMonitorChecksQueryHandler(
        IMonitorRepository monitors,
        ICheckRepository checks,
        IDateTimeProvider clock)
    {
        _monitors = monitors;
        _checks = checks;
        _clock = clock;
    }

    public async Task<MonitorChecksResponse> ExecuteAsync(GetMonitorChecksQuery query, CancellationToken ct)
    {
        var monitor = await _monitors.GetByIdAsync(query.MonitorId, ct)
            ?? throw new BusinessException("monitors.not_found", "Monitor não encontrado.")
            {
                StatusCode = 404
            };

        var to = query.To ?? _clock.UtcNow;
        var from = query.From ?? to.AddHours(-24);

        if (from >= to)
        {
            throw new BusinessException("checks.invalid_range", "Intervalo inválido: 'from' deve ser anterior a 'to'.")
            {
                StatusCode = 400
            };
        }

        var limit = query.Limit <= 0 ? DefaultLimit : Math.Min(query.Limit, MaxLimit);

        var items = await _checks.ListByMonitorAsync(monitor.Id, from, to, limit, ct);
        var total = await _checks.CountByMonitorAsync(monitor.Id, from, to, ct);

        return new MonitorChecksResponse
        {
            MonitorId = monitor.Id,
            From = from,
            To = to,
            Total = total,
            Items = items.Select(c => c.ToCheckResponse()).ToList()
        };
    }
}
