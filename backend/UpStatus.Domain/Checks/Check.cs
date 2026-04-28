namespace UpStatus.Domain.Checks;

public sealed class Check
{
    public Guid Id { get; set; }
    public Guid MonitorId { get; set; }
    public DateTime Timestamp { get; set; }
    public CheckResult Result { get; set; }
    public int? StatusCode { get; set; }
    public int LatencyMs { get; set; }
    public string? ErrorMessage { get; set; }
}
