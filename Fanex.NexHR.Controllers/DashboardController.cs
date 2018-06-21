namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core;
    using Core.Enums;
    using Criteria;
    using Criteria.Employee;
    using Criteria.LateInEarlyOut;
    using Criteria.Notification;
    using Criteria.RedmineAPI;
    using Criteria.Request;
    using Criteria.Workload;
    using Models;
    using Models.RedmineAPI;
    using Models.Request;
    using Models.Workload;
    using Resources;
    using Services;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Report;

    public class DashboardController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IRequestService _requestService;
        private readonly INotificationService _notificationService;
        private readonly ILateInEarlyOutService _lateinearlyoutService;
        private readonly ICommandService _commandService;
        private readonly IRedmineService _redmineService;

        public DashboardController()
            : this(null, null, null, null, null, null, null)
        {
        }

        public DashboardController(
            IEmployeeService employeeService,
            ITeamService teamService,
            IRequestService requestService,
            INotificationService notificationService,
            ILateInEarlyOutService lateinearlyoutService,
            ICommandService commandService,
        IRedmineService redmineService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _requestService = requestService ?? new RequestService();
            _notificationService = notificationService ?? new NotificationService();
            _lateinearlyoutService = lateinearlyoutService ?? new LateInEarlyOutService();
            _commandService = commandService ?? new CommandService();
            _redmineService = redmineService ?? new RedmineService();
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult Index()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria
            {
                EmpId = CurrentUser.Id
            };

            MySpaceViewModel model = _employeeService.GetMySpace(getMySpaceCriteria);
            model.SystemReminder = GetRemind();

            EmployeeAttendanceViewModel empAttendance = new EmployeeAttendanceViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                GroupId = CurrentUser.Team.Id,
                DateOption = 0,
                ExcludeUserDelete = true,
                Teams = _teamService.GetTeam(),
                IsDisplayTeamAttendence = CurrentUserSetting.DisplayTeamAttendence
            };

            model.EmployeeAttendanceModel = empAttendance;
            model.NotificationLateInEarlyOutToDay = GetLateInEarlyOutNotification();
            model.IsLoadTeamWorkload = IsLoadTeamWorkload();

            return View(model);
        }

        private IList<ViewModels.LateInEarlyOut.LateInEarlyOutViewModel> GetLateInEarlyOutNotification()
        {
            GetAllEmployeeCriteria criteria = new GetAllEmployeeCriteria
            {
                Date = DateTime.Now.Date
            };

            return _lateinearlyoutService.GetAllEmployee(criteria);
        }

        private SystemReminderModel GetRemind()
        {
            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = DateTime.Now.Date
            };

            var lstEmployee = _employeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
            return lstEmployee;
        }

        [Authorize(Roles = "SpecialUser")]
        public async Task<ActionResult> GetTodoList(int? employeeId)
        {
            var model = new ToDoViewModel();

            if (CurrentUserSetting.DisplayInfoAndTodoList)
            {
                model.MissingCheckInOutList = GetMissingCheckInOut(employeeId);
                model.PendingRequestsList = GetPendingRequest();
                model.IsAllowInputSpendTime = CurrentUserSetting.DisplayRedmineInputSpendTime;
                if (CurrentUserSetting.DisplayRedmineTask)
                {
                    model.InProgressTasks = await _redmineService.GetInProgressIssues(HttpContext.User.Identity.Name).ConfigureAwait(false);
                    model.ForgotLogtimeDates = await _redmineService.GetHaveNotLogTimeDates(HttpContext.User.Identity.Name, CurrentUser.Id).ConfigureAwait(false);
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
        public async Task<ActionResult> LogtimeIssue(LogTimeIssue model)
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
                var commandResult = await _redmineService.LogtimeToIssue(model, HttpContext.User.Identity.Name).ConfigureAwait(false);
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

        public async Task<ActionResult> GetTotalLogtimeHourOnDate(DateTime onDate)
        {
            var totalHours = await _redmineService.GetTotalSpentHoursOnDate(HttpContext.User.Identity.Name, onDate).ConfigureAwait(false);

            return Json(new { totalHours }, JsonRequestBehavior.AllowGet);
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

            _commandService.Insert(criteria);
        }

        private IList<MissingCheckInOutModel> GetMissingCheckInOut(int? employeeId)
        {
            GetMissingCheckInOutEmployeesCriteria criteria = new GetMissingCheckInOutEmployeesCriteria
            {
                EmpId = employeeId
            };
            return _employeeService.GetMissingCheckInOutByEmpId(criteria);
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

            IList<RequestEntity> entityList = _requestService.GetRequest(criteria);

            return entityList;
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult GetNotification(int employeeId)
        {
            GetAllNotificationCriteria criteria = new GetAllNotificationCriteria { EmpId = employeeId };

            var noti = _notificationService.GetViolationByEmpId(criteria);

            return PartialView("_Notification", noti);
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult GetTeamWorkload()
        {
            GetEmployeeWorkloadCriteria criteria = new GetEmployeeWorkloadCriteria
            {
                TeamId = CurrentUser.Team.Id,
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ToDate = DateTime.Today
            };
            IList<Workload> listWorkload = _employeeService.GetWorkloadByTeam(criteria);

            return PartialView("_ListTeamWorkload", listWorkload);
        }

        [NonAction]
        private bool IsLoadTeamWorkload()
        {
            var ignoreTeamWorkloads = AppSetting.IgnoreTeamWorkload.Split(',');
            return Array.IndexOf(ignoreTeamWorkloads, CurrentUser.Team.Code) < 0 && CurrentUserSetting.DisplayTeamWorkload;
        }
    }
}