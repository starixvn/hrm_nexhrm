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
    using Fanex.NexHR.Criteria.Report;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
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
        private readonly INotificationService notificationService;
        private readonly ILateInEarlyOutService lateinearlyoutService;
        private readonly ICommandService commandService;
        private readonly IRedmineService redmineService;
        private readonly IReportService reportService;
        private readonly IConfiguration configuration;

#pragma warning disable S107 // Methods should not have too many parameters

        public DashboardController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IRequestService requestService,
            INotificationService notificationService,
            ILateInEarlyOutService lateinearlyoutService,
            ICommandService commandService,
            IRedmineService redmineService,
            IReportService reportService,
            IConfiguration configuration) : base(sessionService)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.requestService = requestService;
            this.notificationService = notificationService;
            this.lateinearlyoutService = lateinearlyoutService;
            this.commandService = commandService;
            this.redmineService = redmineService;
            this.reportService = reportService;
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria
            {
                EmpId = CurrentUser.Id
            };

            MySpaceViewModel model = employeeService.GetMySpace(getMySpaceCriteria);
            model.SystemReminder = await GetRemind().ConfigureAwait(false);

            EmployeeAttendanceViewModel empAttendance = new EmployeeAttendanceViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                GroupId = CurrentUser.Team.Id,
                DateOption = 0,
                ExcludeUserDelete = true,
                Teams = teamService.GetTeam(),
                IsDisplayTeamAttendence = CurrentUserSetting.DisplayTeamAttendence
            };

            model.EmployeeAttendanceModel = empAttendance;
            model.NotificationLateInEarlyOutToDay = GetLateInEarlyOutNotification();
            model.IsLoadTeamWorkload = IsLoadTeamWorkload();
            model.IsHavePrivilegeCreateCheckInOutRecord = IsPrivilegeCreateCheckInOut();

            return View(model);
        }

        private IList<ViewModels.LateInEarlyOut.LateInEarlyOutViewModel> GetLateInEarlyOutNotification()
        {
            GetAllEmployeeCriteria criteria = new GetAllEmployeeCriteria
            {
                Date = DateTime.Now.Date
            };

            return lateinearlyoutService.GetAllEmployee(criteria);
        }

        private async Task<SystemReminderModel> GetRemind()
        {
            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = DateTime.Now.Date
            };

            var lstEmployee = await employeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria).ConfigureAwait(false);
            return lstEmployee;
        }

        public async Task<IActionResult> GetTodoList(int? employeeId)
        {
            var model = new ToDoViewModel();

            if (CurrentUserSetting.DisplayInfoAndTodoList)
            {
                model.MissingCheckInOutList = GetMissingCheckInOut(employeeId);
                model.PendingRequestsList = GetPendingRequest();
                model.IsAllowInputSpendTime = CurrentUserSetting.DisplayRedmineInputSpendTime;
                model.EmployeeLatetInEarlyOuts = GetEmployeeLateInEarlyOut();

                if (CurrentUserSetting.DisplayRedmineTask)
                {
                    model.InProgressTasks = await redmineService.GetInProgressIssues(HttpContext.User.Identity.Name).ConfigureAwait(false);
                }

                if (CurrentUserSetting.DisplayRemindLogtime)
                {
                    model.ForgotLogtimeDates = await redmineService.GetHaveNotLogTimeDates(HttpContext.User.Identity.Name, CurrentUser.Id).ConfigureAwait(false);
                }
            }

            return PartialView("ToDoList", model);
        }

        public ActionResult LogtimeIssue(int issueId, string issueName)
        {
            var model = new LogTimeIssue
            {
                IssueId = issueId,
                IssueName = issueName,
                SpentOn = DateTime.Now,
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

            if (model.SpentOn.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("SpentOn", Labels.SpentOnErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var commandResult = await redmineService.LogtimeToIssue(model, HttpContext.User.Identity.Name).ConfigureAwait(false);
                isSuccessful = commandResult.IsSuccessful;
                message = commandResult.Message;
            }

            var errors = ModelState
                .Where(item => item.Value.Errors.Any())
                .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage });

            if (isSuccessful)
            {
                WriteLogInputSpendTime(model);
            }
            return Json(new { isSuccessful, errors, message });
        }

        public async Task<IActionResult> GetTotalLogtimeHourOnDate(DateTime onDate)
        {
            var totalHours = await redmineService.GetTotalSpentHoursOnDate(HttpContext.User.Identity.Name, onDate).ConfigureAwait(false);

            return Json(new { totalHours });
        }

        private void WriteLogInputSpendTime(LogTimeIssue issue)
        {
            var criteria = new WriteLogInputSpendTimeRedmineCriteria
            {
                EmpId = CurrentUser.Id,
                IssueId = issue.IssueId,
                SpentOn = issue.SpentOn,
                Hours = issue.Hours,
                Comments = issue.Comments,
                Activity = issue.Activity,
                CreatedDate = DateTime.Now
            };

            commandService.ExecuteNonQuery(criteria);
        }

        private IList<MissingCheckInOutModel> GetMissingCheckInOut(int? employeeId)
        {
            GetMissingCheckInOutEmployeesCriteria criteria = new GetMissingCheckInOutEmployeesCriteria
            {
                EmpId = employeeId
            };
            return employeeService.GetMissingCheckInOutByEmpId(criteria);
        }

        private IList<RequestEntity> GetPendingRequest()
        {
            var criteria = new GetRequestCriteria
            {
                ActorId = CurrentUser.Id,
                Role = (short)Role.AssistantLeader,
                FromDate = DateTime.Now.AddMonths(-3),
                ToDate = DateTime.Now,
                GroupId = null,
                Status = 1,
                ExcludeEmployeeId = CurrentUser.Id,
                RequestType = string.Empty,
                IsFilterByEffectedDate = false,
                ItemsPerPage = 100
            };

            IList<RequestEntity> entityList = requestService.GetRequest(criteria);

            return entityList;
        }

        private IList<EmployeeLateInEarlyOut> GetEmployeeLateInEarlyOut()
        {
            var criteria = new GetEmployeeAttendanceLateInEarlyOutCriteria
            {
                EmpId = CurrentUser.Id
            };

            IList<EmployeeLateInEarlyOut> employees = employeeService.GetEmployeeLateInEarlyOut(criteria);

            return employees;
        }

        public ActionResult GetNotification(int employeeId)
        {
            GetAllNotificationCriteria criteria = new GetAllNotificationCriteria { EmpId = employeeId };

            var noti = notificationService.GetViolationByEmpId(criteria);

            return PartialView("_Notification", noti);
        }

        public ActionResult GetTeamWorkload()
        {
            GetEmployeeWorkloadCriteria criteria = new GetEmployeeWorkloadCriteria
            {
                TeamId = CurrentUser.Team.Id,
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ToDate = DateTime.Today
            };
            IList<Workload> listWorkload = employeeService.GetWorkloadByTeam(criteria);

            return PartialView("_ListTeamWorkload", listWorkload);
        }

        [NonAction]
        private bool IsLoadTeamWorkload()
        {
            var ignoreTeamWorkloads = configuration.GetSection("IgnoreTeamWorkload").Value.Split(',');
            return Array.IndexOf(ignoreTeamWorkloads, CurrentUser.Team.Code) < 0 && CurrentUserSetting.DisplayTeamWorkload;
        }

        private bool IsPrivilegeCreateCheckInOut()
        {
            var criteria = new GetPrivilegeCreateCheckInOutRecordCriteria
            {
                EmpId = CurrentUser.Id
            };

            var isPrivilegeCreateCheckInOut = reportService.GetIsPrivilegeCreateCheckInOut(criteria);
            return isPrivilegeCreateCheckInOut;
        }
    }
}