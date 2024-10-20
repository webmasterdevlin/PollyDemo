using Polly;

namespace PollyDemo.FacadeApi.Policies;

public static class YourClientResilienceDI
{
   public static IServiceCollection AddYourClientResilience(this IServiceCollection services)
    {
        services.AddHttpClient("YourClient")
            .AddResilienceHandler("default", builder =>
        {
            builder.AddRetry(ResiliencePolicyRegistry.GetHttpRetryStrategyOptions());
            builder.AddCircuitBreaker(ResiliencePolicyRegistry.GetHttpCircuitBreakerStrategyOptions());
            builder.AddTimeout(ResiliencePolicyRegistry.GetHttpTimeoutStrategyOptions());
        });
        
        return services;
    }
}