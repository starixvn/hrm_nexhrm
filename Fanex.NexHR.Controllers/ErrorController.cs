using Microsoft.AspNetCore.Mvc;

namespace Fanex.NexHR.Controllers
{
    public class ErrorController : Microsoft.AspNetCore.Mvc.Controller
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