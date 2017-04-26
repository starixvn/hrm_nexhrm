namespace Fanex.NexHR.Controllers
{
    using Core;
    using Core.Enums;
    using Criteria;
    using Criteria.Employee;
    using Criteria.LateInEarlyOut;
    using Criteria.Notification;
    using Criteria.Request;
    using Criteria.Workload;
    using Models;
    using Models.Notification;
    using Models.Request;
    using Models.Workload;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using ViewModels;
    using ViewModels.Report;

    public class DashboardController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IRequestService _requestService;
        private readonly INotificationService _notificationService;
        private readonly ILateInEarlyOutService _lateinearlyoutService;

        public DashboardController()
            : this(null, null, null, null, null)
        {
        }

        public DashboardController(
            IEmployeeService employeeService,
            ITeamService teamService,
            IRequestService requestService,
            INotificationService notificationService,
            ILateInEarlyOutService lateinearlyoutService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _requestService = requestService ?? new RequestService();
            _notificationService = notificationService ?? new NotificationService();
            _lateinearlyoutService = lateinearlyoutService ?? new LateInEarlyOutService();
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
        public ActionResult GetTodoList(int? employeeId)
        {
            ToDoViewModel model = new ToDoViewModel
            {
                MissingCheckInOutList = GetMissingCheckInOut(employeeId),
                PendingRequestsList = GetPendingRequest()
            };

            return PartialView("ToDoList", model);
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
            Notification noti = _notificationService.GetViolationByEmpId(criteria);

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
            return Array.IndexOf(ignoreTeamWorkloads, CurrentUser.Team.Code) < 0;
        }
    }
}