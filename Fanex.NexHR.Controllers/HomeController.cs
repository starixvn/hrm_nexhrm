namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;

    public class HomeController : BaseController
    {
        [Authorize(Roles = "SpecialUser")]
        public ActionResult Index()
        {
            ViewBag.Employee = CurrentUser;
            return View(RedirectToAction("index", "dashboard"));
        }
    }
}
