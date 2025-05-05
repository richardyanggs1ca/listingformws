using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Xml;
using System.IO;
namespace ListingFormWS
{
    /// <summary>
    /// The main entry point for the ListingFormWS application.
    /// </summary>
    public class Program
    {

        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
               typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates and configures a host builder with the specified arguments.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <returns>An initialized IHostBuilder instance.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    Microsoft.Extensions.Hosting.IHostEnvironment env=builderContext.HostingEnvironment;
                    config.AddJsonFile("secrets/appsettings.json", optional: false, reloadOnChange: true);
                    config.AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
