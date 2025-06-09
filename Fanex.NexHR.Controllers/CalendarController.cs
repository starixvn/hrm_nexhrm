namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Criteria.Calendar;
    using Fanex.NexHR.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Fanex.NexHR.Core;
    using Models;
    using Models.Calendar;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Calendar;

    public class CalendarController : BaseController
    {
        private readonly ICalendarService calendarService;
        private readonly ITeamService teamService;
        private readonly IDepartmentService departmentService;
        private readonly IBaseService baseService;

        public CalendarController(
            ISessionService sessionService,
            ICalendarService calendarService,
            ITeamService teamService,
            IDepartmentService departmentService,
            IBaseService baseService) : base(sessionService)
        {
            this.calendarService = calendarService;
            this.teamService = teamService;
            this.departmentService = departmentService;
            this.baseService = baseService;
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> UpdateEmployeeShiftDate(int employeeId, DateTime date, int? shiftId)
        {
            var command = new UpdateEmployeeShiftDateCommand
            {
                EmployeeId = employeeId,
                Date = date,
                ShiftId = shiftId,
                ModifiedBy = CurrentUser.Id,
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(command);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> InsertEmployeeCalendar(AddEmployeeCalendarViewModel model)
        {
            var result = new CommandResult();
            if (model != null)
            {
                var command = new InsertEmployeeCalendarCommand
                {
                    EmployeeId = model.AddEmployeeId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    SetOffDay = model.SetOffDay
                };

                result = await baseService.ExecuteNonQueryAsync(command);
            }

            return Json(result);
        }

        public async Task<JsonResult> GetWorkingSchedule(int employeeId, DateTime date)
        {
            var fromdate = new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            var criteria = new GetWorkingScheduleCriteria
            {
                FromDate = fromdate,
                ToDate = fromdate.AddMonths(1).AddDays(-1),
                EmployeeId = employeeId
            };

            var schedules = await baseService.GetDataAsync<WorkingSchedule>(criteria);
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
                model.Departments = await departmentService.GetAll();
                model.Month = model.Month == 0 ? DateTimeOffset.Now.DateTime.ToClientTimeZone().Month : model.Month;
                model.Year = model.Year == 0 ? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year : model.Year;
                var date = new DateTimeOffset(model.Year, model.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
                var criteria = new GetWorkingScheduleCriteria
                {
                    FromDate = date,
                    ToDate = date.AddMonths(1).AddDays(-1),
                    EmployeeId = model.EmployeeId < 0 ? (int?)null : model.EmployeeId,
                    TeamId = model.TeamId,
                    DepartmentId = model.DepartmentId
                };
                var schedules = await baseService.GetDataAsync<WorkingSchedule>(criteria);
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
                model.Departments = await departmentService.GetAll();
                model.Month = model.Month == 0 ? DateTimeOffset.Now.DateTime.ToClientTimeZone().Month : model.Month;
                model.Year = model.Year == 0 ? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year : model.Year;
            }

            return View(model);
        }

        public async Task<IActionResult> GetEmployeeLeaveSchedule(WorkingCalendarViewModel model)
        {
            var date = new DateTimeOffset(model.Year, model.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime;

            var criteria = new GetLeaveScheduleCriteria
            {
                FromDate = date,
                ToDate = date.AddMonths(1).AddDays(-1),
                EmployeeId = model.EmployeeId < 0 ? (int?)null : model.EmployeeId,
                TeamId = model.TeamId,
                DepartmentId = model.DepartmentId
            };

            var schedules = await calendarService.GetLeaveSchedule(criteria);

            return PartialView("_LeaveDaySummarySchedule", schedules);
        }

        public PartialViewResult AddEmployeeCalendar()
        {
            return PartialView(new AddEmployeeCalendarViewModel
            {
                FromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
                ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
            });
        }
    }
}