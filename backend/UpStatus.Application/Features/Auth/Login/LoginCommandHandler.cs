using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Auth;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;

namespace UpStatus.Application.Features.Auth.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly IDateTimeProvider _clock;

    public LoginCommandHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        ITokenService tokens,
        IDateTimeProvider clock)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
        _clock = clock;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginCommand command, CancellationToken ct)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, ct);

        if (user is null || !_hasher.Verify(command.Password, user.PasswordHash))
        {
            throw new BusinessException("auth.invalid_credentials", "E-mail ou senha inválidos.")
            {
                StatusCode = 401
            };
        }

        user.LastLoginAt = _clock.UtcNow;
        await _users.UpdateAsync(user, ct);

        var token = _tokens.Generate(user);

        return new LoginResponse
        {
            AccessToken = token.AccessToken,
            ExpiresAt = token.ExpiresAt,
            User = user.ToAuthUserResponse()
        };
    }
}
