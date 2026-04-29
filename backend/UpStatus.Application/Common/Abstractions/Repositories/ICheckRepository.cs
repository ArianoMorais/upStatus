using UpStatus.Domain.Checks;

namespace UpStatus.Application.Common.Abstractions.Repositories;

public interface ICheckRepository
{
    Task AddAsync(Check check, CancellationToken ct);

    Task<IReadOnlyList<Check>> ListByMonitorAsync(
        Guid monitorId,
        DateTime from,
        DateTime to,
        int limit,
        CancellationToken ct);

    Task<long> CountByMonitorAsync(Guid monitorId, DateTime from, DateTime to, CancellationToken ct);

    Task<IReadOnlyList<Check>> GetLastNAsync(Guid monitorId, int n, CancellationToken ct);
}
