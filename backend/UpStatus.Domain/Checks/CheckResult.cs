namespace UpStatus.Domain.Checks;

public enum CheckResult
{
    Success = 0,
    Failure = 1,
    Timeout = 2,
    Degraded = 3
}
