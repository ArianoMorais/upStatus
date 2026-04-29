using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Checks;

namespace UpStatus.Application.Features.Checks.GetMonitorUptime;

public sealed record GetMonitorUptimeQuery : ICommand<MonitorUptimeResponse>
{
    public Guid MonitorId { get; }
    public string Range { get; }

    public GetMonitorUptimeQuery(Guid monitorId, string range)
    {
        MonitorId = monitorId;
        Range = range;
    }
}
