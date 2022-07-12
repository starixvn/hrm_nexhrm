namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        public HomeController(ISessionService sessionService) : base(sessionService)
        {
        }

        public ActionResult Index()
        {
            ViewBag.Employee = CurrentUser;

            return RedirectToAction("index", "dashboard");
        }
    }
}