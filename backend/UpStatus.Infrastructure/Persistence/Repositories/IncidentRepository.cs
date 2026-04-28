using MongoDB.Driver;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Domain.Incidents;

namespace UpStatus.Infrastructure.Persistence.Repositories;

public sealed class IncidentRepository : IIncidentRepository
{
    private readonly MongoContext _context;

    public IncidentRepository(MongoContext context)
    {
        _context = context;
    }

    public Task<Incident?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _context.Incidents.Find(i => i.Id == id).FirstOrDefaultAsync(ct)!;

    public Task<Incident?> GetOpenByMonitorAsync(Guid monitorId, CancellationToken ct) =>
        _context.Incidents
            .Find(i => i.MonitorId == monitorId && i.Status != IncidentStatus.Resolved)
            .FirstOrDefaultAsync(ct)!;

    public async Task<IReadOnlyList<Incident>> ListOpenAsync(CancellationToken ct) =>
        await _context.Incidents
            .Find(i => i.Status != IncidentStatus.Resolved)
            .SortByDescending(i => i.StartedAt)
            .ToListAsync(ct);

    public Task AddAsync(Incident incident, CancellationToken ct) =>
        _context.Incidents.InsertOneAsync(incident, cancellationToken: ct);

    public Task UpdateAsync(Incident incident, CancellationToken ct) =>
        _context.Incidents.ReplaceOneAsync(i => i.Id == incident.Id, incident, cancellationToken: ct);
}
