using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;

namespace UpStatus.Application.Features.Monitors.GetById;

public sealed class GetMonitorByIdQueryHandler : ICommandHandler<GetMonitorByIdQuery, MonitorResponse>
{
    private readonly IMonitorRepository _monitors;

    public GetMonitorByIdQueryHandler(IMonitorRepository monitors)
    {
        _monitors = monitors;
    }

    public async Task<MonitorResponse> ExecuteAsync(GetMonitorByIdQuery query, CancellationToken ct)
    {
        var monitor = await _monitors.GetByIdAsync(query.Id, ct)
            ?? throw new BusinessException("monitors.not_found", "Monitor não encontrado.")
            {
                StatusCode = 404
            };

        return monitor.ToMonitorResponse();
    }
}
