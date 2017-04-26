namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;

    [Authorize]
    public class HomeController : BaseController
    {
        [Authorize(Roles = "SpecialUser")]
        public ActionResult Index()
        {
            ViewBag.Employee = CurrentUser;
            return View(RedirectToAction("index", "dashboard"));
        }

        [Authorize(Roles = "SpecialUser")]
        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult AccountSuspended()
        {
            return View();
        }

        [Authorize(Roles = "SpecialUser")]
        [AllowAnonymous]
        public ActionResult NoData()
        {
            return View();
        }
    }
}
