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

        public JsonResult GetCalendarByEmployee(int employeeId, DateTime date)
        {
            var criteria = new GetWorkingScheduleCriteria
            {
                ReportDate = date,
                EmployeeId = employeeId
            };

            IList<WorkingScheduleModel> schedules = calendarService.GetWorkingSchedule(criteria);
            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyWorkingSchedule()
        {
            return View();
        }

        public ActionResult WorkingCalendars(int month = 0, int year = 0)
        {
            ViewBag.Year = year != 0 ? year : DateTime.Now.Year;
            ViewBag.Month = month != 0 ? month : DateTime.Now.Month;

            var currentDate = string.Format("{0}/{1}/1", ViewBag.Year, ViewBag.Month);
            ViewBag.CurrentDate = currentDate;

            var workingCalendarCriteria = new GetWorkingCalendarCriteria() { ReportDate = Convert.ToDateTime(currentDate.ToString()) };
            IList<WorkingCalendarsViewModel> model = calendarService.GetWorkingCalendars(workingCalendarCriteria);

            ViewBag.LeaveHalfDays = GetLeaveHalfDay(DateTime.Parse(currentDate));
            return View(model);
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

        private IList<LeaveHalfDay> GetLeaveHalfDay(DateTime month)
        {
            var getLeaveHalfDayCriteria = new GetLeaveHalfDayCriteria
            {
                ReportDate = month
            };

            return calendarService.GetLeaveHalfDay(getLeaveHalfDayCriteria);
        }
    }
}