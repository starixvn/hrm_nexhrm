namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    public class ShiftController : BaseController
    {
        private readonly IShiftService shiftService;

        public ShiftController()
            : this(null)
        {
        }

        public ShiftController(IShiftService shiftService = null)
        {
            this.shiftService = shiftService ?? new ShiftService();
        }

        public ActionResult Index()
        {
            IList<Shift> shifts = shiftService.GetAll();

            return View(shifts);
        }
    }
}
