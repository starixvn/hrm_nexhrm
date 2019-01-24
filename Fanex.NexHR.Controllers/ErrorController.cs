
namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;

    public class ErrorController: Controller
    {
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult AccountSuspended()
        {
            return View();
        }

        public ActionResult NoData()
        {
            return View();
        }
    }
}
