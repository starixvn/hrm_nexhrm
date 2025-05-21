namespace Fanex.NexHR.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.TrainingAndDevelopment;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.TrainingAndDevelopment;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class LearningAndDevelopmentController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;
        private readonly ITrainingAndDevelopmentService trainingAndDevelopmentService;
        private readonly IWebHostEnvironment environment;
        private const string Certificate = "Certificates";
        private const string UploadsFilePath = "common_files/certificates";

        public LearningAndDevelopmentController(ISessionService sessionService,
            ITeamService teamService,
            ITrainingAndDevelopmentService trainingAndDevelopmentService,
            IWebHostEnvironment environment,
            IBaseService baseService) : base(sessionService)
        {
            this.baseService = baseService;
            this.teamService = teamService;
            this.environment = environment;
            this.trainingAndDevelopmentService = trainingAndDevelopmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new TrainingAndDevelopmentFilterViewModel
            {
                ToDate = DateTimeOffset.Now.DateTime,
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
                var certificate = await UploadFileAsync(model.FileUpload, null);
                var command = new InsertTrainingAndDevelopmentCommand
                {
                    EmpId = model.EmployeeId,
                    ConductedBy = model.ConductedBy,
                    TypeId = model.TypeId,
                    Name = model.Name,
                    Publish = model.Publish,
                    CreatedBy = CurrentUser.Id,
                    Description = model.Description,
                    Organization = model.Organization,
                    Certificate = certificate,
                    StartDate = model.TypeName.Equals(Certificate) ? (DateTime?)null : model.FromDate,
                    EndDate = model.TypeName.Equals(Certificate) ? (DateTime?)null : model.ToDate,
                    ExamDate = !model.TypeName.Equals(Certificate) ? (DateTime?)null : model.FromDate,
                    ExpirationDate = !model.TypeName.Equals(Certificate) ? (DateTime?)null : model.ToDate
                };
                result = await baseService.ExecuteNonQueryAsync(command);
                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return Json(new { isSuccessful = result.IsSuccessful, error = ModelState.Values.SelectMany(v => v.Errors) });
        }

        private async Task<string> UploadFileAsync(IFormFile file, string certificate)
        {
            if (file == null)
            {
                return string.Empty;
            }
            CreateOrOpenDirectory();
            string savedFilePath = await SaveFile(file);
            if (!string.IsNullOrEmpty(certificate))
            {
                DeleteFile(certificate);
            }

            return savedFilePath;
        }

        private void CreateOrOpenDirectory()
        {
            string[] filePath = { environment.WebRootPath, UploadsFilePath };
            var path = Path.Combine(filePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }
            var fileName = Path.GetFileName(file.FileName);
            var pathAttachFinal = RemoveAccents(fileName);
            pathAttachFinal = CheckDuplicatedFileName(pathAttachFinal);
            var path = Path.Combine(environment.WebRootPath, UploadsFilePath, pathAttachFinal);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return pathAttachFinal;
        }

        private string RemoveAccents(string fileName)
        {
            return Regex.Replace(fileName.Normalize(NormalizationForm.FormD), @"[\p{Mn}]", string.Empty);
        }

        private string CheckDuplicatedFileName(string fileName)
        {
            var pathAttach = $"{UploadsFilePath}/{fileName}";
            var filePath = Path.Combine(environment.WebRootPath, pathAttach);

            if (System.IO.File.Exists(filePath))
            {
                var baseFileName = Path.GetFileNameWithoutExtension(pathAttach);
                var fileExtension = Path.GetExtension(pathAttach);
                var newFileName = $"{baseFileName}({DateTimeOffset.Now.ToString("yyyy-MM-dd-HHmmss")}){fileExtension}";

                return newFileName;
            }

            return fileName;
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
                model.Certificate = trainingAndDevelopment.Certificate;
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
                var certificate = model.FileUpload != null ? await UploadFileAsync(model.FileUpload, model.Certificate) : model.Certificate;
                var command = new UpdateTrainingAndDevelopmentCommand
                {
                    Id = model.Id,
                    ConductedBy = model.ConductedBy,
                    TypeId = model.TypeId,
                    Name = model.Name,
                    Publish = model.Publish,
                    ModifyBy = CurrentUser.Id,
                    Organization = model.Organization,
                    Description = model.Description,
                    Certificate = certificate,
                    StartDate = model.TypeName.Equals(Certificate) ? (DateTime?)null : model.FromDate,
                    EndDate = model.TypeName.Equals(Certificate) ? (DateTime?)null : model.ToDate,
                    ExamDate = !model.TypeName.Equals(Certificate) ? (DateTime?)null : model.FromDate,
                    ExpirationDate = !model.TypeName.Equals(Certificate) ? (DateTime?)null : model.ToDate,
                };
                result = await baseService.ExecuteNonQueryAsync(command);
                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return Json(new { isSuccessful = result.IsSuccessful, error = ModelState.Values.SelectMany(v => v.Errors) });
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Delete(int id, string fileName)
        {
            var result = await baseService.ExecuteNonQueryAsync(new DeleteTrainingAndDevelopmentCommand
            {
                Id = id,
                DeletedBy = CurrentUser.Id
            });
            if (result.IsSuccessful && !string.IsNullOrEmpty(fileName))
            {
                string filePath = Path.Combine(environment.WebRootPath, UploadsFilePath, fileName);
                System.IO.File.Delete(filePath);

                DeleteFile(fileName);
            }

            return result;
        }

        private void DeleteFile(string fileName)
        {
            string filePath = Path.Combine(environment.WebRootPath, UploadsFilePath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
