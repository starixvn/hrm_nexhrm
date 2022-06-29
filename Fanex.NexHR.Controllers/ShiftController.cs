namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using Criteria.Shift;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;

    public class ShiftController : BaseController
    {
        private readonly IShiftService shiftService;

        public ShiftController(ISessionService sessionService, IShiftService shiftService) : base(sessionService)
        {
            this.shiftService = shiftService;
        }

        public ActionResult Index()
        {
            IList<Shift> shifts = shiftService.GetAll();

            return View(shifts);
        }

        public JsonResult GetShiftByDate(int employeeId, DateTime date)
        {
            var getShiftByDateCriteria = new GetShiftByDateCriteria
            {
                EmployeeId = employeeId,
                Date = date
            };
            Shift shift = shiftService.GetByDate(getShiftByDateCriteria);

            return Json(shift);
        }
    }
}
