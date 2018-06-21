namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;

    [Authorize(Roles = "HR")]
    public class BioStationManagementController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}