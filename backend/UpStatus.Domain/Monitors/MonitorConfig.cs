namespace UpStatus.Domain.Monitors;

public sealed class MonitorConfig
{
    public int IntervalSeconds { get; set; } = 60;
    public int TimeoutMs { get; set; } = 5000;
    public int FailuresToOpenIncident { get; set; } = 3;
    public int SuccessesToCloseIncident { get; set; } = 2;
    public int DegradedLatencyMs { get; set; } = 1500;
    public int? ExpectedStatusCode { get; set; }
    public string HttpMethod { get; set; } = "GET";
}
