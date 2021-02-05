namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static partial class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddZebbleBilling(this IServiceCollection services, IConfiguration config, Action<ZebbleBillingOptionsBuilder> builder = null)
        {
            services.AddZebbleProductsCache(config);

            services.Configure<DbContextOptions>(opts => config.GetSection("ZebbleBilling:DbContext")?.Bind(opts));
            services.AddDbContext<BillingDbContext>();

            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.AddScoped(typeof(IPlatformProvider<>), typeof(PlatformProvider<>));

            services.AddScoped<ISubscriptionManager, SubscriptionManager>();
            services.AddScoped<IRootQueueProcessor, RootQueueProcessor>();
            services.AddScoped<IRootHookInterceptor, RootHookInterceptor>();

            builder?.Invoke(new ZebbleBillingOptionsBuilder(services, config));

            return services;
        }
    }
}
