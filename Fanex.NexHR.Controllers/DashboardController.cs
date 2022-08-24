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
        private readonly INotificationService notificationService;
        private readonly ILateInEarlyOutService lateinearlyoutService;
        private readonly ICommandService commandService;
        private readonly IRedmineService redmineService;
        private readonly IReportService reportService;
        private readonly AppSetting appSetting;

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
            AppSetting appSetting) : base(sessionService)
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
            this.appSetting = appSetting;
        }

        public async Task<IActionResult> Index()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria
            {
                EmpId = CurrentUser.Id
            };

            MySpaceViewModel model = await employeeService.GetMySpace(getMySpaceCriteria);
            model.SystemReminder = await GetRemind();

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
            var today = DateTime.Now;
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

            if (model.SpentOn.Date > DateTime.Now.Date)
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
                WriteLogInputSpendTime(model);
            }
            return Json(new { isSuccessful, errors, message });
        }

        public async Task<IActionResult> GetTotalLogtimeHourOnDate(DateTime onDate)
        {
            var totalHours = await redmineService.GetTotalSpentHoursOnDate(HttpContext.User.Identity.Name, onDate);

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

        private async Task<IEnumerable<MissingCheckInOutModel>> GetMissingCheckInOut(int? employeeId)
        {
            GetMissingCheckInOutEmployeesCriteria criteria = new GetMissingCheckInOutEmployeesCriteria
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
                FromDate = DateTime.Now.AddMonths(-3),
                ToDate = DateTime.Now,
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

        public ActionResult GetNotification(int employeeId)
        {
            GetAllNotificationCriteria criteria = new GetAllNotificationCriteria { EmpId = employeeId };

            var noti = notificationService.GetViolationByEmpId(criteria);

            return PartialView("_Notification", noti);
        }

        public async Task<IActionResult> GetTeamWorkload()
        {
            GetEmployeeWorkloadCriteria criteria = new GetEmployeeWorkloadCriteria
            {
                TeamId = CurrentUser.Team.Id,
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ToDate = DateTime.Today
            };
            IList<Workload> listWorkload = await employeeService.GetWorkloadByTeam(criteria);

            return PartialView("_ListTeamWorkload", listWorkload);
        }

        [NonAction]
        private bool IsLoadTeamWorkload()
        {
            return Array.IndexOf(appSetting.IgnoreTeamWorkload.Split(','), CurrentUser.Team.Code) < 0 && CurrentUserSetting.DisplayTeamWorkload;
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