using DotNetEnv;
using FastEndpoints;
using FastEndpoints.Swagger;
using UpStatus.Application;
using UpStatus.Infrastructure;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "UpStatus API";
            s.Version = "v1";
        };
    });

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    p.AllowAnyHeader().AllowAnyMethod();

    if (allowedOrigins.Length > 0)
    {
        p.WithOrigins(allowedOrigins).AllowCredentials();
    }
}));

var app = builder.Build();

app.UseCors();
app.UseFastEndpoints(c => c.Errors.UseProblemDetails());
app.UseSwaggerGen();

app.Run();
