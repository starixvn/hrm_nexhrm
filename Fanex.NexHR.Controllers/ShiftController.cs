namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    [Authorize]
    public class ShiftController : Controller
    {
        private readonly IShiftService _shiftService;

        public ShiftController()
            : this(null)
        {
        }

        public ShiftController(IShiftService shiftService = null)
        {
            _shiftService = shiftService ?? new ShiftService();
        }

        public ActionResult Index()
        {
            IList<Shift> shifts = _shiftService.GetAll();

            return View(shifts);
        }
    }
}
