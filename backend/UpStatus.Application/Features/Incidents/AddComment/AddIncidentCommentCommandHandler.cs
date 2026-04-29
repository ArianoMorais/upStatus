using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;
using UpStatus.Domain.Incidents;

namespace UpStatus.Application.Features.Incidents.AddComment;

public sealed class AddIncidentCommentCommandHandler : ICommandHandler<AddIncidentCommentCommand, IncidentResponse>
{
    private readonly IIncidentRepository _incidents;
    private readonly IDateTimeProvider _clock;
    private readonly IHubNotifier _hub;

    public AddIncidentCommentCommandHandler(
        IIncidentRepository incidents,
        IDateTimeProvider clock,
        IHubNotifier hub)
    {
        _incidents = incidents;
        _clock = clock;
        _hub = hub;
    }

    public async Task<IncidentResponse> ExecuteAsync(AddIncidentCommentCommand command, CancellationToken ct)
    {
        var incident = await _incidents.GetByIdAsync(command.IncidentId, ct)
            ?? throw new BusinessException("incidents.not_found", "Incidente não encontrado.")
            {
                StatusCode = 404
            };

        var comment = new IncidentComment
        {
            AuthorId = command.UserId,
            AuthorName = command.UserName,
            Message = command.Message.Trim(),
            CreatedAt = _clock.UtcNow
        };

        incident.Comments.Add(comment);
        await _incidents.UpdateAsync(incident, ct);

        var response = incident.ToIncidentResponse();
        await _hub.NotifyIncidentCommentedAsync(response, comment.ToIncidentCommentResponse(), ct);
        return response;
    }
}
