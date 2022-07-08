namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Services.Interfaces;
    using NLog;
    using NLog.Web;

    public class LogServiceProvider : ILogServiceProvider
    {
        private readonly Logger nlog = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public void LogFile(string message)
        {
            nlog.Info(message);
        }
    }
}