namespace Fanex.NexHR.Services
{
    using System;
    using Fanex.NexHR.Services.Interfaces;
    using NLog;
    using NLog.Web;

    public class LogServiceProvider : ILogServiceProvider
    {
        private readonly Logger nlog = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
        private readonly Fanex.Logging.ILogger logger;

        public LogServiceProvider(Fanex.Logging.ILogger logger)
        {
            this.logger = logger;
        }

        public void LogException(Exception exception)
        {
            nlog.Error(exception);
            logger.Error(exception.ToString());
        }

        public void LogFile(string message)
        {
            nlog.Error(message);
        }
    }
}