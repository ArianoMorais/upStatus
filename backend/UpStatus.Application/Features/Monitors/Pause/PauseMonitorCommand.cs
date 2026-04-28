using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;

namespace UpStatus.Application.Features.Monitors.Pause;

public sealed record PauseMonitorCommand : ICommand<MonitorResponse>
{
    public Guid Id { get; }

    public PauseMonitorCommand(Guid id)
    {
        Id = id;
    }
}
