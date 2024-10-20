using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace PollyDemo.FacadeApi.Policies
{
    public static class ResiliencePolicyRegistry
    {
        public static HttpRetryStrategyOptions GetHttpRetryStrategyOptions()
        {
            return new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 3, // Best practice is to limit retries to 3 attempts to avoid excessive load

                Delay = TimeSpan.FromSeconds(2), // Start with a small initial delay

                BackoffType = DelayBackoffType.Exponential, // Exponential backoff to increase delays between retries

                UseJitter = true, // Use jitter to prevent retry storms in high-load scenarios
                
                OnRetry = (ex) =>
                {
                    // Log the retry attempt number and calculate the delay using exponential backoff.
                    Console.WriteLine($"--> Retry Attempt {ex.AttemptNumber}");
                    return ValueTask.CompletedTask;
                },

                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>() // Handle network-related exceptions
                    .HandleResult(response =>
                        (int)response.StatusCode >= 500 || // Handle server errors (5xx)
                        response.StatusCode == HttpStatusCode.RequestTimeout || // Handle request timeouts
                        response.StatusCode == HttpStatusCode.TooManyRequests) // Handle rate limiting (429)
            };
        }

        public static HttpCircuitBreakerStrategyOptions GetHttpCircuitBreakerStrategyOptions()
        {
            return new HttpCircuitBreakerStrategyOptions
            {
                SamplingDuration = TimeSpan.FromSeconds(30), // Evaluate over a 30-second window

                FailureRatio = 0.5, // Open circuit if 50% or more requests fail

                MinimumThroughput = 10, // Require at least 10 requests before evaluation

                BreakDuration = TimeSpan.FromSeconds(60), // Keep circuit open for 60 seconds once tripped

                OnOpened = ex =>
                {
                    // Log when the circuit breaker is triggered and opened.
                    Console.WriteLine("--> Circuit Breaker Opened");
                    return ValueTask.CompletedTask;
                },
                
                OnClosed = ex =>
                {
                    // Log when the circuit breaker is reset and closed.
                    Console.WriteLine("--> Circuit Breaker Reset");
                    return ValueTask.CompletedTask;
                },
                
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>() // Handle network-related exceptions
                    .HandleResult(response =>
                        (int)response.StatusCode >= 500 || // Handle server errors (5xx)
                        response.StatusCode == HttpStatusCode.RequestTimeout || // Handle request timeouts
                        response.StatusCode == HttpStatusCode.TooManyRequests) // Handle rate limiting (429)
            };
        }

        public static HttpTimeoutStrategyOptions GetHttpTimeoutStrategyOptions()
        {
            return new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(15), // Set a reasonable timeout duration

                // Optional: Add an on-timeout event to log or handle timeouts
                OnTimeout = args =>
                {
                    Console.WriteLine($"--> Request timed out after {args.Timeout.TotalSeconds} seconds.");
                    return ValueTask.CompletedTask; // Return a completed ValueTask
                }
            };
        }
    }
}
