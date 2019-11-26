using System;
using System.IO;

namespace ServerService.Configuration.StorageConfiguration
{
    public static class StorageApplicationConfiguration
    {
        public static void ConfigureStorage(ConfigurationManager configurationManager)
        {

            var storageFolder = configurationManager.StorageFolder;
            var storageFile = configurationManager.StorageFile;
            var storageFileNames = configurationManager.StorageFileNames;

            Directory.CreateDirectory($"{ConfigurationManager.ApplicationPath}/{storageFolder}/{storageFile}");
            Directory.CreateDirectory($"{ConfigurationManager.ApplicationPath}/{storageFolder}/{storageFileNames}");

        }

        public static void ConfigureAndCreateStorage(this ConfigurationManager configuration)
        {
            ConfigureStorage(configuration);
        }
    }
}
