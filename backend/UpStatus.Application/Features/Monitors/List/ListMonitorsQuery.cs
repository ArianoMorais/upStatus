using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;

namespace UpStatus.Application.Features.Monitors.List;

public sealed record ListMonitorsQuery : ICommand<IReadOnlyList<MonitorResponse>>;
