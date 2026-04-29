using System.Security.Claims;
using UpStatus.Domain.Common;

namespace UpStatus.Api.Common;

public static class CurrentUserAccessor
{
    public static Guid GetUserId(ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(sub, out var userId))
        {
            throw new BusinessException("auth.invalid_token", "Token inválido.")
            {
                StatusCode = 401
            };
        }

        return userId;
    }

    public static string? GetUserName(ClaimsPrincipal principal) =>
        principal.FindFirstValue("name")
        ?? principal.FindFirstValue(ClaimTypes.Name)
        ?? principal.FindFirstValue(ClaimTypes.Email);
}
