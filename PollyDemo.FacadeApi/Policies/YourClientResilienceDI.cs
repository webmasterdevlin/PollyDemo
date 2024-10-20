using Polly;

namespace PollyDemo.FacadeApi.Policies;

public static class YourClientResilienceDI
{
   public static IServiceCollection AddYourClientResilience(this IServiceCollection services)
    {
        services.AddHttpClient("YourClient")
            .AddStandardResilienceHandler(configure =>
            {
                configure.Retry = ResiliencePolicyRegistry.GetHttpRetryStrategyOptions();
                configure.CircuitBreaker = ResiliencePolicyRegistry.GetHttpCircuitBreakerStrategyOptions();
                configure.AttemptTimeout = ResiliencePolicyRegistry.GetHttpTimeoutStrategyOptions();
            });
        
        return services;
    }
}