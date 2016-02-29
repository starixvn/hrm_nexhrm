namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Report;

    public class DashboardController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IRequestService _requestService;

        public DashboardController()
            : this(null, null, null)
        {
        }

        public DashboardController(
            IEmployeeService employeeService,
            ITeamService teamService,
            IRequestService requestService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _requestService = requestService ?? new RequestService();
        }

        public ActionResult Index()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria()
            {
                EmpId = CurrentUser.Id
            };
            MySpaceViewModel model = _employeeService.GetMySpace(getMySpaceCriteria);
            model.SystemReminder = GetRemind();

            EmployeeAttendanceViewModel empAttendance = new EmployeeAttendanceViewModel()
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                GroupId = CurrentUser.Team.Id,
                DateOption = 0,
                ExcludeUserDelete = true,
            };
            empAttendance.Teams = _teamService.GetAll();
            model.EmployeeAttendanceModel = empAttendance;

            return View(model);
        }

        private SystemReminderModel GetRemind()
        {
            SystemReminderModel lstEmployee = null;
            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = DateTime.Now.Date
            };

            lstEmployee = _employeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
            return lstEmployee;
        }

        public ActionResult GetTodoList(int? employeeId)
        {
            ToDoViewModel model = new ToDoViewModel() 
            {
                MissingCheckInOutList = GetMissingCheckInOut(employeeId),
                PendingRequestsList = GetPendingRequest()
            };
            
            return PartialView("ToDoList", model);
        }

        public IList<MissingCheckInOutModel> GetMissingCheckInOut(int? employeeId)
        {
            GetMissingCheckInOutEmployeesCriteria criteria = new GetMissingCheckInOutEmployeesCriteria()
            {
                EmpId = employeeId
            };
            return _employeeService.GetMissingCheckInOutByEmpId(criteria);            
        }
   
        public IList<RequestEntity> GetPendingRequest()
        {
            var criteria = new GetRequestCriteria()
            {
                Role = (short)Role.AssistantLeader,
                FromDate = DateTime.Now.AddDays(-30),
                ToDate = DateTime.Now,                
                GroupId = null,               
                Status = 1,
                ExcludeEmployeeId = new int?(CurrentUser.Id),
                RequestType = string.Empty,
                IsFilterByEffectedDate = false,                
                ItemsPerPage = 100
            };

            IList<RequestEntity> entityList = _requestService.GetRequest(criteria);

            return entityList;
        }
    }
}
