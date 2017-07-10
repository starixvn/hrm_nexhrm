namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Criteria.Shift;
    using Models;
    using Services;
    using Services.Interfaces;

    public class ShiftController : BaseController
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

        public JsonResult GetShiftByDate(int employeeId, DateTime date)
        {
            var getShiftByDateCriteria = new GetShiftByDateCriteria
            {
                EmployeeId = employeeId,
                Date = date
            };
            Shift shift = _shiftService.GetByDate(getShiftByDateCriteria);

            return Json(shift);
        }
    }
}
