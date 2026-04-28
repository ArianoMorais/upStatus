namespace UpStatus.Domain.Monitors;

public sealed class Monitor
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public MonitorConfig Config { get; set; } = new();
    public MonitorStatus Status { get; set; }
    public bool IsPaused { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
