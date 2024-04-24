namespace Fanex.NexHR.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.SupervisorFlow;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SupervisorFlowController : BaseController
    {
        private readonly ISupervisorFlowService supervisorFlowService;
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;

        public SupervisorFlowController(
            ISessionService sessionService,
            ISupervisorFlowService supervisorFlowService,
            IBaseService baseService,
            ITeamService teamService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.supervisorFlowService = supervisorFlowService;
            this.teamService = teamService;
            this.baseService = baseService;
            this.employeeService = employeeService;
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
         public IActionResult Index(int chosenTab = 0)
        {
            switch (chosenTab)
            {
                case 1:
                    ViewBag.Tab = "Personal";
                    break;
                default:
                    ViewBag.Tab = "Team";
                    break;
            }

            return View();
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> TeamSupervisor()
        {
            var criteria = new GetTeamSupervisorCriteria();
            var employees = await employeeService.Query(new GetEmployeeCriteria
            {
                Query = string.Empty,
                IsDelete = false,
                IsDisable = false,
                PageSize = int.MaxValue,
                PageNumber = 1
            });

            var teamSupervisors = await supervisorFlowService.GetTeamSupervisors(criteria);
            if (teamSupervisors != null) {
                foreach (var teamSupervisor in teamSupervisors) {
                    if (!string.IsNullOrEmpty(teamSupervisor.ApproverIds))
                    {
                        var approverIds = teamSupervisor.ApproverIds.Split(',').Select(int.Parse); 
                        var approvedEmployees = employees.Where(emp => approverIds.Contains(emp.Id));
                        teamSupervisor.ApproverNames = string.Join(", ", approvedEmployees.Select(emp => $"{emp.NickName} ({emp.FullName})"));
                    }
                }
            }
            ViewBag.team = await teamService.GetTeam();
            ViewBag.employees =new SelectList(employees, nameof(Employee.Id), nameof(Employee.NickName));

            return PartialView("Views/SupervisorFlow/_Team.cshtml", teamSupervisors);
        }

        // [HttpPost]
        // [HrmAuthorize(UserRoles = "HR")]
        // public async Task<IActionResult> TeamUpdate(int teamId, string ApproverId)
        // {
        //     var criteria = new UpdateTeamSupervisorCriteria {

        //         GroupId = teamId,
        //         ApproverIds = ApproverId,
        //         ModifiedBy = CurrentUser.Id
        //     }

        //     var result = await baseService.ExecuteNonQueryAsync(criteria);

        //     if (result.IsSuccessful)
        //     {
        //         ViewBag.Inserted = true;
        //     }
        //     else
        //     {
        //         ModelState.AddModelError(string.Empty, result.Message);
        //     }

        //     return View(result);
        // }
    }
}
