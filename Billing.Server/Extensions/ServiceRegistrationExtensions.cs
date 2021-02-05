namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddZebbleBilling(this IServiceCollection services, IConfiguration config)
        {
            services.AddZebbleProductsCache(config);

            services.Configure<DbContextOptions>(opts => config.GetSection("DbContext")?.Bind(opts));
            services.AddDbContext<BillingDbContext>();

            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.Configure<AppStoreOptions>(opts => config.GetSection("AppStore")?.Bind(opts));
            services.AddScoped<ILiveSubscriptionQuery, AppStoreLiveSubscriptionQuery>();
            services.AddScoped<IHookInterceptor, AppStoreHookInterceptor>();

            services.Configure<GooglePlayOptions>(opts => config.GetSection("GooglePlay")?.Bind(opts));
            services.Configure<GooglePubSubOptions>(opts => config.GetSection("GooglePubSub")?.Bind(opts));
            services.Configure<GooglePublisherOptions>(opts => config.GetSection("GooglePublisher")?.Bind(opts));
            services.AddScoped<ILiveSubscriptionQuery, GooglePlayLiveSubscriptionQuery>();
            services.AddScoped<IQueueProcessor, GooglePlayQueueProcessor>();

            services.AddScoped(typeof(IPlatformProvider<>), typeof(PlatformProvider<>));

            services.AddScoped<ISubscriptionManager, SubscriptionManager>();
            services.AddScoped<IRootQueueProcessor, RootQueueProcessor>();
            services.AddScoped<IRootHookInterceptor, RootHookInterceptor>();

            return services;
        }
    }
}
