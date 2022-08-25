namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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

        public async Task<ActionResult> Index()
        {
            IEnumerable<Shift> shifts = await shiftService.GetAll();

            return View(shifts);
        }

        public async Task<JsonResult> GetShiftByDate(int employeeId, DateTime date)
        {
            var getShiftByDateCriteria = new GetShiftByDateCriteria
            {
                EmployeeId = employeeId,
                Date = date
            };
            Shift shift = await shiftService.GetByDate(getShiftByDateCriteria);

            return Json(shift);
        }
    }
}
