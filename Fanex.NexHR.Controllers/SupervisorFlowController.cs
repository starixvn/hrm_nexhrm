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
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;

        public SupervisorFlowController(
            ISessionService sessionService,
            ISupervisorFlowService supervisorFlowService,
            IBaseService baseService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.supervisorFlowService = supervisorFlowService;
            this.baseService = baseService;
            this.employeeService = employeeService;
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult Index()
        {
            ViewBag.Tab = "Team";
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
                        teamSupervisor.ApproversName = string.Join(", ", approvedEmployees.Select(emp => $"{emp.NickName} ({emp.FullName})"));
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
                        personal.ApproversName = string.Join(", ", approvedEmployees.Select(emp => $"{emp.NickName} ({emp.FullName})"));
                    }
                }
            }

            return PartialView("Views/SupervisorFlow/_ListPersonal.cshtml", personalSupervisors);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> GetListDetail( string keyword)
        {
            var criteria = new GetDetailSupervisorCriteria {
                Keyword = keyword ?? string.Empty
            };
            var employees = await employeeService.Query(new GetEmployeeCriteria
            {
                Query = string.Empty,
                IsDelete = false,
                IsDisable = false,
                PageSize = int.MaxValue,
                PageNumber = 1
            });

            var listDetail = await supervisorFlowService.GetListDetailSupervisors(criteria);
            if (listDetail != null) {
                foreach (var detail in listDetail) {
                    if (!string.IsNullOrEmpty(detail.ApproverIds))
                    {
                        var approverIds = detail.ApproverIds.Split(',').Select(int.Parse); 
                        var approvedEmployees = employees.Where(emp => approverIds.Contains(emp.Id));
                        detail.ApproversName = string.Join(", ", approvedEmployees.Select(emp => $"{emp.NickName} ({emp.FullName})"));
                    }
                }
            }

            return PartialView("Views/SupervisorFlow/_ListDetail.cshtml", listDetail);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> EditTeam(int groupId, string groupName, string approverIds)
        {
            var employees = await employeeService.Query(new GetEmployeeCriteria
            {
                Query = string.Empty,
                IsDelete = false,
                IsDisable = false,
                PageSize = int.MaxValue,
                PageNumber = 1
            });
            var model = new TeamViewModel {
                GroupId = groupId,
                GroupName = groupName,
                ApproverIds = !string.IsNullOrEmpty(approverIds) ? approverIds.Split(',').Select(int.Parse).ToArray() : null,
                Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.NickName))
            };

            return PartialView("Views/SupervisorFlow/EditTeam.cshtml", model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> EditTeam(TeamViewModel model)
        {
            var criteria = new UpdateTeamSupervisorCriteria {
                GroupId = model.GroupId,
                ApproverIds = model.ApproverIds != null ? string.Join(",", model.ApproverIds) : null,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create()
        {
            var employees = await employeeService.Query(new GetEmployeeCriteria
            {
                Query = string.Empty,
                IsDelete = false,
                IsDisable = false,
                PageSize = int.MaxValue,
                PageNumber = 1
            });
            var model = new PersonalViewModel {
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
            var employees = await employeeService.Query(new GetEmployeeCriteria
            {
                Query = string.Empty,
                IsDelete = false,
                IsDisable = false,
                PageSize = int.MaxValue,
                PageNumber = 1
            });
            var criteria = new GetPersonalSupervisorByIdCriteria { EmpId = empId };
            var personal = await supervisorFlowService.GetPersonalSupervisorsById(criteria);
            var model = new PersonalViewModel {
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
            var criteria = new UpdatePersonalSupervisorCriteria {
                EmpId = model.EmpId,
                ApproverIds = model.ApproverIds != null ? string.Join(",", model.ApproverIds) : null,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }
    }
}
