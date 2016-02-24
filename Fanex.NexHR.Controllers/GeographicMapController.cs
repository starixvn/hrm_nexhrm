namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;

    public class GeographicMapController : BaseController
    {
        public GeographicMapController()
        {
        }

        public ActionResult Index()
        {
            var user = CurrentUser;
            bool roleHR = user.IsInRoles(Role.HR);

            ViewBag.NickName = user.NickName;
            ViewBag.RoleHR = roleHR;

            return View();
        }
    }
}
