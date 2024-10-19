using Polly.CircuitBreaker;
using Polly.Retry;

namespace PollyDemo.FacadeApi.Policies;

public static class ResiliencePolicyRegistry
{
    public static RetryStrategyOptions GetRetryStrategyOptions()
    {
        return new RetryStrategyOptions
        {
            // config here
        };
    }

    public static CircuitBreakerStrategyOptions GetCircuitBreakerStrategyOptions()
    {
        return new CircuitBreakerStrategyOptions
        {
            // config here
        };
    }
}