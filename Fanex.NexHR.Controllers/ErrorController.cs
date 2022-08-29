using Microsoft.AspNetCore.Mvc;

namespace Fanex.NexHR.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult AccountSuspended()
        {
            return View();
        }

        public IActionResult NoData()
        {
            return View();
        }
    }
}