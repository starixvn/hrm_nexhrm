namespace Fanex.NexHR.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.SupervisorFlow;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.SupervisorFlow;
    using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetTeamSupervisor()
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

            return PartialView("Views/SupervisorFlow/_ListTeam.cshtml", teamSupervisors);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> GetPersonalSupervisor()
        {
            var criteria = new GetPersonalSupervisorCriteria();
            var employees = await employeeService.Query(new GetEmployeeCriteria
            {
                Query = string.Empty,
                IsDelete = false,
                IsDisable = false,
                PageSize = int.MaxValue,
                PageNumber = 1
            });

            var personalSupervisors = await supervisorFlowService.GetPersonalSupervisors(criteria);
            if (personalSupervisors != null) {
                foreach (var personal in personalSupervisors) {
                    if (!string.IsNullOrEmpty(personal.ApproverIds))
                    {
                        var approverIds = personal.ApproverIds.Split(',').Select(int.Parse); 
                        var approvedEmployees = employees.Where(emp => approverIds.Contains(emp.Id));
                        personal.ApproverNames = string.Join(", ", approvedEmployees.Select(emp => $"{emp.NickName} ({emp.FullName})"));
                    }
                }
            }

            return PartialView("Views/SupervisorFlow/_ListPersonal.cshtml", personalSupervisors);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> EditTeam(int id)
        {
            var team = await supervisorFlowService.GetById(new GetTeamSupervisorByIdCriteria { GroupId = id });
            var model = new UpdateSupervisorFlowViewModel();
            if (team != null)
            {
                model.GroupId = team.GroupId;
                model.GroupName= team.GroupName;
                model.ApproverIds = team.ApproverIds;
            }

            return PartialView("Views/SupervisorFlow/EditTeam.cshtml", model);
        }

        // [HttpGet]
        // [HrmAuthorize(UserRoles = "HR")]
        // public PartialViewResult EditTeam( int groupId, string groupName, string approverIds)
        // {
        //     return PartialView(new UpdateSupervisorFlowViewModel
        //     {
        //         GroupId = groupId,
        //         GroupName = groupName,
        //         ApproverIds = approverIds
        //     });
        // }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> TeamUpdate(UpdateSupervisorFlowViewModel model)
        {
            var result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };

            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateTeamSupervisorCriteria {
                    GroupId = model.GroupId,
                    ApproverIds = model.ApproverIds,
                    ModifiedBy = CurrentUser.Id
                };

                result = await baseService.ExecuteNonQueryAsync(criteria);

                if (result.IsSuccessful)
                {
                    ViewBag.Inserted = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return Json(result);
        }
    }
}
