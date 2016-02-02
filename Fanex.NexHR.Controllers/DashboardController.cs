namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;

        public DashboardController() : this(null)
        {
        }

        public DashboardController(
            IEmployeeService employeeService = null,
            ITeamService teamService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
        }

        public ActionResult Index()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria()
            {
                EmpId = this.CurrentUser.Id
            };
            MySpaceViewModel model = _employeeService.GetMySpace(getMySpaceCriteria);
            model.SystemReminder = GetRemind();

            EmployeeAttendanceViewModel empAttendance = new EmployeeAttendanceViewModel() 
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                GroupId = this.CurrentUser.Team.Id,
                DateOption = 0,
                ExcludeUserDelete = true,
            };
            empAttendance.TeamCollection = _teamService.GetAll(new GetAllTeamCriteria());
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
    }
}
