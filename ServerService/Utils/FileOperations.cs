using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ServerService.Utils
{
    public static class FileOperations
    {
        private static string ApplicationPath { get; set; }
        public static string GetFileFullPath(string fileName)
        {
            if (File.Exists(fileName)) return Path.GetFullPath(fileName);
            var theFile = Directory.GetFiles(Directory.GetCurrentDirectory(), fileName, SearchOption.AllDirectories).FirstOrDefault();
            return string.IsNullOrEmpty(theFile) ? string.Empty : theFile;
        }

        public static string GetApplicationPath()
        {
            if (string.IsNullOrEmpty(ApplicationPath))
            {
                ApplicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            return ApplicationPath;
        }
    }
}
