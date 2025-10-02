namespace Fanex.NexHR.WebSite
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;

    public class ErrorHandler(RequestDelegate next, Logging.ILogger logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                var logMessage = new LogMessage(exception.Message)
                    .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                    .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                    .ToString();
                await logger.ErrorAsync(logMessage);
            }
        }
    }
}