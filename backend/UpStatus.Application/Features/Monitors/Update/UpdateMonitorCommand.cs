using FastEndpoints;
using UpStatus.Api.Contracts.Requests.Monitors;
using UpStatus.Api.Contracts.Responses.Monitors;

namespace UpStatus.Application.Features.Monitors.Update;

public sealed record UpdateMonitorCommand : ICommand<MonitorResponse>
{
    public Guid Id { get; }
    public string Name { get; }
    public string Url { get; }
    public MonitorConfigRequest Config { get; }

    public UpdateMonitorCommand(Guid id, string name, string url, MonitorConfigRequest config)
    {
        Id = id;
        Name = name;
        Url = url;
        Config = config;
    }
}
