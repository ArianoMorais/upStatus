using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Auth;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Application.Common.Mappings;
using UpStatus.Domain.Common;

namespace UpStatus.Application.Features.Auth.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : ICommandHandler<GetCurrentUserQuery, AuthUserResponse>
{
    private readonly IUserRepository _users;

    public GetCurrentUserQueryHandler(IUserRepository users)
    {
        _users = users;
    }

    public async Task<AuthUserResponse> ExecuteAsync(GetCurrentUserQuery query, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(query.UserId, ct);

        if (user is null)
        {
            throw new BusinessException("auth.user_not_found", "Usuário não encontrado.")
            {
                StatusCode = 404
            };
        }

        return user.ToAuthUserResponse();
    }
}
