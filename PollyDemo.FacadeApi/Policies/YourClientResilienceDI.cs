using Polly;

namespace PollyDemo.FacadeApi.Policies;

public static class YourClientResilienceDI
{
   public static IServiceCollection AddYourClientResilience(this IServiceCollection services)
    {
        services.AddHttpClient("YourClient")
            .AddStandardResilienceHandler();
        
        services.AddResiliencePipeline("default", builder =>
        {
            builder.AddRetry(ResiliencePolicyRegistry.GetRetryStrategyOptions());
            builder.AddCircuitBreaker(ResiliencePolicyRegistry.GetCircuitBreakerStrategyOptions());
        });
        
        return services;
    }
}