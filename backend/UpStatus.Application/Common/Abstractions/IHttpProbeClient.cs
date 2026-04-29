using UpStatus.Domain.Checks;
using UpStatus.Domain.Monitors;

namespace UpStatus.Application.Common.Abstractions;

public interface IHttpProbeClient
{
    Task<ProbeOutcome> ProbeAsync(string url, MonitorConfig config, CancellationToken ct);
}

public sealed record ProbeOutcome
{
    public CheckResult Result { get; init; }
    public int? StatusCode { get; init; }
    public int LatencyMs { get; init; }
    public string? ErrorMessage { get; init; }
}
