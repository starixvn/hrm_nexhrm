// --------------------------------------------------------------------------
// <copyright file="GeographicMapController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;

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
            bool roleHR = user.IsInRoles(Role.HR);

            ViewBag.NickName = user.NickName;
            ViewBag.RoleHR = roleHR;

            return View();
        }
    }
}
