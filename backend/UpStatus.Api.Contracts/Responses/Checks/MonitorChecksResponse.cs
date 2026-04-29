namespace UpStatus.Api.Contracts.Responses.Checks;

public sealed record MonitorChecksResponse
{
    public Guid MonitorId { get; init; }
    public DateTime From { get; init; }
    public DateTime To { get; init; }
    public long Total { get; init; }
    public IReadOnlyList<CheckResponse> Items { get; init; } = Array.Empty<CheckResponse>();
}
