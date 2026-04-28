using MongoDB.Driver;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Domain.Checks;

namespace UpStatus.Infrastructure.Persistence.Repositories;

public sealed class CheckRepository : ICheckRepository
{
    private readonly MongoContext _context;

    public CheckRepository(MongoContext context)
    {
        _context = context;
    }

    public Task AddAsync(Check check, CancellationToken ct) =>
        _context.Checks.InsertOneAsync(check, cancellationToken: ct);

    public async Task<IReadOnlyList<Check>> ListByMonitorAsync(Guid monitorId, DateTime from, DateTime to, CancellationToken ct)
    {
        var filter = Builders<Check>.Filter.And(
            Builders<Check>.Filter.Eq(c => c.MonitorId, monitorId),
            Builders<Check>.Filter.Gte(c => c.Timestamp, from),
            Builders<Check>.Filter.Lt(c => c.Timestamp, to));

        return await _context.Checks
            .Find(filter)
            .SortByDescending(c => c.Timestamp)
            .ToListAsync(ct);
    }
}
