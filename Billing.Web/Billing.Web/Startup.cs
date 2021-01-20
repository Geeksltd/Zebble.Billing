using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Olive;
using Olive.Email;

namespace Billing.Web
{
    public class Startup : Olive.Mvc.Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration config, ILoggerFactory loggerFactory) : base(env, config, loggerFactory)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddEmail();
            services.AddCors(c => c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()));
            services.AddMvc();
            services.AddAwsEventBus();

            services.AddAWSService<Amazon.S3.IAmazonS3>();
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);

            app.UseCors("AllowOrigin");
        }
    }
}
