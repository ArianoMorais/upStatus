using UpStatus.Application.Common.Abstractions;
using UpStatus.Application.Common.Abstractions.Repositories;
using UpStatus.Domain.Users;

namespace UpStatus.Api.Seed;

public sealed class AdminUserSeeder
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IDateTimeProvider _clock;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminUserSeeder> _logger;

    public AdminUserSeeder(
        IUserRepository users,
        IPasswordHasher hasher,
        IDateTimeProvider clock,
        IConfiguration configuration,
        ILogger<AdminUserSeeder> logger)
    {
        _users = users;
        _hasher = hasher;
        _clock = clock;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken ct)
    {
        if (await _users.AnyAsync(ct))
        {
            return;
        }

        var email = _configuration["Seed:AdminEmail"];
        var password = _configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Seed do admin não executado: Seed:AdminEmail ou Seed:AdminPassword não configurados.");
            return;
        }

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = _hasher.Hash(password),
            Name = "Administrador",
            Role = UserRole.Admin,
            CreatedAt = _clock.UtcNow
        };

        await _users.AddAsync(admin, ct);

        _logger.LogInformation("Usuário admin criado com e-mail {Email}.", admin.Email);
    }
}
