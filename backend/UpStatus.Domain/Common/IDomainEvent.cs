namespace UpStatus.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
