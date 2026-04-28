namespace UpStatus.Domain.Common;

public class BusinessException : Exception
{
    public string Code { get; }
    public int StatusCode { get; init; } = 400;

    public BusinessException(string code, string message) : base(message)
    {
        Code = code;
    }
}
