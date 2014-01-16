using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fanex.NexHR.Websites.Controllers
{
    public class UIController : Controller
    {
        [ChildActionOnly]
        public PartialViewResult NavBar()
        {
            var routeData = ControllerContext.ParentActionViewContext.RouteData;
            ViewBag.Controller = routeData.Values["controller"];
            ViewBag.Action = routeData.Values["action"];

           
            return PartialView();
        }

     
    }
}
