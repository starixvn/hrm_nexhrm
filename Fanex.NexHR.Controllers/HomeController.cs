namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;

    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Employee = CurrentUser;

            return View(RedirectToAction("index", "dashboard"));
        }
    }
}