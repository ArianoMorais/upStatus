using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;

namespace UpStatus.Application.Features.Monitors.Update;

public sealed class UpdateMonitorCommandHandler : ICommandHandler<UpdateMonitorCommand, MonitorResponse>
{
    private readonly IMonitorRepository _monitors;

    public UpdateMonitorCommandHandler(IMonitorRepository monitors)
    {
        _monitors = monitors;
    }

    public async Task<MonitorResponse> ExecuteAsync(UpdateMonitorCommand command, CancellationToken ct)
    {
        var monitor = await _monitors.GetByIdAsync(command.Id, ct)
            ?? throw new BusinessException("monitors.not_found", "Monitor não encontrado.")
            {
                StatusCode = 404
            };

        var url = command.Url.Trim();

        if (await _monitors.ExistsByUrlAsync(url, ignoreId: monitor.Id, ct))
        {
            throw new BusinessException("monitors.url_already_exists", "Já existe outro monitor cadastrado para esta URL.")
            {
                StatusCode = 409
            };
        }

        monitor.Name = command.Name.Trim();
        monitor.Url = url;
        monitor.Config = command.Config.ToMonitorConfig();

        await _monitors.UpdateAsync(monitor, ct);

        return monitor.ToMonitorResponse();
    }
}
