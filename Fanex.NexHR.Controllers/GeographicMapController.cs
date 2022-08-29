namespace Fanex.NexHR.Controllers
{
    using Core.Enums;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class GeographicMapController : BaseController
    {
        public GeographicMapController(ISessionService sessionService) : base(sessionService)
        {
        }

        public IActionResult Index()
        {
            var user = CurrentUser;
            bool roleHR = user.IsInRoles(Role.HR);

            ViewBag.NickName = user.NickName;
            ViewBag.RoleHR = roleHR;

            return View();
        }
    }
}