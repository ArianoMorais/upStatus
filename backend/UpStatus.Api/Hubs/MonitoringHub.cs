using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace UpStatus.Api.Hubs;

[Authorize]
public sealed class MonitoringHub : Hub
{
    public const string Path = "/hubs/monitoring";

    public const string MonitorStatusChanged = "monitor.status_changed";
    public const string CheckRecorded = "check.recorded";
    public const string IncidentOpened = "incident.opened";
    public const string IncidentResolved = "incident.resolved";
    public const string IncidentAcknowledged = "incident.acknowledged";
    public const string IncidentCommented = "incident.commented";
}
