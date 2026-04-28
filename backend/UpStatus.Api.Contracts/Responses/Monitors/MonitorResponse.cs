namespace UpStatus.Api.Contracts.Responses.Monitors;

public sealed record MonitorResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsPaused { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastCheckedAt { get; init; }
    public Guid CreatedBy { get; init; }
    public MonitorConfigResponse Config { get; init; } = new();
}
