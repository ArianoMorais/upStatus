using FastEndpoints;
using UpStatus.Api.Contracts.Requests.Monitors;
using UpStatus.Api.Contracts.Responses.Monitors;

namespace UpStatus.Application.Features.Monitors.Create;

public sealed record CreateMonitorCommand : ICommand<CreateMonitorResponse>
{
    public string Name { get; }
    public string Url { get; }
    public MonitorConfigRequest Config { get; }
    public Guid CreatedBy { get; }

    public CreateMonitorCommand(string name, string url, MonitorConfigRequest config, Guid createdBy)
    {
        Name = name;
        Url = url;
        Config = config;
        CreatedBy = createdBy;
    }
}
