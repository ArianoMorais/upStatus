using MongoDB.Driver;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Domain.Users;

namespace UpStatus.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly MongoContext _context;

    public UserRepository(MongoContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync(ct)!;

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync(ct)!;

    public async Task<bool> AnyAsync(CancellationToken ct) =>
        await _context.Users.Find(FilterDefinition<User>.Empty).AnyAsync(ct);

    public Task AddAsync(User user, CancellationToken ct) =>
        _context.Users.InsertOneAsync(user, cancellationToken: ct);

    public Task UpdateAsync(User user, CancellationToken ct) =>
        _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: ct);
}
