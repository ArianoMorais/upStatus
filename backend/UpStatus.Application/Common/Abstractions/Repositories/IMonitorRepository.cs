using UpStatus.Domain.Monitors;
using Monitor = UpStatus.Domain.Monitors.Monitor;

namespace UpStatus.Application.Common.Abstractions.Repositories;

public interface IMonitorRepository
{
    Task<Monitor?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<Monitor>> ListAsync(CancellationToken ct);

    Task<bool> ExistsByUrlAsync(string url, Guid? ignoreId, CancellationToken ct);

    Task AddAsync(Monitor monitor, CancellationToken ct);

    Task UpdateAsync(Monitor monitor, CancellationToken ct);

    Task DeleteAsync(Guid id, CancellationToken ct);
}
