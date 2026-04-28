namespace UpStatus.Api.Contracts.Requests.Monitors;

public sealed record MonitorConfigRequest
{
    public int IntervalSeconds { get; init; } = 60;
    public int TimeoutMs { get; init; } = 5000;
    public int FailuresToOpenIncident { get; init; } = 3;
    public int SuccessesToCloseIncident { get; init; } = 2;
    public int DegradedLatencyMs { get; init; } = 1500;
    public int? ExpectedStatusCode { get; init; }
    public string HttpMethod { get; init; } = "GET";
}
