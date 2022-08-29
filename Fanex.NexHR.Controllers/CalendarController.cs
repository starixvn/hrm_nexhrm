namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
        private readonly IBaseService baseService;

        public CalendarController(ISessionService sessionService,
            ICalendarService calendarService,
            ITeamService teamService,
            IBaseService baseService) : base(sessionService)
        {
            this.calendarService = calendarService;
            this.teamService = teamService;
            this.baseService = baseService;
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<IActionResult> UpdateEmployeeShiftDate(int employeeId, DateTime date, int? shiftId)
        {
            var criteria = new UpdateEmployeeShiftDateCriteria
            {
                EmployeeId = employeeId,
                Date = date,
                ShiftId = shiftId,
                ModifiedBy = CurrentUser.Id,
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> InsertEmployeeCalendar(AddEmployeeCalendarViewModel model)
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

                result = await baseService.ExecuteNonQueryAsync(criteria);
            }

            return Json(result);
        }

        public async Task<JsonResult> GetWorkingSchedule(int employeeId, DateTime date)
        {
            var fromdate = new DateTime(date.Year, date.Month, 1);
            var criteria = new GetWorkingScheduleCriteria
            {
                FromDate = fromdate,
                ToDate = fromdate.AddMonths(1).AddDays(-1),
                EmployeeId = employeeId
            };

            IList<WorkingSchedule> schedules = await calendarService.GetWorkingSchedule(criteria);
            return Json(schedules);
        }

        public IActionResult MyWorkingSchedule()
        {
            return View();
        }

        public async Task<IActionResult> WorkingCalendars(WorkingCalendarViewModel model)
        {
            if (model != null)
            {
                if (model.Year == 0)
                {
                    model.EmployeeId = -1;
                }

                model.Teams = await teamService.GetTeam();
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

                IList<WorkingSchedule> schedules = await calendarService.GetWorkingSchedule(criteria);
                model.WorkingSchedules = schedules;
            }

            return View(model);
        }

        public async Task<IActionResult> LeaveSchedule(WorkingCalendarViewModel model)
        {
            if (model != null)
            {
                if (model.Year == 0)
                {
                    model.EmployeeId = -1;
                }

                model.Teams = await teamService.GetTeam();
                model.Month = model.Month == 0 ? DateTime.Now.Month : model.Month;
                model.Year = model.Year == 0 ? DateTime.Now.Year : model.Year;
            }

            return View(model);
        }

        public async Task<IActionResult> GetEmployeeLeaveSchedule(WorkingCalendarViewModel model)
        {
            DateTime date = new DateTime(model.Year, model.Month, 1);
            var criteria = new GetLeaveScheduleCriteria
            {
                FromDate = date,
                ToDate = date.AddMonths(1).AddDays(-1),
                EmployeeId = model.EmployeeId < 0 ? (int?)null : model.EmployeeId,
                TeamId = model.TeamId
            };

            var schedules = await calendarService.GetLeaveSchedule(criteria);

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