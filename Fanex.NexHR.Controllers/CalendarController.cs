namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Calendar;

    public class CalendarController : BaseController
    {
        private readonly ICalendarService calendarService;
        private readonly ITeamService teamService;
        private readonly ICommandService commandService;

        public CalendarController()
            : this(null, null, null)
        {
        }

        public CalendarController(
            ICalendarService calendarService,
            ITeamService teamService,
            ICommandService commandService)
        {
            this.calendarService = calendarService ?? new CalendarService();
            this.teamService = teamService ?? new TeamService();
            this.commandService = commandService ?? new CommandService();
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
            CommandResult result = commandService.Update(criteria);

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

                result = commandService.Insert(criteria);
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

        public ActionResult WorkingCalendars(WorkingCalendarViewModel model)
        {
            if (model != null)
            {
                model.Teams = teamService.GetAll();
                model.Month = model.Month == 0 ? DateTime.Now.Month : model.Month;
                model.Year = model.Year == 0 ? DateTime.Now.Year : model.Year;
                DateTime date = new DateTime(model.Year, model.Month, 1);
                var criteria = new GetWorkingScheduleCriteria
                {
                    FromDate = date,
                    ToDate = date.AddMonths(1).AddDays(-1),
                    EmployeeId = model.EmployeeId <= 0 ? (int?)null : model.EmployeeId,
                    TeamId = model.TeamId
                };

                IList<WorkingSchedule> schedules = calendarService.GetWorkingSchedule(criteria);
                model.WorkingSchedules = schedules;
            }
            
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
    }
}