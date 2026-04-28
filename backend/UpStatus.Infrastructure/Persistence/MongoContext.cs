using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UpStatus.Domain.Checks;
using UpStatus.Domain.Incidents;
using UpStatus.Domain.Users;
using DomainMonitor = UpStatus.Domain.Monitors.Monitor;

namespace UpStatus.Infrastructure.Persistence;

public sealed class MongoContext
{
    public IMongoDatabase Database { get; }

    public MongoContext(IOptions<MongoOptions> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        Database = client.GetDatabase(settings.Database);
    }

    public IMongoCollection<User> Users => Database.GetCollection<User>("users");
    public IMongoCollection<DomainMonitor> Monitors => Database.GetCollection<DomainMonitor>("monitors");
    public IMongoCollection<Check> Checks => Database.GetCollection<Check>("checks");
    public IMongoCollection<Incident> Incidents => Database.GetCollection<Incident>("incidents");
}
