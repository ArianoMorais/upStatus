using System.Diagnostics;
using UpStatus.Application.Common.Abstractions;
using UpStatus.Domain.Checks;
using UpStatus.Domain.Monitors;

namespace UpStatus.Infrastructure.Http;

public sealed class HttpProbeClient : IHttpProbeClient
{
    public const string HttpClientName = "upstatus-probe";

    private readonly IHttpClientFactory _factory;

    public HttpProbeClient(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<ProbeOutcome> ProbeAsync(string url, MonitorConfig config, CancellationToken ct)
    {
        var client = _factory.CreateClient(HttpClientName);
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(config.TimeoutMs));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        var request = new HttpRequestMessage(new HttpMethod(config.HttpMethod), url);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                linked.Token);

            stopwatch.Stop();
            var latencyMs = (int)stopwatch.ElapsedMilliseconds;
            var statusCode = (int)response.StatusCode;

            var statusOk = config.ExpectedStatusCode.HasValue
                ? statusCode == config.ExpectedStatusCode.Value
                : statusCode is >= 200 and < 400;

            if (!statusOk)
            {
                return new ProbeOutcome
                {
                    Result = CheckResult.Failure,
                    StatusCode = statusCode,
                    LatencyMs = latencyMs,
                    ErrorMessage = $"Status code inesperado: {statusCode}."
                };
            }

            if (latencyMs >= config.DegradedLatencyMs)
            {
                return new ProbeOutcome
                {
                    Result = CheckResult.Degraded,
                    StatusCode = statusCode,
                    LatencyMs = latencyMs
                };
            }

            return new ProbeOutcome
            {
                Result = CheckResult.Success,
                StatusCode = statusCode,
                LatencyMs = latencyMs
            };
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            stopwatch.Stop();
            return new ProbeOutcome
            {
                Result = CheckResult.Timeout,
                LatencyMs = (int)stopwatch.ElapsedMilliseconds,
                ErrorMessage = $"Tempo limite de {config.TimeoutMs} ms excedido."
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new ProbeOutcome
            {
                Result = CheckResult.Failure,
                LatencyMs = (int)stopwatch.ElapsedMilliseconds,
                ErrorMessage = ex.Message
            };
        }
    }
}
