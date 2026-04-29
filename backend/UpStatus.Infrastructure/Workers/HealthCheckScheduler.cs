using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using DomainMonitor = UpStatus.Domain.Monitors.Monitor;

namespace UpStatus.Infrastructure.Workers;

public sealed class HealthCheckScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WorkerOptions _options;
    private readonly ILogger<HealthCheckScheduler> _logger;

    public HealthCheckScheduler(
        IServiceScopeFactory scopeFactory,
        IOptions<WorkerOptions> options,
        ILogger<HealthCheckScheduler> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HealthCheckScheduler iniciado (tick {Tick} ms, concorrência máx {Max}).",
            _options.TickIntervalMs, _options.MaxConcurrentChecks);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await TickAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no tick do worker.");
            }

            try
            {
                await Task.Delay(_options.TickIntervalMs, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task TickAsync(CancellationToken ct)
    {
        IReadOnlyList<DomainMonitor> monitors;
        DateTime now;

        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IMonitorRepository>();
            var clock = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
            monitors = await repo.ListAsync(ct);
            now = clock.UtcNow;
        }

        var due = monitors.Where(m => !m.IsPaused && IsDue(m, now)).ToList();

        if (due.Count == 0)
        {
            return;
        }

        using var semaphore = new SemaphoreSlim(_options.MaxConcurrentChecks);
        var tasks = due.Select(monitor => RunSingleAsync(monitor.Id, semaphore, ct));
        await Task.WhenAll(tasks);
    }

    private async Task RunSingleAsync(Guid monitorId, SemaphoreSlim semaphore, CancellationToken ct)
    {
        await semaphore.WaitAsync(ct);

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var executor = scope.ServiceProvider.GetRequiredService<HealthCheckExecutor>();
            await executor.ExecuteAsync(monitorId, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao processar monitor {MonitorId}.", monitorId);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static bool IsDue(DomainMonitor monitor, DateTime now)
    {
        if (monitor.LastCheckedAt is null)
        {
            return true;
        }

        return (now - monitor.LastCheckedAt.Value).TotalSeconds >= monitor.Config.IntervalSeconds;
    }
}
