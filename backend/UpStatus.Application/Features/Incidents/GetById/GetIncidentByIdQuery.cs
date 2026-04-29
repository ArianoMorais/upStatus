using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;

namespace UpStatus.Application.Features.Incidents.GetById;

public sealed record GetIncidentByIdQuery : ICommand<IncidentResponse>
{
    public Guid Id { get; }

    public GetIncidentByIdQuery(Guid id)
    {
        Id = id;
    }
}
