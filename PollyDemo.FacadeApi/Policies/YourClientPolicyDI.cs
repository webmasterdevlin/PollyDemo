namespace PollyDemo.FacadeApi.Policies;

public static class YourClientPolicyDI
{
    public static IServiceCollection AddYourClientPolicy(this IServiceCollection services)
    {
        services.AddHttpClient("YourClient")
            .AddPolicyHandler(PolicyRegistry.GetRetryPolicy())
            .AddPolicyHandler(PolicyRegistry.GetCircuitBreakerPolicy());
        
        return services;
    }
}