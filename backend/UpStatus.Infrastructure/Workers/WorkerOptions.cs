namespace UpStatus.Infrastructure.Workers;

public sealed class WorkerOptions
{
    public int TickIntervalMs { get; set; } = 1000;
    public int MaxConcurrentChecks { get; set; } = 20;
    public int DefaultCooldownSeconds { get; set; } = 300;
}
