namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ScheduledMessageController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            return View();
        }
    }
}