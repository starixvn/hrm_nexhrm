namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
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

            return Json(result);
        }

        public JsonResult GetWorkingSchedule(DateTime date, int id)
        {
            var workingScheduleCriteria = new GetWorkingScheduleCriteria
            {
                ReportDate = date,
                EmployeeId = id
            };

            var schedules = calendarService.GetWorkingSchedule(workingScheduleCriteria);
            return Json(schedules);
        }

        public ActionResult WorkingCalendars(int month = 0, int year = 0)
        {
            var currentDate = new StringBuilder();

            ViewBag.Year = year != 0 ? year : DateTime.Now.Year;
            ViewBag.Month = month != 0 ? month : DateTime.Now.Month;

            currentDate.AppendFormat("{0}/{1}/1", ViewBag.Year, ViewBag.Month);
            ViewBag.CurrentDate = currentDate;

            var getUserChangedShiftCriteria = new GetUserChangedShiftCriteria
            {
                ReportDate = new DateTime(ViewBag.Year, ViewBag.Month, 1),
                EmployeeId = null,
                GroupId = null
            };
            var userChangedShift = calendarService.GetUserChangedShift(getUserChangedShiftCriteria);

            ViewBag.UserChangeShift = userChangedShift;
            var workingCalendarCriteria = new GetWorkingCalendarCriteria() { ReportDate = Convert.ToDateTime(currentDate.ToString()) };
            var model = calendarService.GetWorkingCalendars(workingCalendarCriteria);

            var getHalfDayOffCriteria = new GetHalfDayOffCriteria
            {
                ReportDate = DateTime.Parse(currentDate.ToString()),
                EmployeeId = null,
                GroupId = null
            };
            ViewBag.HalfDayOffs = calendarService.GetHalfDayOffs(getHalfDayOffCriteria);
            return View(model);
        }

        public ActionResult MyWorkingSchedule()
        {
            var workingScheduleCriteria = new GetWorkingScheduleCriteria
            {
                ReportDate = DateTime.Now,
                EmployeeId = CurrentUser.Id
            };
            var model = calendarService.GetWorkingSchedule(workingScheduleCriteria);

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult InsertWorkingCalendar(AddEmployeeCalendarViewModel model)
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

            return Json(result);
        }

        [ChildActionOnly]
        public PartialViewResult WorkingCalendarInsert()
        {
            return PartialView(new AddEmployeeCalendarViewModel
            {
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
            });
        }
    }
}