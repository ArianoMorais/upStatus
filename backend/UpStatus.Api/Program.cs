using DotNetEnv;
using FastEndpoints;
using FastEndpoints.Swagger;
using UpStatus.Api.Configuration;
using UpStatus.Api.Hubs;
using UpStatus.Api.Middleware;
using UpStatus.Api.Seed;
using UpStatus.Application;
using UpStatus.Infrastructure;
using UpStatus.Infrastructure.Persistence;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddJwtAuth(builder.Configuration)
    .AddRealtime(builder.Configuration)
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "UpStatus API";
            s.Version = "v1";
        };
    });

builder.Services.AddScoped<AdminUserSeeder>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    p.AllowAnyHeader().AllowAnyMethod();

    if (allowedOrigins.Length > 0)
    {
        p.WithOrigins(allowedOrigins).AllowCredentials();
    }
}));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var indexes = scope.ServiceProvider.GetRequiredService<MongoIndexInitializer>();
    await indexes.EnsureIndexesAsync(CancellationToken.None);

    var seeder = scope.ServiceProvider.GetRequiredService<AdminUserSeeder>();
    await seeder.RunAsync(CancellationToken.None);
}

app.UseMiddleware<BusinessExceptionMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c => c.Errors.UseProblemDetails());
app.UseSwaggerGen();
app.MapHub<MonitoringHub>(MonitoringHub.Path);

app.Run();
