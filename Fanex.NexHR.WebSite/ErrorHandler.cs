namespace Fanex.NexHR.WebSite
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Http;

    public class ErrorHandler
    {
        private readonly RequestDelegate next;
        private readonly ILogServiceProvider logService;

        public ErrorHandler(RequestDelegate next, ILogServiceProvider logService)
        {
            this.next = next;
            this.logService = logService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logService.LogException(ex);
            }
        }
    }
}