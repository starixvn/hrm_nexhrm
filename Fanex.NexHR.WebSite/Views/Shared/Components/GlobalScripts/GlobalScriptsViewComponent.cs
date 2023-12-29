namespace Fanex.NexHR.WebSite.Views.Shared.Components.GlobalScripts
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class GlobalScriptsViewComponent : ViewComponent
    {
        private readonly ISessionService sessionService;

        public GlobalScriptsViewComponent(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.Employee = sessionService.CurrentEmployee;
            await Task.CompletedTask;
            return View();
        }
    }
}