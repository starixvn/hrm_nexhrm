
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NexHR.Controllers
{

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
