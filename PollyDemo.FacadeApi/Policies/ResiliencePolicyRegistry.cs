using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace PollyDemo.FacadeApi.Policies
{
    /// <summary>
    /// Provides methods to configure resilience policies for HTTP clients.
    /// </summary>
    public static class ResiliencePolicyRegistry
    {
        /// <summary>
        /// Creates and returns the HTTP retry strategy options.
        /// Configures a retry policy with exponential backoff and jitter to handle transient failures.
        /// </summary>
        /// <returns>A configured <see cref="HttpRetryStrategyOptions"/> instance.</returns>
        public static HttpRetryStrategyOptions GetHttpRetryStrategyOptions()
        {
            return new HttpRetryStrategyOptions
            {
                // The maximum number of retry attempts before failing.
                // Limiting retries prevents excessive load and potential cascading failures.
                MaxRetryAttempts = 3,

                // The initial delay before the first retry attempt.
                // Starting with a small delay improves responsiveness.
                Delay = TimeSpan.FromSeconds(2),

                // Specifies the backoff strategy to use for calculating the delay between retries.
                // Exponential backoff increases the delay exponentially after each attempt.
                BackoffType = DelayBackoffType.Exponential,

                // Indicates whether to add a random jitter to the delay between retries.
                // Jitter helps prevent retry storms by randomizing retry intervals.
                UseJitter = true,

                // Action to perform on each retry attempt.
                // Useful for logging retry attempts or implementing custom logic.
                OnRetry = args =>
                {
                    // Log the retry attempt number.
                    Console.WriteLine($"--> Retry Attempt {args.AttemptNumber}");
                    return ValueTask.CompletedTask;
                },

                // Defines the conditions under which retries should be attempted.
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    // Handle network-related exceptions like timeouts, connection failures, etc.
                    .Handle<HttpRequestException>()
                    // Handle specific HTTP status codes indicating transient failures.
                    .HandleResult(response =>
                        (int)response.StatusCode >= 500 ||          // Server errors (HTTP 5xx)
                        response.StatusCode == HttpStatusCode.RequestTimeout ||     // Request timeout (HTTP 408)
                        response.StatusCode == HttpStatusCode.TooManyRequests)      // Too many requests (HTTP 429)
            };
        }

        /// <summary>
        /// Creates and returns the HTTP circuit breaker strategy options.
        /// Configures a circuit breaker to temporarily stop sending requests to an unhealthy service.
        /// </summary>
        /// <returns>A configured <see cref="HttpCircuitBreakerStrategyOptions"/> instance.</returns>
        public static HttpCircuitBreakerStrategyOptions GetHttpCircuitBreakerStrategyOptions()
        {
            return new HttpCircuitBreakerStrategyOptions
            {
                // The time window over which failure rates are evaluated.
                // A longer sampling duration provides a more accurate picture of service health.
                SamplingDuration = TimeSpan.FromSeconds(30),

                // The failure rate threshold at which the circuit breaker will open.
                // A value of 0.5 means the circuit will open if 50% or more requests fail.
                FailureRatio = 0.5,

                // The minimum number of requests required before the failure ratio is evaluated.
                // Prevents the circuit breaker from opening due to insufficient data.
                MinimumThroughput = 10,

                // The duration the circuit breaker remains open before transitioning to half-open state.
                BreakDuration = TimeSpan.FromSeconds(60),

                // Action to perform when the circuit breaker opens.
                // Useful for logging or triggering alerts.
                OnOpened = args =>
                {
                    // Log that the circuit breaker has opened.
                    Console.WriteLine("--> Circuit Breaker Opened");
                    return ValueTask.CompletedTask;
                },

                // Action to perform when the circuit breaker closes.
                // Useful for logging recovery or resetting state.
                OnClosed = args =>
                {
                    // Log that the circuit breaker has reset.
                    Console.WriteLine("--> Circuit Breaker Reset");
                    return ValueTask.CompletedTask;
                },

                // Defines the conditions that are considered failures for the circuit breaker.
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    // Handle network-related exceptions.
                    .Handle<HttpRequestException>()
                    // Handle specific HTTP status codes indicating failures.
                    .HandleResult(response =>
                        (int)response.StatusCode >= 500 ||          // Server errors (HTTP 5xx)
                        response.StatusCode == HttpStatusCode.RequestTimeout ||     // Request timeout (HTTP 408)
                        response.StatusCode == HttpStatusCode.TooManyRequests)      // Too many requests (HTTP 429)
            };
        }

        /// <summary>
        /// Creates and returns the HTTP timeout strategy options.
        /// Configures a timeout policy to cancel requests that exceed a specified duration.
        /// </summary>
        /// <returns>A configured <see cref="HttpTimeoutStrategyOptions"/> instance.</returns>
        public static HttpTimeoutStrategyOptions GetHttpTimeoutStrategyOptions()
        {
            return new HttpTimeoutStrategyOptions
            {
                // The maximum duration to allow for an HTTP request before timing out.
                // A reasonable timeout prevents hanging requests and improves application responsiveness.
                Timeout = TimeSpan.FromSeconds(15),

                // Action to perform when a timeout occurs.
                // Useful for logging timeouts or performing cleanup.
                OnTimeout = args =>
                {
                    // Log that the request has timed out.
                    Console.WriteLine($"--> Request timed out after {args.Timeout.TotalSeconds} seconds.");
                    return ValueTask.CompletedTask;
                }
            };
        }
    }
}
