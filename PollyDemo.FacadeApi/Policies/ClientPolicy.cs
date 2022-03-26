using System.Net;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace PollyDemo.FacadeApi.Policies;

public class ClientPolicy
{
    public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
    public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }
    public AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry { get; }
    
    public AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy { get; }

    public ClientPolicy()
    {
        ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                res => !res.IsSuccessStatusCode)
            .RetryAsync(10);
        

        LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                res => !res.IsSuccessStatusCode)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(3), (httpResponseMessage, retryCount, context) =>
            {
                if (context.Contains("User-Agent or Host here"))
                {
                    // Log....
                }
            });
        
        
        // retries exponentially increase the waiting time up to a certain threshold
        // not overwhelmed with requests hitting at the same time when it comes back up
        ExponentialHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                res => IsInKnownServerErrors(res.StatusCode))
            .WaitAndRetryAsync(3, retryAttempt =>
            {
                Console.WriteLine($"--> Retry Attempt {retryAttempt}");
                return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
            });


        CircuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(
                res => (int)res.StatusCode == 500)
            .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));
    }

    private bool IsInKnownServerErrors(HttpStatusCode returnedStatusCode)
    {
        return new List<HttpStatusCode>
        {
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        }.Contains(returnedStatusCode);
    }
}