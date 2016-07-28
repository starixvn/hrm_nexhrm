namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.Criteria.Notification;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Workload;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Notification;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Workload;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Report;

    public class DashboardController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IRequestService requestService;
        private readonly INotificationService notificationService;
        private readonly ILateInEarlyOutService lateinearlyoutService;

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
            this.employeeService = employeeService ?? new EmployeeService();
            this.teamService = teamService ?? new TeamService();
            this.requestService = requestService ?? new RequestService();
            this.notificationService = notificationService ?? new NotificationService();
            this.lateinearlyoutService = lateinearlyoutService ?? new LateInEarlyOutService();
        }
         
        [Authorize(Roles = "SpecialUser")]
        public ActionResult Index()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria()
            {
                EmpId = CurrentUser.Id
            };
            MySpaceViewModel model = employeeService.GetMySpace(getMySpaceCriteria);
            model.SystemReminder = GetRemind();

            EmployeeAttendanceViewModel empAttendance = new EmployeeAttendanceViewModel()
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                GroupId = CurrentUser.Team.Id,
                DateOption = 0,
                ExcludeUserDelete = true,
            };
            empAttendance.Teams = teamService.Get();
            model.EmployeeAttendanceModel = empAttendance;

            model.NotificationLateInEarlyOutToDay = GetLateInEarlyOutNotification();
            return View(model);
        }

        private IList<Fanex.NexHR.ViewModels.LateInEarlyOut.LateInEarlyOutViewModel> GetLateInEarlyOutNotification()
        {
            GetAllEmployeeCriteria criteria = new GetAllEmployeeCriteria()
            {
                Date = DateTime.Now.Date
            };

            return lateinearlyoutService.GetAllEmployee(criteria);
        }

        private SystemReminderModel GetRemind()
        {
            SystemReminderModel lstEmployee = null;
            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = DateTime.Now.Date
            };

            lstEmployee = employeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
            return lstEmployee;
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult GetTodoList(int? employeeId)
        {
            ToDoViewModel model = new ToDoViewModel()
            {
                MissingCheckInOutList = GetMissingCheckInOut(employeeId),
                PendingRequestsList = GetPendingRequest()
            };

            return PartialView("ToDoList", model);
        }

        private IList<MissingCheckInOutModel> GetMissingCheckInOut(int? employeeId)
        {
            GetMissingCheckInOutEmployeesCriteria criteria = new GetMissingCheckInOutEmployeesCriteria()
            {
                EmpId = employeeId
            };
            return employeeService.GetMissingCheckInOutByEmpId(criteria);
        }

        private IList<RequestEntity> GetPendingRequest()
        {
            var criteria = new GetRequestCriteria()
            {
                ActorId = CurrentUser.Id,
                Role = (short)Role.AssistantLeader,
                FromDate = DateTime.Now.AddMonths(-3),
                ToDate = DateTime.Now,
                GroupId = null,
                Status = 1,
                ExcludeEmployeeId = new int?(CurrentUser.Id),
                RequestType = string.Empty,
                IsFilterByEffectedDate = false,
                ItemsPerPage = 100
            };

            IList<RequestEntity> entityList = requestService.GetRequest(criteria);

            return entityList;
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult GetNotification(int employeeId)
        {
            GetAllNotificationCriteria criteria = new GetAllNotificationCriteria() { EmpId = employeeId };
            Notification noti = notificationService.GetViolationByEmpId(criteria);

            return PartialView("_Notification", noti);
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult GetTeamWorkload()
        {
            GetEmployeeWorkloadCriteria criteria = new GetEmployeeWorkloadCriteria()
            {
                TeamId = CurrentUser.Team.Id,
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ToDate = DateTime.Today
            };
            IList<Workload> listWorkload = employeeService.GetWorkloadByTeam(criteria);

            return PartialView("_ListTeamWorkload", listWorkload);
        }
    }
}
