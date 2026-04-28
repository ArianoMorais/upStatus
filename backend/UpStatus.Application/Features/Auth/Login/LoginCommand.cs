using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Auth;

namespace UpStatus.Application.Features.Auth.Login;

public sealed record LoginCommand : ICommand<LoginResponse>
{
    public string Email { get; }
    public string Password { get; }

    public LoginCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}
