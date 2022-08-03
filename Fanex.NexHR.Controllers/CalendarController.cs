namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using Criteria.Calendar;
    using Fanex.NexHR.Models.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Calendar;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Calendar;

    public class CalendarController : BaseController
    {
        private readonly ICalendarService calendarService;
        private readonly ITeamService teamService;
        private readonly ICommandService commandService;

        public CalendarController(ISessionService sessionService,
            ICalendarService calendarService,
            ITeamService teamService,
            ICommandService commandService) : base(sessionService)
        {
            this.calendarService = calendarService;
            this.teamService = teamService;
            this.commandService = commandService;
        }

        [HrmAuthorize(Roles = "HR")]
        public JsonResult UpdateEmployeeShiftDate(int employeeId, DateTime date, int? shiftId)
        {
            var criteria = new UpdateEmployeeShiftDateCriteria
            {
                EmployeeId = employeeId,
                Date = date,
                ShiftId = shiftId,
                ModifiedBy = CurrentUser.Id,
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult InsertEmployeeCalendar(AddEmployeeCalendarViewModel model)
        {
            CommandResult result = new CommandResult();
            if (model != null)
            {
                var criteria = new InsertEmployeeCalendarCriteria
                {
                    EmployeeId = model.AddEmployeeId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    SetOffDay = model.SetOffDay
                };

                result = commandService.ExecuteNonQuery(criteria);
            }

            return Json(result);
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
            return Json(schedules);
        }

        public ActionResult MyWorkingSchedule()
        {
            return View();
        }

        public ActionResult WorkingCalendars(WorkingCalendarViewModel model)
        {
            if (model != null)
            {
                if (model.Year == 0)
                {
                    model.EmployeeId = -1;
                }

                model.Teams = teamService.GetTeam();
                model.Month = model.Month == 0 ? DateTime.Now.Month : model.Month;
                model.Year = model.Year == 0 ? DateTime.Now.Year : model.Year;
                DateTime date = new DateTime(model.Year, model.Month, 1);
                var criteria = new GetWorkingScheduleCriteria
                {
                    FromDate = date,
                    ToDate = date.AddMonths(1).AddDays(-1),
                    EmployeeId = model.EmployeeId < 0 ? (int?)null : model.EmployeeId,
                    TeamId = model.TeamId
                };

                IList<WorkingSchedule> schedules = calendarService.GetWorkingSchedule(criteria);
                model.WorkingSchedules = schedules;
            }

            return View(model);
        }

        public ActionResult LeaveSchedule(WorkingCalendarViewModel model)
        {
            if (model != null)
            {
                if (model.Year == 0)
                {
                    model.EmployeeId = -1;
                }

                model.Teams = teamService.GetTeam();
                model.Month = model.Month == 0 ? DateTime.Now.Month : model.Month;
                model.Year = model.Year == 0 ? DateTime.Now.Year : model.Year;
            }

            return View(model);
        }

        public ActionResult GetEmployeeLeaveSchedule(WorkingCalendarViewModel model)
        {
            DateTime date = new DateTime(model.Year, model.Month, 1);
            var criteria = new GetLeaveScheduleCriteria
            {
                FromDate = date,
                ToDate = date.AddMonths(1).AddDays(-1),
                EmployeeId = model.EmployeeId < 0 ? (int?)null : model.EmployeeId,
                TeamId = model.TeamId
            };

            var schedules = calendarService.GetLeaveSchedule(criteria);

            return PartialView("_LeaveDaySummarySchedule", schedules);
        }

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