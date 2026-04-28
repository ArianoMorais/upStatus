using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;

namespace UpStatus.Application.Features.Monitors.GetById;

public sealed record GetMonitorByIdQuery : ICommand<MonitorResponse>
{
    public Guid Id { get; }

    public GetMonitorByIdQuery(Guid id)
    {
        Id = id;
    }
}
