using FastEndpoints;
using UpStatus.Api.Contracts.Requests.Auth;
using UpStatus.Api.Contracts.Responses.Auth;
using UpStatus.Application.Features.Auth.Login;

namespace UpStatus.Api.Endpoints.Auth;

public sealed class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
        Description(b => b.WithTags("Auth"));
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var command = new LoginCommand(req.Email, req.Password);
        var response = await command.ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
