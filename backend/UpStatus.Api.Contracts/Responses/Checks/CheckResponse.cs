namespace UpStatus.Api.Contracts.Responses.Checks;

public sealed record CheckResponse
{
    public Guid Id { get; init; }
    public Guid MonitorId { get; init; }
    public DateTime Timestamp { get; init; }
    public string Result { get; init; } = string.Empty;
    public int? StatusCode { get; init; }
    public int LatencyMs { get; init; }
    public string? ErrorMessage { get; init; }
}
