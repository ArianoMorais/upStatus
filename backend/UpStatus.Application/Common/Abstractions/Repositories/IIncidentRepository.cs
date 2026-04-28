using UpStatus.Domain.Incidents;

namespace UpStatus.Application.Common.Abstractions.Repositories;

public interface IIncidentRepository
{
    Task<Incident?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<Incident?> GetOpenByMonitorAsync(Guid monitorId, CancellationToken ct);

    Task<IReadOnlyList<Incident>> ListOpenAsync(CancellationToken ct);

    Task AddAsync(Incident incident, CancellationToken ct);

    Task UpdateAsync(Incident incident, CancellationToken ct);
}
