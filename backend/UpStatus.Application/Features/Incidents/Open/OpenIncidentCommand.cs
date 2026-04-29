using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;

namespace UpStatus.Application.Features.Incidents.Open;

public sealed record OpenIncidentCommand : ICommand<IncidentResponse>
{
    public Guid MonitorId { get; }
    public string? Reason { get; }

    public OpenIncidentCommand(Guid monitorId, string? reason)
    {
        MonitorId = monitorId;
        Reason = reason;
    }
}
