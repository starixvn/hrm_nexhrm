namespace Fanex.NexHR.Core
{
    using System.Web.Hosting;
    using Serilog;
    using Serilog.Events;

    public static class SerilogHelper
    {
        private static readonly ILogger logger = new LoggerConfiguration()
               .MinimumLevel.Information()
               .WriteTo.Async(a =>
               {
                   a.File(HostingEnvironment.MapPath("~/logs/log.txt"), rollingInterval: RollingInterval.Day);
               })
               .CreateLogger();

        public static void WriteInfo(string message)
        {
            logger.Write(LogEventLevel.Information, message);
        }
    }
}