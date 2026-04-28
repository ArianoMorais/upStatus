using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;

namespace UpStatus.Application.Features.Monitors.Resume;

public sealed record ResumeMonitorCommand : ICommand<MonitorResponse>
{
    public Guid Id { get; }

    public ResumeMonitorCommand(Guid id)
    {
        Id = id;
    }
}
