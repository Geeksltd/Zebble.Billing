namespace Zebble.Billing
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZebbleBilling(this IServiceCollection services, IConfiguration config, Action<IZebbleBillingBuilder> builder = null)
        {
            var zebbleBillingConfig = config.GetSection("ZebbleBilling");

            services.AddZebbleProductsCache(zebbleBillingConfig);

            services.Configure<DbContextOptions>(opts => zebbleBillingConfig.GetSection("DbContext")?.Bind(opts));
            services.AddDbContext<BillingDbContext>();

            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.AddScoped(typeof(IPlatformProvider<>), typeof(PlatformProvider<>));

            services.AddScoped<ISubscriptionManager, SubscriptionManager>();

            builder?.Invoke(new ZebbleBillingBuilder(services, zebbleBillingConfig));

            return services;
        }
    }
}
