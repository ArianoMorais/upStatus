using FastEndpoints;
using UpStatus.Application.Common.Abstractions;

namespace UpStatus.Application.Features.Checks.Record;

public sealed record RecordCheckCommand : ICommand<EmptyResponse>
{
    public Guid MonitorId { get; }
    public ProbeOutcome Outcome { get; }

    public RecordCheckCommand(Guid monitorId, ProbeOutcome outcome)
    {
        MonitorId = monitorId;
        Outcome = outcome;
    }
}
