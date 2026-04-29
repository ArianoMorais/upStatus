using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Checks;

namespace UpStatus.Application.Features.Checks.GetMonitorChecks;

public sealed record GetMonitorChecksQuery : ICommand<MonitorChecksResponse>
{
    public Guid MonitorId { get; }
    public DateTime? From { get; }
    public DateTime? To { get; }
    public int Limit { get; }

    public GetMonitorChecksQuery(Guid monitorId, DateTime? from, DateTime? to, int limit)
    {
        MonitorId = monitorId;
        From = from;
        To = to;
        Limit = limit;
    }
}
