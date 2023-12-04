namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Services.Interfaces;
    using Fanex.NexHR.Controllers.Attributes;
    using ViewModels.Applicant;
    using Models.Applicant;
    using Criteria.Applicant;
    using System;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Core.Enums;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Fanex.NexHR.Criteria.Employee;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Text;
    using Fanex.NexHR.Criteria.Position;

    public class ApplicantsController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IWebHostEnvironment environment;
        private readonly IApplicantService applicantService;
        private readonly IPositionService positionService;
        private readonly IEmployeeService employeeService;

        public ApplicantsController(ISessionService sessionService,
            IBaseService baseService,
            IWebHostEnvironment environment,
            IApplicantService applicantService,
            IPositionService positionService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.baseService = baseService;
            this.environment = environment;
            this.applicantService = applicantService;
            this.positionService = positionService;
            this.employeeService = employeeService;
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Index()
        {
            var model = new ApplicantViewModel
            {
                Positions = await GetPositions(),
                StatusList = GetStatusList()
            };

            return View(model);
        }

        private async Task<List<Position>> GetPositions()
        {
            var criteria = new GetAllPositionCriteria();
            var position = await positionService.GetAll(criteria);
            return position.ToList(); 
        }

        private static List<SelectListItem> GetStatusList()
        {
            var statusList = new List<SelectListItem>();

            foreach (CVStatus status in Enum.GetValues(typeof(CVStatus)))
            {
                var item = new SelectListItem
                {
                    Text = status.ToString(),
                    Value = ((int)status).ToString()
                };

                statusList.Add(item);
            }

            return statusList;
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Index(ApplicantViewModel model)
        {
            IEnumerable<Applicant> result = new List<Applicant>();
            if (model != null)
            {
                var criteria = new GetAllApplicantCriteria
                {
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    Status = model.Status,
                    PositionId = model.PositionId,
                    FullName = model.FullName
                };

                result = await applicantService.GetAll(criteria);
                foreach (var applicant in result)
                {
                    applicant.SourceName = applicant.Source != 0 ? GetName<CVSource>(applicant.Source) : string.Empty;
                    applicant.ResultName1 = applicant.Result1 != 0 ? GetName<ResultInterview>(applicant.Result1) : string.Empty;
                    applicant.ResultName2 = applicant.Result2 != 0 ? GetName<ResultInterview>(applicant.Result2) : string.Empty;
                }
            }

            return PartialView("_ListApplicant", result);
        }

        private static string GetName<T>(int id)
        {
            if (Enum.IsDefined(typeof(T), id))
            {
                T enumValue = (T)(object)id;
                return enumValue.ToString();
            }
            else
            {
                return $"Invalid {typeof(T).Name}";
            }
        }

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
            var criteria = new GetAllPositionCriteria();
            var model = new CreateApplicantViewModel
            {
                Positions = await GetPositions(),
                StatusList = GetStatusList(),
                Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.NickName))
            };

            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create(CreateApplicantViewModel model)
        {
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    var cv = UploadFileAsync(model.FileUpload).Result;
                    var criteria = new InsertApplicantCriteria
                    {
                        FullName = model.FullName,
                        PositionId = model.PositionId,
                        Phone = model.Phone,
                        Email = model.Email,
                        ApplyDate = model.ApplyDate,
                        YearsOfExperience = model.YearsOfExperience,
                        DOB = model.DOB,
                        ProfessionalLevel = model.ProfessionalLevel,
                        ProfessionalMajor = model.ProfessionalMajor,
                        Status = model.Status,
                        Resume = cv,
                        InterviewDate1 = model.InterviewDate1,
                        Result1 = model.Result1,
                        InterviewDate2 = model.InterviewDate2,
                        Result2 = model.Result2,
                        JoinDate = model.JoinDate,
                        Source = model.Source,
                        RecruiterId = model.RecruiterId,
                        EducationBackground = model.EducationBackground,
                        InterviewerComment = model.InterviewerComment,
                        Grade = model.Grade,
                        Note = model.Note,
                        InterviewerIds = model.InterviewerIds != null ? string.Join(",", model.InterviewerIds) : null,
                        ExpectedSalary = model.ExpectedSalary,
                        OfferSalary = model.OfferSalary,
                        FinalComment = model.FinalComment,
                        CreatedBy = CurrentUser.Id
                    };

                    CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

                    if (result.IsSuccessful)
                    {
                        ViewBag.Inserted = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }

                model.Positions = await GetPositions();
                model.StatusList = GetStatusList();
            }

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(int id)
        {
            var criteria = new GetApplicantByIdCriteria
            {
                ApplicantId = id
            };

            var employees = await employeeService.Query(new GetEmployeeCriteria
            {
                Query = string.Empty,
                IsDelete = false,
                IsDisable = false,
                PageSize = int.MaxValue,
                PageNumber = 1
            });

            EditApplicantViewModel applicant = await applicantService.GetById(criteria);
            if (applicant != null)
            {
                applicant.Positions = await GetPositions();
                applicant.StatusList = GetStatusList();
                applicant.Recruiter = applicant.NickNameRecruiter + (string.IsNullOrWhiteSpace(applicant.FullNameRecruiter) ? string.Empty : " (" + applicant.FullNameRecruiter + ")");
                applicant.InterviewerIds = !string.IsNullOrEmpty(applicant.InterviewerIdString) ? applicant.InterviewerIdString.Split(',').Select(int.Parse).ToArray() : null;
                applicant.Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.NickName));
            }

            return View(applicant);
        }

        private void CreateOrOpenDirectory(string dir)
        {
            string[] filePath = { environment.WebRootPath, dir };
            var path = Path.Combine(filePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(EditApplicantViewModel model)
        {
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    var cv = UploadFileAsync(model.FileUpload).Result;

                    var criteria = new UpdateApplicantCriteria
                    {
                        Id = model.Id,
                        FullName = model.FullName,
                        PositionId = model.PositionId,
                        Phone = model.Phone,
                        Email = model.Email,
                        ApplyDate = model.ApplyDate,
                        YearsOfExperience = model.YearsOfExperience,
                        DOB = model.DOB,
                        ProfessionalLevel = model.ProfessionalLevel,
                        ProfessionalMajor = model.ProfessionalMajor,
                        Status = model.Status,
                        Resume = cv,
                        InterviewDate1 = model.InterviewDate1,
                        Result1 = model.Result1,
                        InterviewDate2 = model.InterviewDate2,
                        Result2 = model.Result2,
                        JoinDate = model.JoinDate,
                        Source = model.Source,
                        RecruiterId = model.RecruiterId,
                        EducationBackground = model.EducationBackground,
                        InterviewerComment = model.InterviewerComment,
                        Grade = model.Grade,
                        Note = model.Note,
                        InterviewerIds = model.InterviewerIds != null ? string.Join(",", model.InterviewerIds) : null,
                        ExpectedSalary = model.ExpectedSalary,
                        OfferSalary = model.OfferSalary,
                        FinalComment = model.FinalComment
                    };

                    CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

                    if (result.IsSuccessful)
                    {
                        ViewBag.Inserted = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }

                model.Positions = await GetPositions();
                model.StatusList = GetStatusList();
            }

            return View(model);
        }

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            var attachedFilePath = new StringBuilder();

            if (file == null)
            {
                return attachedFilePath.ToString();
            }

            var dir = $"file/uploads/cv";
            CreateOrOpenDirectory(dir);

            string savedFilePath = await SaveFileAsync(file, dir);
            attachedFilePath.Append("/" + savedFilePath);

            return file.FileName;
        }

        private async Task<string> SaveFileAsync(IFormFile file, string dir)
        {
            if (file == null)
            {
                return null;
            }

            var fileName = Path.GetFileName(file.FileName);
            var pathAttach = new StringBuilder($"{dir}/{fileName}");

            //fileName = ChangeNameForDuplicatedFile(pathAttach.ToString(), fileName);

            string[] filePath = { environment.WebRootPath, pathAttach.ToString() };
            var path = Path.Combine(filePath);

            var stream = new FileStream(path, FileMode.Create);

            await file.CopyToAsync(stream);

            return pathAttach.ToString();
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteApplicantCriteria = new DeleteApplicantCriteria { Id = id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(deleteApplicantCriteria);

            return Json(result);
        }
    }
}