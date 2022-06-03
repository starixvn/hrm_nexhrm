namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Newtonsoft.Json;

    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            SerilogHelper.WriteInfo(JsonConvert.SerializeObject(new { RequestId = 10000, Command = "Oliver", Nickname = "Oliver" }));

            ViewBag.Employee = CurrentUser;

            return View(RedirectToAction("index", "dashboard"));
        }
    }
}