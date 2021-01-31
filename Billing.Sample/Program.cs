namespace Zebble.Billing.Sample
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.FileProviders;
    using System.Reflection;

    public class Program
    {
        public static void Main(string[] args) => BuildWebHost(args).Run();

        static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder<Startup>(args)
                   .ConfigureAppConfiguration(builder =>
                   {
                       var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
                       builder.AddJsonFile(fileProvider, "Catalog.json", false, true);
                   })
                   .Build();
        }
    }
}
