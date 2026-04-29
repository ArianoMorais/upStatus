using FastEndpoints;
using Microsoft.Extensions.Options;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Caching;
using UpStatus.Application.Features.Incidents.Open;
using UpStatus.Application.Features.Incidents.Resolve;
using UpStatus.Domain.Checks;
using UpStatus.Infrastructure.Workers;
using DomainMonitor = UpStatus.Domain.Monitors.Monitor;

namespace UpStatus.Infrastructure.Incidents;

public sealed class ThresholdIncidentTrigger : IIncidentTrigger
{
    private readonly ICheckRepository _checks;
    private readonly IIncidentRepository _incidents;
    private readonly ICacheService _cache;
    private readonly WorkerOptions _workerOptions;
    private readonly ICommandHandler<OpenIncidentCommand, IncidentResponse> _openHandler;
    private readonly ICommandHandler<ResolveIncidentCommand, IncidentResponse> _resolveHandler;

    public ThresholdIncidentTrigger(
        ICheckRepository checks,
        IIncidentRepository incidents,
        ICacheService cache,
        IOptions<WorkerOptions> workerOptions,
        ICommandHandler<OpenIncidentCommand, IncidentResponse> openHandler,
        ICommandHandler<ResolveIncidentCommand, IncidentResponse> resolveHandler)
    {
        _checks = checks;
        _incidents = incidents;
        _cache = cache;
        _workerOptions = workerOptions.Value;
        _openHandler = openHandler;
        _resolveHandler = resolveHandler;
    }

    public async Task EvaluateAsync(DomainMonitor monitor, Check check, CancellationToken ct)
    {
        var failuresThreshold = monitor.Config.FailuresToOpenIncident;
        var successesThreshold = monitor.Config.SuccessesToCloseIncident;
        var window = Math.Max(failuresThreshold, successesThreshold);

        var recent = await _checks.GetLastNAsync(monitor.Id, window, ct);

        if (recent.Count == 0)
        {
            return;
        }

        var open = await _incidents.GetOpenByMonitorAsync(monitor.Id, ct);

        if (open is null)
        {
            if (recent.Count < failuresThreshold)
            {
                return;
            }

            var lastFailures = recent.Take(failuresThreshold).ToList();
            var allFailures = lastFailures.All(c => c.Result is CheckResult.Failure or CheckResult.Timeout);

            if (!allFailures)
            {
                return;
            }

            var cooldownKey = IncidentCacheKeys.Cooldown(monitor.Id);
            var cooldown = await _cache.GetAsync<string>(cooldownKey, ct);

            if (!string.IsNullOrEmpty(cooldown))
            {
                return;
            }

            var reason = check.ErrorMessage
                ?? (check.StatusCode.HasValue ? $"Status code {check.StatusCode.Value}." : "Falhas consecutivas detectadas.");

            await _openHandler.ExecuteAsync(new OpenIncidentCommand(monitor.Id, reason), ct);

            await _cache.SetAsync(
                cooldownKey,
                "1",
                TimeSpan.FromSeconds(_workerOptions.DefaultCooldownSeconds),
                ct);

            return;
        }

        if (recent.Count < successesThreshold)
        {
            return;
        }

        var lastSuccesses = recent.Take(successesThreshold).ToList();
        var allRecovered = lastSuccesses.All(c => c.Result is CheckResult.Success or CheckResult.Degraded);

        if (allRecovered)
        {
            await _resolveHandler.ExecuteAsync(new ResolveIncidentCommand(open.Id, userId: null, userName: null, comment: null), ct);
            await _cache.RemoveAsync(IncidentCacheKeys.Cooldown(monitor.Id), ct);
        }
    }
}
