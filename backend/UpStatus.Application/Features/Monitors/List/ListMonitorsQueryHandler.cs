using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;

namespace UpStatus.Application.Features.Monitors.List;

public sealed class ListMonitorsQueryHandler : ICommandHandler<ListMonitorsQuery, IReadOnlyList<MonitorResponse>>
{
    private readonly IMonitorRepository _monitors;

    public ListMonitorsQueryHandler(IMonitorRepository monitors)
    {
        _monitors = monitors;
    }

    public async Task<IReadOnlyList<MonitorResponse>> ExecuteAsync(ListMonitorsQuery query, CancellationToken ct)
    {
        var monitors = await _monitors.ListAsync(ct);
        return monitors.Select(m => m.ToMonitorResponse()).ToList();
    }
}
