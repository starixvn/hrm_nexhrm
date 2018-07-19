using System.Web.Mvc;
using Fanex.NexHR.HangFire.BioTask;

namespace Fanex.NexHR.HangFire.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var init = new BioTimeChangeMonitor();
            init.Initialize();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}