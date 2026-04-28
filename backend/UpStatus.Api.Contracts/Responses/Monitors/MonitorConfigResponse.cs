namespace UpStatus.Api.Contracts.Responses.Monitors;

public sealed record MonitorConfigResponse
{
    public int IntervalSeconds { get; init; }
    public int TimeoutMs { get; init; }
    public int FailuresToOpenIncident { get; init; }
    public int SuccessesToCloseIncident { get; init; }
    public int DegradedLatencyMs { get; init; }
    public int? ExpectedStatusCode { get; init; }
    public string HttpMethod { get; init; } = "GET";
}
