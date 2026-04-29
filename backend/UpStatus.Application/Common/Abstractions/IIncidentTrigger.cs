using UpStatus.Domain.Checks;
using DomainMonitor = UpStatus.Domain.Monitors.Monitor;

namespace UpStatus.Application.Common.Abstractions;

public interface IIncidentTrigger
{
    Task EvaluateAsync(DomainMonitor monitor, Check check, CancellationToken ct);
}

public sealed class NullIncidentTrigger : IIncidentTrigger
{
    public Task EvaluateAsync(DomainMonitor monitor, Check check, CancellationToken ct) => Task.CompletedTask;
}
