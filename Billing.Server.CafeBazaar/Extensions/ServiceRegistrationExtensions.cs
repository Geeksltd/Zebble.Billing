namespace Zebble.Billing
{
    using CafeBazaar.DeveloperApi;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddZebbleBillingForCafeBazaar(this IServiceCollection services, IConfiguration config)
        {
            services.AddCafeBazaarDeveloperApi(config, "CafeBazaarDeveloperApi");

            services.Configure<CafeBazaarOptions>(opts => config.GetSection("CafeBazaar")?.Bind(opts));
            services.AddScoped<IQueueProcessor, CafeBazaarQueueProcessor>();
            services.AddScoped<ISubscriptionProcessor, CafeBazaarSubscriptionProcessor>();

            return services;
        }
    }
}
