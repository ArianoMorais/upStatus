using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Checks;
using UpStatus.Application.Common;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Domain.Checks;
using UpStatus.Domain.Common;

namespace UpStatus.Application.Features.Checks.GetMonitorUptime;

public sealed class GetMonitorUptimeQueryHandler : ICommandHandler<GetMonitorUptimeQuery, MonitorUptimeResponse>
{
    private const int SampleLimit = 10000;

    private readonly IMonitorRepository _monitors;
    private readonly ICheckRepository _checks;
    private readonly IDateTimeProvider _clock;

    public GetMonitorUptimeQueryHandler(
        IMonitorRepository monitors,
        ICheckRepository checks,
        IDateTimeProvider clock)
    {
        _monitors = monitors;
        _checks = checks;
        _clock = clock;
    }

    public async Task<MonitorUptimeResponse> ExecuteAsync(GetMonitorUptimeQuery query, CancellationToken ct)
    {
        var monitor = await _monitors.GetByIdAsync(query.MonitorId, ct)
            ?? throw new BusinessException("monitors.not_found", "Monitor não encontrado.")
            {
                StatusCode = 404
            };

        if (!UptimeRange.TryParse(query.Range, out var duration, out var range))
        {
            throw new BusinessException("checks.invalid_range", "Range inválido. Use 1h, 24h, 7d ou 30d.")
            {
                StatusCode = 400
            };
        }

        var to = _clock.UtcNow;
        var from = to - duration;

        var samples = await _checks.ListByMonitorAsync(monitor.Id, from, to, SampleLimit, ct);
        var total = samples.Count;

        if (total == 0)
        {
            return new MonitorUptimeResponse
            {
                MonitorId = monitor.Id,
                Range = range,
                From = from,
                To = to
            };
        }

        var successful = samples.Count(c => c.Result == CheckResult.Success);
        var degraded = samples.Count(c => c.Result == CheckResult.Degraded);
        var failed = total - successful - degraded;

        var availableCount = successful + degraded;
        var uptimePercent = Math.Round((double)availableCount / total * 100d, 2);

        var latencies = samples
            .Where(c => c.Result is CheckResult.Success or CheckResult.Degraded)
            .Select(c => c.LatencyMs)
            .OrderBy(v => v)
            .ToArray();

        return new MonitorUptimeResponse
        {
            MonitorId = monitor.Id,
            Range = range,
            From = from,
            To = to,
            Total = total,
            Successful = successful,
            Degraded = degraded,
            Failed = failed,
            UptimePercent = uptimePercent,
            P50LatencyMs = Percentile(latencies, 0.50),
            P95LatencyMs = Percentile(latencies, 0.95),
            P99LatencyMs = Percentile(latencies, 0.99)
        };
    }

    private static int? Percentile(int[] sorted, double p)
    {
        if (sorted.Length == 0)
        {
            return null;
        }

        var rank = (int)Math.Ceiling(p * sorted.Length) - 1;
        rank = Math.Clamp(rank, 0, sorted.Length - 1);
        return sorted[rank];
    }
}
