namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.FileProviders;

    public class Program
    {
        public static void Main(string[] args) => BuildWebHost(args).Run();

        static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder<Startup>(args)
                   .ConfigureAppConfiguration(builder =>
                   {
                       var fileProvider = new EmbeddedFileProvider(typeof(IPlatformProvider).Assembly);
                       builder.AddJsonFile(fileProvider, "Zebble.Catalog.json", false, true);
                   })
                   .Build();
        }
    }
}
