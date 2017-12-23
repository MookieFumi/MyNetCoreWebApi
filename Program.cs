using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

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
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();

                    Log.Logger = new LoggerConfiguration()
                                    .Enrich.FromLogContext()
                                    .Enrich.WithMachineName()
                                    .Enrich.WithThreadId()
                                    .WriteTo.ColoredConsole()
                                    .WriteTo.RollingFile(Path.Combine(env.ContentRootPath,"Logs/{Date}.txt"))
                                    .CreateLogger();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));       
                    logging.AddSerilog();
                })
                .UseStartup<Startup>()
                .Build();

            webHost.Run();
        }
    }
}
