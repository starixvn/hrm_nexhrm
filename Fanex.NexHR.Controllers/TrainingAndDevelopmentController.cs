namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.TrainingAndDevelopment;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.TrainingAndDevelopment;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    public class TrainingAndDevelopmentController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;
        private readonly ITrainingAndDevelopmentService trainingAndDevelopmentService;
        private const string Certificate = "Certificates";
        public TrainingAndDevelopmentController(ISessionService sessionService,
            ITeamService teamService,
            IEmployeeService employeeService,
            ITrainingAndDevelopmentService trainingAndDevelopmentService,
            IBaseService baseService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.baseService = baseService;
            this.teamService = teamService;
            this.trainingAndDevelopmentService = trainingAndDevelopmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new TrainingAndDevelopmentFilterViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                Teams = await teamService.GetTeam(),
                Types = await trainingAndDevelopmentService.GetTrainingAndDevelopmentType()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetTrainingAndDevelopment(TrainingAndDevelopmentFilterViewModel model)
        {
            var trainingAndDevelopments = await trainingAndDevelopmentService.GetAll(new GetTrainingAndDevelopmentCriteria
            {
                GroupId = model.TeamId,
                EmpId = model.EmployeeId,
                TypeId = model.TypeId,
                Name = model.Name,
                FromDate = model.FromDate,
                ToDate = model.ToDate.AddDays(1)
            });
            return PartialView("Views/TrainingAndDevelopment/_TrainingAndDevelopmentGrid.cshtml", trainingAndDevelopments);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Types = await trainingAndDevelopmentService.GetTrainingAndDevelopmentType();
            return PartialView();
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Create(TrainingAndDevelopmentViewModel model)
        {
            var result = new CommandResult();
            if (model != null)
            {
                var criteria = new InsertTrainingAndDevelopmentCriteria
                {
                    EmpId = model.EmployeeId,
                    ConductedBy = model.ConductedBy,
                    TypeId = model.TypeId,
                    Name = model.Name,
                    CreatedBy = CurrentUser.Id,
                    Description = model.Description,
                    Organization = model.Organization,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ExamDate = model.ExamDate,
                    ExpirationDate = model.ExpirationDate
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
            }
            return result;
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var trainingAndDevelopment = await trainingAndDevelopmentService.GetById(new GetTrainingAndDevelopmentByIdCriteria { Id = id });
            var model = new TrainingAndDevelopmentViewModel();
            if (trainingAndDevelopment != null)
            {
                model.Id = trainingAndDevelopment.Id;
                model.EmployeeId = trainingAndDevelopment.EmpId;
                model.Employee = trainingAndDevelopment.NickName + " (" + trainingAndDevelopment.FullName + ")";
                model.ConductedBy = trainingAndDevelopment.ConductedBy;
                model.TypeId = trainingAndDevelopment.TypeId;
                model.Name = trainingAndDevelopment.Name;
                model.Organization = trainingAndDevelopment.Organization;
                model.Description = trainingAndDevelopment.Description;
                model.ExamDate = trainingAndDevelopment.TypeName.Equals(Certificate) ?
                    trainingAndDevelopment.ExamDate : trainingAndDevelopment.StartDate;
                model.ExpirationDate = trainingAndDevelopment.TypeName.Equals(Certificate) ?
                    trainingAndDevelopment.ExpirationDate : trainingAndDevelopment.EndDate;
            }
            ViewBag.Types = await trainingAndDevelopmentService.GetTrainingAndDevelopmentType();
            return PartialView(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Update(TrainingAndDevelopmentViewModel model)
        {
            var result = new CommandResult();
            if (model != null)
            {
                var criteria = new UpdateTrainingAndDevelopmentCriteria
                {
                    Id = model.Id,
                    ConductedBy = model.ConductedBy,
                    TypeId = model.TypeId,
                    Name = model.Name,
                    ModifyBy = CurrentUser.Id,
                    Organization = model.Organization,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ExamDate = model.ExamDate,
                    ExpirationDate = model.ExpirationDate
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
            }
            return result;
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Delete(int id)
        {
            var result = await baseService.ExecuteNonQueryAsync(new DeleteTrainingAndDevelopmentCriteria
            {
                Id = id
            });
            return result;
        }
    }
}
