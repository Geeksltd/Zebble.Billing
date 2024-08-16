namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class Startup : Olive.Mvc.Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration config, ILoggerFactory loggerFactory) : base(env, config, loggerFactory) { }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddControllers();

            services.AddZebbleBilling(builder =>
            {
                if (Environment.IsDevelopment())
                    builder.AddEntityFramework();
                else
                    builder.AddDynamoDb();

                builder.AddAppStoreV2();
                builder.AddGooglePlay();
                builder.AddCafeBazaar();
                builder.AddHuawei();

                builder.AddVoucher(builder =>
                {
                    if (Environment.IsDevelopment())
                        builder.AddEntityFramework();
                    else
                        builder.AddDynamoDb();
                });
            });
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(builder => builder.MapControllers());

            app.UseZebbleBilling(builder =>
            {
                if (Environment.IsDevelopment())
                    builder.UseEntityFramework();
                else
                    builder.UseDynamoDb();

                builder.UseAppStoreV2();
                builder.UseGooglePlay();
                builder.UseHuawei();

                builder.UseVoucher(builder =>
                {
                    if (Environment.IsDevelopment())
                        builder.UseEntityFramework();
                    else
                        builder.UseDynamoDb();
                });
            });
        }
    }
}
