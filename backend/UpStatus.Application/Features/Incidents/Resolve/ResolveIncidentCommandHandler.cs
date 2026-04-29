using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;
using UpStatus.Domain.Incidents;

namespace UpStatus.Application.Features.Incidents.Resolve;

public sealed class ResolveIncidentCommandHandler : ICommandHandler<ResolveIncidentCommand, IncidentResponse>
{
    private readonly IIncidentRepository _incidents;
    private readonly IDateTimeProvider _clock;
    private readonly IHubNotifier _hub;

    public ResolveIncidentCommandHandler(
        IIncidentRepository incidents,
        IDateTimeProvider clock,
        IHubNotifier hub)
    {
        _incidents = incidents;
        _clock = clock;
        _hub = hub;
    }

    public async Task<IncidentResponse> ExecuteAsync(ResolveIncidentCommand command, CancellationToken ct)
    {
        var incident = await _incidents.GetByIdAsync(command.IncidentId, ct)
            ?? throw new BusinessException("incidents.not_found", "Incidente não encontrado.")
            {
                StatusCode = 404
            };

        if (incident.Status == IncidentStatus.Resolved)
        {
            return incident.ToIncidentResponse();
        }

        var now = _clock.UtcNow;

        if (!string.IsNullOrWhiteSpace(command.Comment) && command.UserId.HasValue)
        {
            incident.Comments.Add(new IncidentComment
            {
                AuthorId = command.UserId.Value,
                AuthorName = command.UserName ?? string.Empty,
                Message = command.Comment.Trim(),
                CreatedAt = now
            });
        }

        incident.Status = IncidentStatus.Resolved;
        incident.ResolvedAt = now;

        await _incidents.UpdateAsync(incident, ct);

        var response = incident.ToIncidentResponse();
        await _hub.NotifyIncidentResolvedAsync(response, ct);
        return response;
    }
}
