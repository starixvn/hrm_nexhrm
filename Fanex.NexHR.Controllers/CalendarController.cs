namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class CalendarController : BaseController
    {
        private readonly ICalendarService calendarService;

        public CalendarController()
            : this(null)
        {
        }

        public CalendarController(
            ICalendarService calendarService)
        {
            this.calendarService = calendarService ?? new CalendarService();
        }

        [Authorize(Roles = "HR")]
        public JsonResult UpdateEmployeeShiftDate(int employeeId, DateTime date, int? shiftId)
        {
            var criteria = new UpdateEmployeeShiftDateCriteria
            {
                EmployeeId = employeeId,
                Date = date,
                ShiftId = shiftId,
                ModifiedBy = CurrentUser.Id,
            };
            CommandResult result = calendarService.UpdateEmployeeShiftDate(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult InsertEmployeeCalendar(AddEmployeeCalendarViewModel model)
        {
            CommandResult result = new CommandResult();
            if (model != null)
            {
                var criteria = new InsertEmployeeCalendarCriteria()
                {
                    EmployeeId = model.EmployeeId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate
                };

                result = calendarService.InsertWorkingCalendar(criteria);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkingSchedule(int employeeId, DateTime date)
        {
            var fromdate = new DateTime(date.Year, date.Month, 1);
            var criteria = new GetWorkingScheduleCriteria
            {
                FromDate = fromdate,
                ToDate = fromdate.AddMonths(1).AddDays(-1),
                EmployeeId = employeeId
            };

            IList<WorkingSchedule> schedules = calendarService.GetWorkingSchedule(criteria);
            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyWorkingSchedule()
        {
            return View();
        }

        public ActionResult WorkingCalendars(int month = 0, int year = 0)
        {
            var date = Convert.ToDateTime(string.Format("{0}/{1}/1", year != 0 ? year : DateTime.Now.Year, month != 0 ? month : DateTime.Now.Month));
            ViewBag.CurrentDate = date;
            var criteria = new GetWorkingScheduleCriteria
            {
                FromDate = date,
                ToDate = date.AddMonths(1).AddDays(-1)
            };

            IList<WorkingSchedule> schedules = calendarService.GetWorkingSchedule(criteria);
            return View(schedules);
        }

        [ChildActionOnly]
        public PartialViewResult AddEmployeeCalendar()
        {
            return PartialView(new AddEmployeeCalendarViewModel
            {
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
            });
        }
    }
}