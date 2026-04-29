namespace UpStatus.Api.Contracts.Responses.Checks;

public sealed record MonitorUptimeResponse
{
    public Guid MonitorId { get; init; }
    public string Range { get; init; } = string.Empty;
    public DateTime From { get; init; }
    public DateTime To { get; init; }
    public long Total { get; init; }
    public long Successful { get; init; }
    public long Degraded { get; init; }
    public long Failed { get; init; }
    public double UptimePercent { get; init; }
    public int? P50LatencyMs { get; init; }
    public int? P95LatencyMs { get; init; }
    public int? P99LatencyMs { get; init; }
}
