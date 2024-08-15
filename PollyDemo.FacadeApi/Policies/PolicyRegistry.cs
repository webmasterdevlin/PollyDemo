using Polly;
using Polly.Extensions.Http;

namespace PollyDemo.FacadeApi.Policies;

/// <summary>
/// Static class containing methods to define and retrieve various Polly policies for handling HTTP requests.
/// </summary>
public static class PolicyRegistry
{
    /// <summary>
    /// Defines a retry policy using Polly that handles transient HTTP errors by retrying the operation up to 3 times.
    /// Implements an exponential backoff strategy where the wait time increases with each retry attempt.
    /// </summary>
    /// <returns>An asynchronous policy that retries the HTTP request upon encountering transient errors.</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
            {
                // Log the retry attempt number and calculate the delay using exponential backoff.
                Console.WriteLine($"--> Retry Attempt {retryAttempt}");
                return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
            });
    }

    /// <summary>
    /// Defines a circuit breaker policy using Polly that breaks the circuit after 6 consecutive failures, 
    /// preventing further attempts for 30 seconds. The circuit will be reset once the duration elapses, 
    /// allowing requests to go through again.
    /// </summary>
    /// <returns>An asynchronous policy that breaks the circuit upon encountering multiple consecutive transient errors.</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(6, TimeSpan.FromSeconds(30), (result, span) =>
            {
                // Log when the circuit breaker is triggered and opened.
                Console.WriteLine("--> Circuit Breaker Opened");
            }, () =>
            {
                // Log when the circuit breaker is reset and closed.
                Console.WriteLine("--> Circuit Breaker Reset");
            });
    }
}