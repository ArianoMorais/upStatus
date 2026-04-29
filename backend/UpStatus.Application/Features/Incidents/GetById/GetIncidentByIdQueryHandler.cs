using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;

namespace UpStatus.Application.Features.Incidents.GetById;

public sealed class GetIncidentByIdQueryHandler : ICommandHandler<GetIncidentByIdQuery, IncidentResponse>
{
    private readonly IIncidentRepository _incidents;

    public GetIncidentByIdQueryHandler(IIncidentRepository incidents)
    {
        _incidents = incidents;
    }

    public async Task<IncidentResponse> ExecuteAsync(GetIncidentByIdQuery query, CancellationToken ct)
    {
        var incident = await _incidents.GetByIdAsync(query.Id, ct)
            ?? throw new BusinessException("incidents.not_found", "Incidente não encontrado.")
            {
                StatusCode = 404
            };

        return incident.ToIncidentResponse();
    }
}
