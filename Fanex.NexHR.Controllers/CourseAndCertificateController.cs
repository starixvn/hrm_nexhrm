namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.CourseAndCertificate;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.CourseAndCertificate;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class CourseAndCertificateController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;
        private readonly ICourseAndCertificateService courseAndCertificateService;
        private const string Certificate = "Certificate";
        public CourseAndCertificateController(ISessionService sessionService,
            ITeamService teamService,
            IEmployeeService employeeService,
            ICourseAndCertificateService courseAndCertificateService,
            IBaseService baseService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.baseService = baseService;
            this.teamService = teamService;
            this.courseAndCertificateService = courseAndCertificateService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new CourseAndCertificateFilterViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                Teams = await teamService.GetTeam()
            };
            var userRoles = employeeService.UserRoles(CurrentUser);
            ViewBag.HasHRRole = userRoles.Any(item => item == "HR");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetCourseAndCertificate(CourseAndCertificateFilterViewModel model)
        {
            var courseAndCertificate = await courseAndCertificateService.GetAll(new GetCourseAndCertificateCriteria
            {
                GroupId = model.TeamId,
                EmpId = model.EmployeeId,
                Type = model.Type,
                Name = model.Name,
                FromDate = model.FromDate,
                ToDate = model.ToDate.AddDays(1)
            });
            var userRoles = employeeService.UserRoles(CurrentUser);
            ViewBag.HasHRRole = userRoles.Any(item => item == "HR");
            return PartialView("Views/CourseAndCertificate/_CourseAndCertificateGrid.cshtml", courseAndCertificate);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpGet]
        public IActionResult Insert()
        {
            return PartialView();
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Insert(CourseAndCertificateViewModel model)
        {
            var result = new CommandResult();
            if (model != null)
            {
                var criteria = new InsertCourseAndCertificateCriteria
                {
                    EmpId = model.EmployeeId,
                    ConductedBy = model.ConductedBy,
                    Type = model.Type,
                    Name = model.Name,
                    CreatedBy = CurrentUser.Id,
                    Description = model.Description,
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

        [HrmAuthorize(UserRoles = "HR")]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var courseAndCertificate = await courseAndCertificateService.GetById(new GetCourseAndCertificateByIdCriteria { Id = id });
            var model = new CourseAndCertificateViewModel();
            if (courseAndCertificate != null)
            {
                model.Id = courseAndCertificate.Id;
                model.EmployeeId = courseAndCertificate.EmpId;
                model.Employee = courseAndCertificate.NickName + " (" + courseAndCertificate.FullName + ")";
                model.ConductedBy = courseAndCertificate.ConductedBy;
                model.Type = courseAndCertificate.Type;
                model.Name = courseAndCertificate.Name;
                model.Organization = courseAndCertificate.Organization;
                model.Description = courseAndCertificate.Description;
                model.ExamDate = courseAndCertificate.Type.Equals(Certificate) ?
                    courseAndCertificate.ExamDate : courseAndCertificate.StartDate;
                model.ExpirationDate = courseAndCertificate.Type.Equals(Certificate) ?
                    courseAndCertificate.ExpirationDate : courseAndCertificate.EndDate;
            }
            return PartialView(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Update(CourseAndCertificateViewModel model)
        {
            var result = new CommandResult();
            if (model != null)
            {
                var criteria = new UpdateCourseAndCertificateCriteria
                {
                    Id = model.Id,
                    ConductedBy = model.ConductedBy,
                    Type = model.Type,
                    Name = model.Name,
                    ModifyBy = CurrentUser.Id,
                    Organization = model.Organization,
                    Description = model.Description,
                    StartDate = !model.Type.Equals(Certificate) ? model.ExamDate : (DateTime?)null,
                    EndDate = !model.Type.Equals(Certificate) ? model.ExpirationDate : (DateTime?)null,
                    ExamDate = model.Type.Equals(Certificate) ? model.ExamDate : (DateTime?)null,
                    ExpirationDate = model.Type.Equals(Certificate) ? model.ExpirationDate : (DateTime?)null
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
            }
            return result;
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Delete(int id)
        {
            var result = await baseService.ExecuteNonQueryAsync(new DeleteCourseAndCertificateCriteria
            {
                Id = id
            });
            return result;
        }
    }
}
