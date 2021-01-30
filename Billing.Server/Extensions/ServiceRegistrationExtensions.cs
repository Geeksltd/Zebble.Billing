namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddZebbleBilling(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CatalogOptions>(opts => config.GetSection("Catalog")?.Bind(opts));
            services.AddScoped<IProductRepository, ProductRepository>();

            services.Configure<DbContextOptions>(opts => config.GetSection("DbContext")?.Bind(opts));
            services.AddDbContext<BillingDbContext>();

            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.Configure<AppStoreOptions>(opts => config.GetSection("AppStore")?.Bind(opts));
            services.AddScoped<ILiveSubscriptionQuery, AppStoreLiveSubscriptionQuery>();

            services.Configure<GooglePlayOptions>(opts => config.GetSection("GooglePlay")?.Bind(opts));
            services.Configure<GooglePubSubOptions>(opts => config.GetSection("GooglePubSub")?.Bind(opts));
            services.Configure<GooglePublisherOptions>(opts => config.GetSection("GooglePublisher")?.Bind(opts));
            services.AddScoped<IQueueProcessor, GooglePlayQueueProcessor>();
            services.AddScoped<ILiveSubscriptionQuery, GooglePlayLiveSubscriptionQuery>();

            services.AddScoped(typeof(IPlatformProvider<>), typeof(PlatformProvider<>));

            services.AddScoped<ISubscriptionManager, SubscriptionManager>();
            services.AddScoped<IRootQueueProcessor, RootQueueProcessor>();

            return services;
        }
    }
}
