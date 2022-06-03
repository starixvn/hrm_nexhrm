namespace Fanex.NexHR.Core
{
    using NLog;

    public static class LogFileHelper
    {
        private static readonly Logger logger = LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();

        public static void WriteInfo(string message)
        {
            logger.Info(message);
        }
    }
}