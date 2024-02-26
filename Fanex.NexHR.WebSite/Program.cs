namespace Fanex.NexHR.WebSite
{
    using System;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using NLog;
    using NLog.Web;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
           return WebHost.CreateDefaultBuilder(args)
                    .UseNLog()
                    .UseUrls($"http://*:5000")
                    .UseStartup<Startup>();
        }
    }
}