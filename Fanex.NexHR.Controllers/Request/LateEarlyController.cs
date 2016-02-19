namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    [Authorize]
    public class LateEarlyController : BaseController
    {
        public LateEarlyController() 
        {
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
