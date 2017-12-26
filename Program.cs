using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace mywebapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();

                    Log.Logger = new LoggerConfiguration()
                                    .MinimumLevel.Debug()
                                    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                                    .Enrich.FromLogContext()
                                    .Enrich.WithMachineName()
                                    .Enrich.WithThreadId()
                                    .WriteTo.ColoredConsole()
                                    .WriteTo.RollingFile(Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "Logs/{Date}.txt"))
                                    .CreateLogger();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddDebug();
                    logging.AddSerilog();
                    //logging.AddConsole();
                })
                .UseStartup<Startup>()
                .Build();

            webHost.Run();
        }
    }
}
