namespace Fanex.NexHR.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.SupervisorFlow;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.SupervisorFlow;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SupervisorFlowController : BaseController
    {
        private readonly ISupervisorFlowService supervisorFlowService;

        private readonly IDepartmentService departmentService;

        private readonly ITeamService teamService;

        private readonly IBaseService baseService;

        public SupervisorFlowController(
            ISessionService sessionService,
            ISupervisorFlowService supervisorFlowService,
            IBaseService baseService,
            IDepartmentService departmentService,
            ITeamService teamService) : base(sessionService)
        {
            this.supervisorFlowService = supervisorFlowService;
            this.baseService = baseService;
            this.departmentService = departmentService;
            this.teamService = teamService;
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Index()
        {
            var model = new SupervisorFlowViewModel
            {
                Departments = await departmentService.GetAll(),
                Teams = await teamService.GetTeam()
            };
            ViewBag.Tab = "Team";
            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> GetTeamSupervisor()
        {
            var criteria = new GetTeamSupervisorCriteria();
            var teamSupervisors = await supervisorFlowService.GetTeamSupervisors(criteria);

            return PartialView("Views/SupervisorFlow/_ListTeam.cshtml", teamSupervisors);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> GetPersonalSupervisor()
        {
            var criteria = new GetPersonalSupervisorCriteria();
            var personalSupervisors = await supervisorFlowService.GetPersonalSupervisors(criteria);

            return PartialView("Views/SupervisorFlow/_ListPersonal.cshtml", personalSupervisors);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> GetListDetail(string keyword, int? departmentId, int? teamId)
        {
            var criteria = new GetDetailSupervisorCriteria
            {
                Keyword = keyword ?? string.Empty,
                DepartmentId = departmentId,
                GroupId = teamId
            };
            var listDetail = await supervisorFlowService.GetListDetailSupervisors(criteria);

            return PartialView("Views/SupervisorFlow/_ListDetail.cshtml", listDetail);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> EditTeam(int groupId)
        {
            var employees = await baseService.GetDataAsync<StaffInfo>(new GetStaffInfoCriteria
            {
                Query = string.Empty,
                TeamId = null,
                IsSpecial = null
            });

            var teamSupervisors = await supervisorFlowService.GetById(new GetTeamSupervisorByIdCriteria { GroupId = groupId });
            var model = new TeamViewModel
            {
                GroupId = teamSupervisors.GroupId,
                GroupName = teamSupervisors.GroupName,
                ApproverIds = !string.IsNullOrEmpty(teamSupervisors.ApproverIds) ? teamSupervisors.ApproverIds.Split(',').Select(int.Parse).ToArray() : null,
                Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.NickName))
            };

            return PartialView("Views/SupervisorFlow/EditTeam.cshtml", model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> EditTeam(TeamViewModel model)
        {
            var criteria = new UpdateTeamSupervisorCriteria
            {
                GroupId = model.GroupId,
                ApproverIds = model.ApproverIds != null ? string.Join(",", model.ApproverIds) : null,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> CreatePersonal()
        {
            var employees = await baseService.GetDataAsync<StaffInfo>(new GetStaffInfoCriteria
            {
                Query = string.Empty,
                TeamId = null,
                IsSpecial = null
            });
            var model = new PersonalViewModel
            {
                Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.NickName))
            };

            return PartialView("Views/SupervisorFlow/CreatePersonal.cshtml", model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> CreatePersonal(PersonalViewModel model)
        {
            var criteria = new InsertPersonalSupervisorCriteria
            {
                EmpId = model.EmpId,
                ApproverIds = model.ApproverIds != null ? string.Join(",", model.ApproverIds) : null,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> EditPersonal(int empId)
        {
            var employees = await baseService.GetDataAsync<StaffInfo>(new GetStaffInfoCriteria
            {
                Query = string.Empty,
                TeamId = null,
                IsSpecial = null
            });
            var criteria = new GetPersonalSupervisorByIdCriteria { EmpId = empId };
            var personal = await supervisorFlowService.GetPersonalSupervisorsById(criteria);
            var model = new PersonalViewModel
            {
                EmpId = empId,
                Employee = personal.Nickname + (string.IsNullOrWhiteSpace(personal.FullName) ? string.Empty : " (" + personal.FullName + ")"),
                ApproverIds = !string.IsNullOrEmpty(personal.ApproverIds) ? personal.ApproverIds.Split(',').Select(int.Parse).ToArray() : null,
                Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.NickName))
            };

            return PartialView("Views/SupervisorFlow/EditPersonal.cshtml", model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> EditPersonal(PersonalViewModel model)
        {
            var criteria = new UpdatePersonalSupervisorCriteria
            {
                EmpId = model.EmpId,
                ApproverIds = model.ApproverIds != null ? string.Join(",", model.ApproverIds) : null,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }
    }
}
