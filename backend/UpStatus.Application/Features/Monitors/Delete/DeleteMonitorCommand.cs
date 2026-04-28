using FastEndpoints;

namespace UpStatus.Application.Features.Monitors.Delete;

public sealed record DeleteMonitorCommand : ICommand<EmptyResponse>
{
    public Guid Id { get; }

    public DeleteMonitorCommand(Guid id)
    {
        Id = id;
    }
}
