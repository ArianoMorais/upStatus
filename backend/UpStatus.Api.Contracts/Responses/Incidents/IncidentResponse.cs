namespace UpStatus.Api.Contracts.Responses.Incidents;

public sealed record IncidentResponse
{
    public Guid Id { get; init; }
    public Guid MonitorId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime StartedAt { get; init; }
    public DateTime? AcknowledgedAt { get; init; }
    public DateTime? ResolvedAt { get; init; }
    public Guid? AcknowledgedBy { get; init; }
    public string? Reason { get; init; }
    public IReadOnlyList<IncidentCommentResponse> Comments { get; init; } = Array.Empty<IncidentCommentResponse>();
}
