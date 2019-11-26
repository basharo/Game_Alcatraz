namespace ServerService.Configuration
{
    public static class ServerOptions
    {
        public static string IPAddress { get; set; }

        public static int Port { get; set; }
        public static bool IsPrimary { get; internal set; }
    }
}
