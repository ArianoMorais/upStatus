namespace UpStatus.Api.Contracts.Requests.Monitors;

public sealed record UpdateMonitorRequest
{
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public MonitorConfigRequest Config { get; init; } = new();
}
