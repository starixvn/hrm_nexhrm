using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Mvc;


namespace Fanex.NexHR.Websites.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SiteMapNode rootNode = SiteMap.RootNode;
            var collection = rootNode.GetAllNodes();
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Intro()
        {
            
            return View();
        }
    }
}
