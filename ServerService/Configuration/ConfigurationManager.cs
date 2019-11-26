using Microsoft.Extensions.Configuration;
using ServerService.Configuration.LogConfiguration;
using ServerService.Configuration.StorageConfiguration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ServerService.Configuration
{
    public class ConfigurationManager
    {
        private IConfiguration Configuration { get; set; }
        public string LogFile { get; set; }
        public string LogFolder { get; set; }
        public string ApplicationName { get; set; }
        public static string ApplicationPath { get; set; }
        
        public string SpreadDeamon { get; set; }
        public int SpreadPort { get; set; }
        public string SpreadUser { get; set; }
        public string SpreadGroup { get; set; }
        public List<int> ServerGroups { get; set; }
        public string StorageFolder { get; set; }
        public string StorageFile { get; set; }
        public string StorageFileNames { get; set; }

        public ConfigurationManager()
        {
            DiscoverApplicationPath();

            Configuration = new ConfigurationBuilder()
                .SetBasePath(ApplicationPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables().Build();

            Configure();
        }

        public IConfiguration GetConfiguration()
        {
            return Configuration;
        }

        private string GetConfigurationValue(string key)
        {
            return Configuration[key];
        }

        private void Configure()
        {
            LogFile = GetConfigurationValue("Log:File:LogFile");
            LogFolder = GetConfigurationValue("Log:File:LogFolder");
            StorageFolder = GetConfigurationValue("Storage:StorageFolder");
            StorageFile = GetConfigurationValue("Storage:StorageFile");
            StorageFileNames = GetConfigurationValue("Storage:StorageFileNames");
            if (string.IsNullOrEmpty(LogFolder)) LogFolder = "Logs";
            if (string.IsNullOrEmpty(LogFile)) LogFile = "log-{0}.txt";
            if (string.IsNullOrEmpty(StorageFolder)) LogFolder = "Storage";
            if (string.IsNullOrEmpty(StorageFile)) StorageFile = "data-{0}.txt";
            if (string.IsNullOrEmpty(StorageFileNames)) StorageFileNames = "clientNames.txt";
            ServerOptions.IPAddress = GetConfigurationValue("ServerConfig:Address");
            ServerOptions.Port = Convert.ToInt32(GetConfigurationValue("ServerConfig:Port"));
            ServerOptions.IsPrimary = Convert.ToBoolean(GetConfigurationValue("ServerConfig:IsPrimary"));
            SpreadDeamon = GetConfigurationValue("Spread:Deamon");
            SpreadPort = Convert.ToInt32(GetConfigurationValue("Spread:Port"));
            SpreadUser = GetConfigurationValue("Spread:User");
            SpreadGroup = GetConfigurationValue("Spread:GroupName");
            ServerGroups = new List<int>() 
            { 
                Convert.ToInt32(GetConfigurationValue("ServerGroup:ServerOnePort")), 
                Convert.ToInt32(GetConfigurationValue("ServerGroup:ServerTwoPort")) 
            };

            this.ConfigureLog();
            this.ConfigureAndCreateStorage();
        }
        public static void DiscoverApplicationPath()
        {
            ApplicationPath = Path.GetDirectoryName(Directory.GetFiles(Directory.GetCurrentDirectory(), "appsettings.json", SearchOption.AllDirectories).FirstOrDefault());
        }
    }
}