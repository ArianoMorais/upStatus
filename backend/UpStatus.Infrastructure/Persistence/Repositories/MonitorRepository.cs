using MongoDB.Driver;
using UpStatus.Application.Common.Abstractions.Repositories;
using DomainMonitor = UpStatus.Domain.Monitors.Monitor;

namespace UpStatus.Infrastructure.Persistence.Repositories;

public sealed class MonitorRepository : IMonitorRepository
{
    private readonly MongoContext _context;

    public MonitorRepository(MongoContext context)
    {
        _context = context;
    }

    public Task<DomainMonitor?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _context.Monitors.Find(m => m.Id == id).FirstOrDefaultAsync(ct)!;

    public async Task<IReadOnlyList<DomainMonitor>> ListAsync(CancellationToken ct) =>
        await _context.Monitors
            .Find(FilterDefinition<DomainMonitor>.Empty)
            .SortByDescending(m => m.CreatedAt)
            .ToListAsync(ct);

    public async Task<bool> ExistsByUrlAsync(string url, Guid? ignoreId, CancellationToken ct)
    {
        var filter = Builders<DomainMonitor>.Filter.Eq(m => m.Url, url);

        if (ignoreId.HasValue)
        {
            filter &= Builders<DomainMonitor>.Filter.Ne(m => m.Id, ignoreId.Value);
        }

        var count = await _context.Monitors.CountDocumentsAsync(filter, cancellationToken: ct);
        return count > 0;
    }

    public Task AddAsync(DomainMonitor monitor, CancellationToken ct) =>
        _context.Monitors.InsertOneAsync(monitor, cancellationToken: ct);

    public Task UpdateAsync(DomainMonitor monitor, CancellationToken ct) =>
        _context.Monitors.ReplaceOneAsync(m => m.Id == monitor.Id, monitor, cancellationToken: ct);

    public Task DeleteAsync(Guid id, CancellationToken ct) =>
        _context.Monitors.DeleteOneAsync(m => m.Id == id, ct);
}
