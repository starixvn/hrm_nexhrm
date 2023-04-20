namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.EmployeeExperience;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.EmployeeExperience;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    public class EmployeeExperienceController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;
        private readonly IEmployeeExperienceService employeeExperienceService;
        private const string Certificate = "Certificate";
        public EmployeeExperienceController(ISessionService sessionService,
            ITeamService teamService,
            IEmployeeExperienceService employeeExperienceService,
            IBaseService baseService) : base(sessionService)
        {
            this.baseService = baseService;
            this.teamService = teamService;
            this.employeeExperienceService = employeeExperienceService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new EmployeeExperienceFilterViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                Teams = await teamService.GetTeam(),
            };
            return View(model);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> GetEmployeeExperience(EmployeeExperienceFilterViewModel model)
        {
            var employeeExperience = await employeeExperienceService.GetAll(new GetEmployeeExperienceCriteria
            {
                GroupId = model.TeamId,
                EmpId = model.EmployeeId,
                Type = model.Type == "0" ? null : model.Type,
                Name = model.Keyword,
                FromDate = model.FromDate,
                ToDate = model.ToDate.AddDays(1)
            });
            return PartialView("Views/EmployeeExperience/_EmployeeExperienceGrid.cshtml", employeeExperience);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult Insert()
        {
            return PartialView();
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<CommandResult> Insert(EmployeeExperienceViewModel model)
        {
            var result = new CommandResult();
            if (model != null)
            {
                var criteria = new InsertEmployeeExperienceCriteria
                {
                    EmpId = model.EmployeeId,
                    DepartmentId = model.DepartmentId,
                    Type = model.Type,
                    Name = model.Name,
                    CreatedBy = CurrentUser.Id,
                    Organization = model.Organization,
                    StartDate = !model.Type.Equals(Certificate) ? model.StartDate : (DateTime?)null,
                    EndDate = !model.Type.Equals(Certificate) ? model.EndDate : (DateTime?)null,
                    ExamDate = model.Type.Equals(Certificate) ? model.StartDate : (DateTime?)null,
                    ExpirationDate = model.Type.Equals(Certificate) ? model.EndDate : (DateTime?)null
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
            }
            return result;
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Update(int id)
        {
            var employeeExperience = await employeeExperienceService.GetById(new GetEmployeeExperienceByIdCriteria { Id = id });
            var model = new EmployeeExperienceViewModel();
            if (employeeExperience != null)
            {
                model.Id = employeeExperience.Id;
                model.EmployeeId = employeeExperience.EmpId;
                model.Employee = employeeExperience.NickName + " (" + employeeExperience.FullName + ")";
                model.Department = employeeExperience.DepartmentName;
                model.DepartmentId = employeeExperience.DepartmentId;
                model.Type = employeeExperience.EmployeeExperienceType;
                model.Name = employeeExperience.EmployeeExperienceName;
                model.Organization = employeeExperience.Organization;
                model.ExamDate = employeeExperience.EmployeeExperienceType.Equals(Certificate) ?
                    employeeExperience.ExamDate : employeeExperience.StartDate;
                model.ExpirationDate = employeeExperience.EmployeeExperienceType.Equals(Certificate) ?
                    employeeExperience.ExpirationDate : employeeExperience.EndDate;
            }
            return PartialView(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<CommandResult> Update(EmployeeExperienceViewModel model)
        {
            var result = new CommandResult();
            if (model != null)
            {
                var criteria = new UpdateEmployeeExperienceCriteria
                {
                    Id = model.Id,
                    DepartmentId = model.DepartmentId,
                    Type = model.Type,
                    Name = model.Name,
                    ModifyBy = CurrentUser.Id,
                    Organization = model.Organization,
                    StartDate = !model.Type.Equals(Certificate) ? model.ExamDate : (DateTime?)null,
                    EndDate = !model.Type.Equals(Certificate) ? model.ExpirationDate : (DateTime?)null,
                    ExamDate = model.Type.Equals(Certificate) ? model.ExamDate : (DateTime?)null,
                    ExpirationDate = model.Type.Equals(Certificate) ? model.ExpirationDate : (DateTime?)null
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
            }
            return result;
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<CommandResult> Delete(int id)
        {
            var result = await baseService.ExecuteNonQueryAsync(new DeleteEmployeeExperienceCriteria
            {
                Id = id
            });
            return result;
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> GetEmployeeExperienceByEmp(int empId)
        {
            var employeeExperience = await employeeExperienceService.GetByEmpId(new GetEmployeeExperienceByEmpIdCriteria
            {
                EmpId = empId
            });
            return PartialView("Views/Employee/_ExperienceGrid.cshtml", employeeExperience);
        }
    }
}
