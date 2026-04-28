using MongoDB.Driver;
using UpStatus.Domain.Checks;
using UpStatus.Domain.Incidents;
using UpStatus.Domain.Users;
using DomainMonitor = UpStatus.Domain.Monitors.Monitor;

namespace UpStatus.Infrastructure.Persistence;

public sealed class MongoIndexInitializer
{
    private readonly MongoContext _context;

    public MongoIndexInitializer(MongoContext context)
    {
        _context = context;
    }

    public async Task EnsureIndexesAsync(CancellationToken ct)
    {
        await _context.Users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true, Name = "ux_users_email" }),
            cancellationToken: ct);

        await _context.Monitors.Indexes.CreateOneAsync(
            new CreateIndexModel<DomainMonitor>(
                Builders<DomainMonitor>.IndexKeys.Ascending(m => m.CreatedAt),
                new CreateIndexOptions { Name = "ix_monitors_createdAt" }),
            cancellationToken: ct);

        await _context.Checks.Indexes.CreateOneAsync(
            new CreateIndexModel<Check>(
                Builders<Check>.IndexKeys
                    .Ascending(c => c.MonitorId)
                    .Descending(c => c.Timestamp),
                new CreateIndexOptions { Name = "ix_checks_monitor_ts" }),
            cancellationToken: ct);

        await _context.Incidents.Indexes.CreateOneAsync(
            new CreateIndexModel<Incident>(
                Builders<Incident>.IndexKeys
                    .Ascending(i => i.MonitorId)
                    .Ascending(i => i.Status),
                new CreateIndexOptions { Name = "ix_incidents_monitor_status" }),
            cancellationToken: ct);
    }
}
