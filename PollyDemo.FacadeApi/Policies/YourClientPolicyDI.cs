namespace PollyDemo.FacadeApi.Policies;

/// <summary>
/// Static class containing an extension method for configuring HTTP client policies with Dependency Injection (DI).
/// This class is specifically designed to add Polly policies (retry and circuit breaker) to an HTTP client.
/// </summary>
public static class YourClientPolicyDI
{
    /// <summary>
    /// Extension method for <see cref="IServiceCollection"/> that configures an HTTP client named "YourClient"
    /// with retry and circuit breaker policies using Polly.
    /// </summary>
    /// <param name="services">The IServiceCollection to which the HTTP client and policies are added.</param>
    /// <returns>The modified IServiceCollection with the added HTTP client and policies.</returns>
    public static IServiceCollection AddYourClientPolicy(this IServiceCollection services)
    {
        // Add an HTTP client named "YourClient" and configure it with the retry and circuit breaker policies
        // defined in the PolicyRegistry class.
        services.AddHttpClient("YourClient")
            .AddPolicyHandler(PolicyRegistry.GetRetryPolicy())
            .AddPolicyHandler(PolicyRegistry.GetCircuitBreakerPolicy());
        
        return services;
    }
}