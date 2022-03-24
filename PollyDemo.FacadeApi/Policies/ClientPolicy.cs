using System.Net;
using Polly;
using Polly.Retry;

namespace PollyDemo.FacadeApi.Policies;

public class ClientPolicy
{
    public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
    public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }
    public AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry { get; }

    public ClientPolicy()
    {
        ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                res => !res.IsSuccessStatusCode)
            .RetryAsync(10);

        LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                res => !res.IsSuccessStatusCode)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(3));

        // retries exponentially increase the waiting time up to a certain threshold
        // not overwhelmed with requests hitting at the same time when it comes back up
        ExponentialHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                res =>  IsInKnownServerErrors(res.StatusCode))
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));            
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