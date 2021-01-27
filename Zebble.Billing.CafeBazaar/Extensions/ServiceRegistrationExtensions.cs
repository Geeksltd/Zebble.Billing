namespace Zebble.Billing
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddZebbleBillingForCafeBazaar(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CafeBazaarOptions>(opts => config.GetSection("CafeBazaar")?.Bind(opts));
            services.AddScoped<IQueueProcessor, CafeBazaarQueueProcessor>();
            services.AddScoped<ISubscriptionProcessor, CafeBazaarSubscriptionProcessor>();

            return services;
        }
    }
}
