using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UpStatus.Domain.Common;

namespace UpStatus.Api.Middleware;

public sealed class BusinessExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<BusinessExceptionMiddleware> _logger;

    public BusinessExceptionMiddleware(RequestDelegate next, ILogger<BusinessExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning("Business exception: {Code} - {Message}", ex.Code, ex.Message);

            var problem = new ProblemDetails
            {
                Type = "https://upstatus.local/errors/" + ex.Code,
                Title = "Erro de regra de negócio",
                Status = ex.StatusCode,
                Detail = ex.Message,
                Instance = context.Request.Path
            };
            problem.Extensions["code"] = ex.Code;

            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
        }
    }
}
