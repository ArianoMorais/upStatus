using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Incidents;

namespace UpStatus.Application.Features.Incidents.ListOpen;

public sealed record ListOpenIncidentsQuery : ICommand<IReadOnlyList<IncidentResponse>>;
