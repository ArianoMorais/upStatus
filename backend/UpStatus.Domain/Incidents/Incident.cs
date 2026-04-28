namespace UpStatus.Domain.Incidents;

public sealed class Incident
{
    public Guid Id { get; set; }
    public Guid MonitorId { get; set; }
    public IncidentStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Guid? AcknowledgedBy { get; set; }
    public string? Reason { get; set; }
    public List<IncidentComment> Comments { get; set; } = new();
}
