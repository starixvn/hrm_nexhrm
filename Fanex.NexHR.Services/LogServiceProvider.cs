namespace Fanex.NexHR.Services
{
    using System;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interfaces;
    using NLog;
    using NLog.Web;

    public class LogServiceProvider : ILogServiceProvider
    {
        private readonly Logger nlog;
        private readonly Logging.ILogger logger;

        public LogServiceProvider(Logging.ILogger logger, AppSetting settings)
        {
            this.logger = logger;
            nlog = NLogBuilder.ConfigureNLog(settings.NLogConfigFilePath).GetCurrentClassLogger();
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