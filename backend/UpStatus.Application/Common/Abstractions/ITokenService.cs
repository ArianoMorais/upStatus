using UpStatus.Domain.Users;

namespace UpStatus.Application.Common.Abstractions;

public interface ITokenService
{
    GeneratedToken Generate(User user);
}

public sealed record GeneratedToken(string AccessToken, DateTime ExpiresAt);
