using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ServerService.Configuration;
using ServerService.Interface;

namespace ServerService
{
    public class Program
    {
        private static ConfigurationManager ConfigurationManager { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                ConfigurationManager = new ConfigurationManager();

                Log.Information($"Starting up the {ConfigurationManager.ApplicationName} service.");

                CreateHostBuilder(args)
                    .Build()
                    .Run();

                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"There was a problem when starting the service.");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    hostingContext.Configuration = ConfigurationManager.GetConfiguration();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IServerService, Service.ServerService>();
                    services.AddHostedService<Worker>();
                })
                .UseSerilog();
    }
}
