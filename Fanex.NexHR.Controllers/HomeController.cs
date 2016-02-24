namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;

    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController()
        { 
        }

        public ActionResult Index()
        {
            ViewBag.Employee = this.CurrentUser;
            return View(RedirectToAction("myspace", "employee"));
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult AccountSuspended()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult NoData()
        {
            return View();
        }
    }
}
