namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.TrainingAndDevelopment;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.TrainingAndDevelopment;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class LearningAndDevelopmentController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;
        private readonly ITrainingAndDevelopmentService trainingAndDevelopmentService;
        private const string Certificate = "Certificates";
        public LearningAndDevelopmentController(ISessionService sessionService,
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
                CurrentUserId = CurrentUser.Id,
                ToDate = model.ToDate.AddDays(1)
            }, CurrentUser);
            return PartialView("Views/LearningAndDevelopment/_TrainingAndDevelopmentGrid.cshtml", trainingAndDevelopments);
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
        public async Task<IActionResult> Create(TrainingAndDevelopmentCreateViewModel model)
        {
            var result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertTrainingAndDevelopmentCriteria
                {
                    EmpId = model.EmployeeId,
                    ConductedBy = model.ConductedBy,
                    TypeId = model.TypeId,
                    Name = model.Name,
                    Publish = model.Publish,
                    CreatedBy = CurrentUser.Id,
                    Description = model.Description,
                    Organization = model.Organization,
                    StartDate = model.TypeName.Equals(Certificate) ? (DateTime?)null : model.FromDate,
                    EndDate = model.TypeName.Equals(Certificate) ? (DateTime?)null : model.ToDate,
                    ExamDate = !model.TypeName.Equals(Certificate) ? (DateTime?)null : model.FromDate,
                    ExpirationDate = !model.TypeName.Equals(Certificate) ? (DateTime?)null : model.ToDate
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return Json(new { isSuccessful = result.IsSuccessful, error = ModelState.Values.SelectMany(v => v.Errors) });
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var trainingAndDevelopment = await trainingAndDevelopmentService.GetById(new GetTrainingAndDevelopmentByIdCriteria { Id = id });
            var model = new TrainingAndDevelopmentUpdateViewModel();
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
                model.Publish = trainingAndDevelopment.Publish;
                model.FromDate = trainingAndDevelopment.TypeName.Equals(Certificate) ?
                    trainingAndDevelopment.ExamDate : trainingAndDevelopment.StartDate;
                model.ToDate = trainingAndDevelopment.TypeName.Equals(Certificate) ?
                    trainingAndDevelopment.ExpirationDate : trainingAndDevelopment.EndDate;
            }
            ViewBag.Types = await trainingAndDevelopmentService.GetTrainingAndDevelopmentType();
            return PartialView(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Update(TrainingAndDevelopmentUpdateViewModel model)
        {
            var result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateTrainingAndDevelopmentCriteria
                {
                    Id = model.Id,
                    ConductedBy = model.ConductedBy,
                    TypeId = model.TypeId,
                    Name = model.Name,
                    Publish = model.Publish,
                    ModifyBy = CurrentUser.Id,
                    Organization = model.Organization,
                    Description = model.Description,
                    StartDate = model.TypeName.Equals(Certificate) ? (DateTime?)null : model.FromDate,
                    EndDate = model.TypeName.Equals(Certificate) ? (DateTime?)null : model.ToDate,
                    ExamDate = !model.TypeName.Equals(Certificate) ? (DateTime?)null : model.FromDate,
                    ExpirationDate = !model.TypeName.Equals(Certificate) ? (DateTime?)null : model.ToDate
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return Json(new { isSuccessful = result.IsSuccessful, error = ModelState.Values.SelectMany(v => v.Errors) });
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Delete(int id)
        {
            var result = await baseService.ExecuteNonQueryAsync(new DeleteTrainingAndDevelopmentCriteria
            {
                Id = id,
                DeletedBy = CurrentUser.Id
            });
            return result;
        }
    }
}
