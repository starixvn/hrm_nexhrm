// -----------------------------------------------------------------------
// <copyright file="GeographicMapController.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Geographic Map Controller.
    /// </summary>
    [Authorize]
    public class GeographicMapController : BaseController
    {
        public GeographicMapController()
        {
        }

        /// <summary>
        /// Geo-location Map.
        /// </summary>
        /// <returns>View Index.</returns>
        public ActionResult Index()
        {
            var user = this.CurrentUser;
            bool roleHR = user.IsInRoles(RolesEnum.HR);

            ViewBag.NickName = user.NickName;
            ViewBag.RoleHR = roleHR;

            return View();
        }
    }
}
