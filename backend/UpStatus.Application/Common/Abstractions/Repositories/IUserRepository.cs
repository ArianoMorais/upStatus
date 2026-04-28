using UpStatus.Domain.Users;

namespace UpStatus.Application.Common.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<User?> GetByEmailAsync(string email, CancellationToken ct);

    Task<bool> AnyAsync(CancellationToken ct);

    Task AddAsync(User user, CancellationToken ct);

    Task UpdateAsync(User user, CancellationToken ct);
}
