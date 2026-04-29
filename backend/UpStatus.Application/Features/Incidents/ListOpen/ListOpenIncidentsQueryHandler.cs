using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;

namespace UpStatus.Application.Features.Incidents.ListOpen;

public sealed class ListOpenIncidentsQueryHandler : ICommandHandler<ListOpenIncidentsQuery, IReadOnlyList<IncidentResponse>>
{
    private readonly IIncidentRepository _incidents;

    public ListOpenIncidentsQueryHandler(IIncidentRepository incidents)
    {
        _incidents = incidents;
    }

    public async Task<IReadOnlyList<IncidentResponse>> ExecuteAsync(ListOpenIncidentsQuery query, CancellationToken ct)
    {
        var incidents = await _incidents.ListOpenAsync(ct);
        return incidents.Select(i => i.ToIncidentResponse()).ToList();
    }
}
