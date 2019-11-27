using System;
using System.IO;
using System.Linq;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ServerService.Configuration;
using ServerService.Interface;
using ServerService.Service;
using ServerService.Utils;

namespace ServerService
{
    public class Program
    {
        private static ConfigurationManager ConfigurationManager { get; set; }
       

        public static void Main(string[] args)
        {

            string actorName = "server";
            Globals.groupSize = 2;
            Console.Title = actorName;



            try
            {

                startActorSystem("alcatraz");
                var localChatActor = Globals.mainActorSystem.ActorOf(Props.Create<RegisterActor>(), "RegisterActor");


                string line = string.Empty;
                while (line != null)
                {
                    line = Console.ReadLine();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        

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

        public static void startActorSystem(string actorSystemName)
        {
            var config = File.ReadAllText($"{Path.GetDirectoryName(Directory.GetFiles(Directory.GetCurrentDirectory(), "AkkaConfig.txt", SearchOption.AllDirectories).FirstOrDefault())}/AkkaConfig.txt");
            var conf = ConfigurationFactory.ParseString(config);
            Globals.mainActorSystem = ActorSystem.Create(actorSystemName, conf);

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
                    services.AddTransient<ISpreadService, SpreadService>();
                    services.AddTransient<IServerService, Service.ServerService>();
                    services.AddHostedService<Worker>();
                })
                .UseSerilog();
    }
}
