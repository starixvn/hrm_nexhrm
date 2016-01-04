// --------------------------------------------------------------------------
// <copyright file="JobTitleController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;

    [Authorize]
    public class JobTitleController : BaseController
    {
        public ActionResult JobTitleManagement()
        {
            return View();
        }
    }
}