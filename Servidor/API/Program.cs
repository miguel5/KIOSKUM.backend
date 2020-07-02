using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.ClearProviders();
                        logging.AddConsole();
                        logging.AddTraceSource("Information, ActivityTracing");
                    })
                .UseStartup<Startup>();
    }
}
