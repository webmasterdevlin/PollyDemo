using Polly;
using Polly.Extensions.Http;

namespace PollyDemo.FacadeApi.Policies;

public static class PolicyRegistry
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
            {
                Console.WriteLine($"--> Retry Attempt {retryAttempt}");
                return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
            });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(6, TimeSpan.FromSeconds(30), (result, span) =>
            {
                Console.WriteLine("--> Circuit Breaker Opened");
            }, () =>
            {
                Console.WriteLine("--> Circuit Breaker Reset");
            });
    }
}