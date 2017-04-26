namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Core.Enums;

    public class GeographicMapController : BaseController
    {
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
