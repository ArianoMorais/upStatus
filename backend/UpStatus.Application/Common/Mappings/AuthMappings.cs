using UpStatus.Api.Contracts.Responses.Auth;
using UpStatus.Domain.Users;

namespace UpStatus.Application.Common.Mappings;

public static class AuthMappings
{
    public static AuthUserResponse ToAuthUserResponse(this User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        Role = user.Role.ToString()
    };
}
