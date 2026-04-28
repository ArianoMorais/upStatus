namespace UpStatus.Api.Contracts.Responses.Auth;

public sealed record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public AuthUserResponse User { get; init; } = new();
}
