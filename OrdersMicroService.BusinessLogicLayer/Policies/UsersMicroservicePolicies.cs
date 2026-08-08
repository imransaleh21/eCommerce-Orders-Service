using Microsoft.Extensions.Logging;
using Polly;

namespace OrdersMicroService.BusinessLogicLayer.Policies;

public class UsersMicroservicePolicies : IUsersMicroservicePolicies
{
    private readonly ILogger<UsersMicroservicePolicies> _logger;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
    private readonly IAsyncPolicy<HttpResponseMessage> _circuitBreakerPolicy;

    public UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger)
    {
        _logger = logger;

        _retryPolicy = Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: 5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogInformation("Retry {RetryCount} after {Timespan} due to: {ReasonPhrase}", retryCount, timespan.TotalSeconds, outcome.Result?.ReasonPhrase);
                }
            );

        _circuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromMinutes(1),
                onBreak: (outcome, timespan) =>
                {
                    _logger.LogInformation("Circuit breaker opened for {Duration} seconds due to: {ReasonPhrase}", timespan.TotalSeconds, outcome.Result?.ReasonPhrase);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker reset.");
                }
            );
    }

    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() => _retryPolicy;

    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() => _circuitBreakerPolicy;
}
