namespace UpStatus.Domain.Monitors;

public enum MonitorStatus
{
    Up = 0,
    Down = 1,
    Degraded = 2,
    Paused = 3,
    Unknown = 4
}
