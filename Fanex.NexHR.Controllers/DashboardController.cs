namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria;
    using Criteria.Employee;
    using Criteria.LateInEarlyOut;
    using Criteria.Notification;
    using Criteria.RedmineAPI;
    using Criteria.Request;
    using Criteria.Workload;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Notification;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.RedmineAPI;
    using Models.Report;
    using Models.Request;
    using Models.Workload;
    using Resources;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Report;

    public class DashboardController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IRequestService requestService;
        private readonly ILateInEarlyOutService lateinearlyoutService;
        private readonly IBaseService baseService;
        private readonly IRedmineService redmineService;
        private readonly IReportService reportService;
        private readonly AppSetting appSetting;

#pragma warning disable S107 // Methods should not have too many parameters

        public DashboardController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IRequestService requestService,
            ILateInEarlyOutService lateinearlyoutService,
            IBaseService baseService,
            IRedmineService redmineService,
            IReportService reportService,
            AppSetting appSetting) : base(sessionService)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.requestService = requestService;
            this.lateinearlyoutService = lateinearlyoutService;
            this.baseService = baseService;
            this.redmineService = redmineService;
            this.reportService = reportService;
            this.appSetting = appSetting;
        }

        public async Task<IActionResult> Index()
        {
            var model = await GetMySpaceModelAsync();

            var systemReminder = GetRemind();
            var attendance = GetEmployeeAttendaneAsync();
            var notificationLateInEarlyOut = GetLateInEarlyOutNotification();
            var isPrivilegeCreateCheckInOut = IsPrivilegeCreateCheckInOut();

            await Task.WhenAll(systemReminder, attendance, notificationLateInEarlyOut, isPrivilegeCreateCheckInOut);

            model.SystemReminder = await systemReminder;
            model.EmployeeAttendanceModel = await attendance;
            model.NotificationLateInEarlyOutToDay = await notificationLateInEarlyOut;
            model.IsLoadTeamWorkload = IsLoadTeamWorkload();
            model.IsHavePrivilegeCreateCheckInOutRecord = await isPrivilegeCreateCheckInOut;

            return View(model);
        }

        private async Task<MySpaceViewModel> GetMySpaceModelAsync()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria
            {
                EmpId = CurrentUser.Id
            };
            var model = await employeeService.GetMySpace(getMySpaceCriteria);
            model.IgnoreShowLeaveRequest = appSetting.IgnoreShowLeaveRequest;
            return model;
        }

        private async Task<EmployeeAttendanceViewModel> GetEmployeeAttendaneAsync()
            => new EmployeeAttendanceViewModel
            {
                FromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                GroupId = CurrentUser.TeamId,
                DateOption = 0,
                ExcludeUserDelete = true,
                Teams = await teamService.GetTeam(),
                IsDisplayTeamAttendence = CurrentUserSetting.DisplayTeamAttendence
            };

        private async Task<IEnumerable<ViewModels.LateInEarlyOut.LateInEarlyOutViewModel>> GetLateInEarlyOutNotification()
        {
            var criteria = new GetAllLatInEarlyOutNotificationCriteria
            {
                Date = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date
            };

            return await lateinearlyoutService.GetAllEmployee(criteria);
        }

        private async Task<SystemReminderModel> GetRemind()
        {
            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date
            };

            var lstEmployee = await employeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
            return lstEmployee;
        }

        public async Task<IActionResult> GetTodoList(int? employeeId)
        {
            var model = new ToDoViewModel();

            if (CurrentUserSetting.DisplayInfoAndTodoList)
            {
                model.MissingCheckInOutList = await GetMissingCheckInOut(employeeId);
                model.PendingRequestsList = await GetPendingRequest();
                model.IsAllowInputSpendTime = CurrentUserSetting.DisplayRedmineInputSpendTime;
                model.EmployeeLatetInEarlyOuts = await GetEmployeeLateInEarlyOut();

                if (CurrentUserSetting.DisplayRedmineTask)
                {
                    model.InProgressTasks = await redmineService.GetInProgressIssues(HttpContext.User.Identity.Name);
                }

                if (CurrentUserSetting.DisplayRemindLogtime)
                {
                    model.ForgotLogtimeDates = await redmineService.GetHaveNotLogTimeDates(HttpContext.User.Identity.Name, CurrentUser.Id);
                }
            }

            return PartialView("ToDoList", model);
        }

        public IActionResult LogtimeIssue(int issueId, string issueName)
        {
            var today = DateTimeOffset.Now.DateTime.ToClientTimeZone();
            var model = new LogTimeIssue
            {
                IssueId = issueId,
                IssueName = issueName,
                SpentOn = today,
                Hours = null,
                Activity = RedmineLogTimeActivity.Development
            };
            return PartialView("_LogtimeDialog", model);
        }

        [HttpPost]
        public async Task<IActionResult> LogtimeIssue(LogTimeIssue model)
        {
            var isSuccessful = false;
            var message = string.Empty;

            if (model.Hours.Equals(0.0))
            {
                ModelState.AddModelError("Hours", Labels.LogtimeHoursErrrorMessage);
            }

            if (model.SpentOn.Date > DateTimeOffset.Now.DateTime.ToClientTimeZone().Date)
            {
                ModelState.AddModelError("SpentOn", Labels.SpentOnErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var commandResult = await redmineService.LogtimeToIssue(model, HttpContext.User.Identity.Name);
                isSuccessful = commandResult.IsSuccessful;
                message = commandResult.Message;
            }

            var errors = ModelState
                .Where(item => item.Value.Errors.Any())
                .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage });

            if (isSuccessful)
            {
                await WriteLogInputSpendTime(model);
            }
            return Json(new { isSuccessful, errors, message });
        }

        public async Task<IActionResult> GetTotalLogtimeHourOnDate(DateTime onDate)
        {
            var totalHours = await redmineService.GetTotalSpentHoursOnDate(HttpContext.User.Identity.Name, onDate);

            return Json(new { totalHours });
        }

        private async Task WriteLogInputSpendTime(LogTimeIssue issue)
        {
            var criteria = new WriteLogInputSpendTimeRedmineCriteria
            {
                EmpId = CurrentUser.Id,
                IssueId = issue.IssueId,
                SpentOn = issue.SpentOn,
                Hours = issue.Hours,
                Comments = issue.Comments,
                Activity = issue.Activity,
                CreatedDate = DateTimeOffset.Now.DateTime.ToClientTimeZone()
            };

            await baseService.ExecuteNonQueryAsync(criteria);
        }

        private async Task<IEnumerable<MissingCheckInOutModel>> GetMissingCheckInOut(int? employeeId)
        {
            var criteria = new GetMissingCheckInOutEmployeesCriteria
            {
                EmpId = employeeId
            };
            return await employeeService.GetMissingCheckInOutByEmpId(criteria);
        }

        private async Task<IList<RequestEntity>> GetPendingRequest()
        {
            var criteria = new GetRequestCriteria
            {
                ActorId = CurrentUser.Id,
                Role = (short)Role.AssistantLeader,
                FromDate = DateTimeOffset.Now.AddMonths(-3).DateTime.ToClientTimeZone(),
                ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                GroupId = null,
                Status = 1,
                ExcludeEmployeeId = CurrentUser.Id,
                RequestType = string.Empty,
                IsFilterByEffectedDate = false,
                ItemsPerPage = 100
            };

            IList<RequestEntity> entityList = await requestService.GetRequest(criteria);

            return entityList;
        }

        private async Task<IEnumerable<EmployeeLateInEarlyOut>> GetEmployeeLateInEarlyOut()
        {
            var criteria = new GetEmployeeAttendanceLateInEarlyOutCriteria
            {
                EmpId = CurrentUser.Id
            };

            IEnumerable<EmployeeLateInEarlyOut> employees = await employeeService.GetEmployeeLateInEarlyOut(criteria);

            return employees;
        }

        public async Task<IActionResult> GetNotification(int employeeId)
        {
            var criteria = new GetAllNotificationCriteria { EmpId = employeeId };
            var noti = await baseService.GetDataEntityAsync<Notification>(criteria);

            return PartialView("_Notification", noti ?? new Notification());
        }

        public async Task<IActionResult> GetTeamWorkload()
        {
            var today = DateTimeOffset.Now.DateTime.ToClientTimeZone();
            var criteria = new GetEmployeeWorkloadCriteria
            {
                TeamId = CurrentUser.TeamId,
                FromDate = new DateTimeOffset(today.Year, today.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = today.Date
            };
            IList<Workload> listWorkload = await employeeService.GetWorkloadDefaultByTeam(criteria);
            await GetWorkloadByTeam(
                listWorkload,
                new DateTimeOffset(today.Year, today.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                today.Date.AddDays(-1));

            return PartialView("_ListTeamWorkload", listWorkload);
        }

        private async Task GetWorkloadByTeam(IList<Workload> employees, DateTime fromDate, DateTime toDate)
        {
            const int NumberRounding = 2;

            for (int i = 0; i < employees.Count; i++)
            {
                var domainName = employees[i].Employee.DomainUsername;
                var redmineUserId = await redmineService.GetCurrentRedmineId(domainName);
                if (redmineUserId > 0)
                {
                    var timeEntries = await redmineService.GetTimeEntries(redmineUserId, fromDate, toDate);
                    var totalHour = GetTotalHourLogTime(timeEntries);
                    if (employees[i].WorkHour == 0)
                    {
                        employees[i].PercentWorkload = 0;
                    }
                    else
                    {
                        employees[i].PercentWorkload = Math.Round(totalHour / employees[i].WorkHour, NumberRounding);
                    }
                }
            }
        }

        [NonAction]
        private static double GetTotalHourLogTime(IList<TimeEntry> timeEntries)
        {
            double total = 0;
            foreach (var timeEntry in timeEntries)
            {
                total += timeEntry.Hours;
            }
            return total;
        }

        [NonAction]
        private bool IsLoadTeamWorkload()
        {
            return Array.IndexOf(appSetting.IgnoreTeamWorkload.Split(','), CurrentUser.GroupTeamId) < 0 && CurrentUserSetting.DisplayTeamWorkload;
        }

        private async Task<bool> IsPrivilegeCreateCheckInOut()
        {
            var criteria = new GetPrivilegeCreateCheckInOutRecordCriteria
            {
                EmpId = CurrentUser.Id
            };

            var isPrivilegeCreateCheckInOut = await reportService.GetIsPrivilegeCreateCheckInOut(criteria);
            return isPrivilegeCreateCheckInOut;
        }
    }
}