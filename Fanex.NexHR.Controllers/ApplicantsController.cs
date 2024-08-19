namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Criteria.Applicant;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Position;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Applicant;
    using Services.Interfaces;
    using ViewModels.Applicant;

    public class ApplicantsController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IWebHostEnvironment environment;
        private readonly IApplicantService applicantService;
        private readonly IPositionService positionService;
        private readonly ICryptoService cryptoService;
        private readonly IPromotionService promotionService;

        private const string UploadsFilePath = "file/uploads/cv";

        public ApplicantsController(ISessionService sessionService,
            IBaseService baseService,
            IWebHostEnvironment environment,
            IApplicantService applicantService,
            IPositionService positionService,
            ICryptoService cryptoService,
            IPromotionService promotionService) : base(sessionService)
        {
            this.baseService = baseService;
            this.environment = environment;
            this.applicantService = applicantService;
            this.positionService = positionService;
            this.cryptoService = cryptoService;
            this.promotionService = promotionService;
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Index()
        {
            var model = new ApplicantViewModel
            {
                Positions = await GetPositions(),
                StatusList = await GetCVStatus()
            };

            return View(model);
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
                    FullName = model.FullName,
                    PageNumber = model.PageNumber,
                    PageSize = model.PageSize
                };

                result = await applicantService.GetAll(criteria);
                ViewBag.TotalRow = criteria.TotalRow;
                ViewBag.PageNumber = model.PageNumber;
                ViewBag.PageSize = model.PageSize;
            }

            return PartialView("_ListApplicant", result);
        }

        private async Task<List<Position>> GetPositions()
        {
            var criteria = new GetAllPositionCriteria();
            var positions = await positionService.GetAll(criteria);
            var activePositions = positions.Where(p => p.IsActive).ToList();
            return activePositions;
        }

        private async Task<List<Source>> GetCVStatus()
        {
            var criteria = new GetCVStatusApplicantCriteria();
            var statusList = await applicantService.GetCVStatusApplicant(criteria);
            return statusList.ToList();
        }

        private async Task<List<Source>> GetResult()
        {
            var criteria = new GetResultApplicantCriteria();
            var resultList = await applicantService.GetResultApplicant(criteria);
            return resultList.ToList();
        }

        private async Task<List<Source>> GetSource()
        {
            var criteria = new GetSourceApplicantCriteria();
            var sourceList = await applicantService.GetSourceApplicant(criteria);
            return sourceList.ToList();
        }

        private async Task<List<Source>> GetProLevel()
        {
            var criteria = new GetProLevelApplicantCriteria();
            var proLevelList = await applicantService.GetProLevelApplicant(criteria);
            return proLevelList.ToList();
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Detail(int id)
        {
            var criteria = new GetApplicantByIdCriteria { ApplicantId = id };
            var applicant = await applicantService.GetById(criteria);
            var positions = await GetPositions();
            var statusList = await GetCVStatus();
            var proLevelList = await GetProLevel();
            var resultList = await GetResult();
            var gradeList = await promotionService.GetJobGrade();
            var sourceList = await GetSource();
            var model = new DetailApplicantViewModel();
            if (applicant != null)
            {
                model.FullName = applicant.FullName;
                model.Phone = applicant.Phone;
                model.DOB = applicant.DOB;
                model.Email = applicant.Email;
                model.YearsOfExperience = applicant.YearsOfExperience;
                model.ApplyDate = applicant.ApplyDate;
                model.ProfessionalMajor = applicant.ProfessionalMajor;
                model.InterviewDate1 = applicant.InterviewDate1;
                model.InterviewDate2 = applicant.InterviewDate2;
                model.JoinDate = applicant.JoinDate;
                model.EducationBackground = applicant.EducationBackground;
                model.InterviewerComment = applicant.InterviewerComment;
                model.Note = applicant.Note;
                model.FinalComment = DecryptValue(applicant.FinalComment);
                model.OfferSalary = DecryptValue(applicant.OfferSalary);
                model.ExpectedSalary = DecryptValue(applicant.ExpectedSalary);
                model.Recruiter = applicant.NickNameRecruiter + (string.IsNullOrWhiteSpace(applicant.FullNameRecruiter) ? string.Empty : " (" + applicant.FullNameRecruiter + ")");
                model.Position = positions.Find(p => p.PositionId == applicant.PositionId).PositionName;
                model.Status = applicant.Status != null ? statusList.Find(p => p.Value == applicant.Status.ToString()).Name : null;
                model.ProfessionalLevel = applicant.ProfessionalLevel != null
                ? proLevelList.Find(p => p.Value == applicant.ProfessionalLevel.ToString()).Name : null;
                model.Result1 = applicant.Result1 != null ? resultList.Find(p => p.Value == applicant.Result1.ToString()).Name : null;
                model.Result2 = applicant.Result2 != null ? resultList.Find(p => p.Value == applicant.Result2.ToString()).Name : null;
                model.Source = applicant.Source != null ? sourceList.Find(p => p.Value == applicant.Source.ToString()).Name : null;
                model.Grade = applicant.Grade != null ? gradeList.FirstOrDefault(p => p.Id == applicant.Grade)?.Name : null;
                model.Interviewer = await GetInterviewerNames(applicant.InterviewerIdString);
                model.Resume = applicant.Resume;
            }
            ViewBag.IsManager = IsCurrentUserManager();

            return View(model);
        }

        private bool IsCurrentUserManager()
        {
            return CurrentUser.CodeOfTeam.Equals("MGT") || CurrentUser.IsLeaderHR;
        }

        private string DecryptValue(string encryptedValue)
        {
            return encryptedValue != null ? cryptoService.RsaDecrypt(encryptedValue) : null;
        }

        private async Task<string> GetInterviewerNames(string interviewerIdString)
        {
            if (interviewerIdString == null)
            {
                return null;
            }
            var interviews = await baseService.GetDataAsync<Interviewer>(new GetInterviewerCriteria());
            var interviewerIds = interviewerIdString.Split(',').Select(int.Parse).ToList();
            var employeeNicknames = interviewerIds != null
                ? interviews.Where(emp => interviewerIds.Contains(emp.Id)).Select(emp => emp.NickName).ToList()
                : new List<string>();

            return string.Join(", ", employeeNicknames);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create()
        {
            var interviews = await baseService.GetDataAsync<Interviewer>(new GetInterviewerCriteria());
            var model = new CreateApplicantViewModel
            {
                Positions = await GetPositions(),
                StatusList = await GetCVStatus(),
                ProLevelList = await GetProLevel(),
                ResultList = await GetResult(),
                GradeList = await promotionService.GetJobGrade(),
                SourceList = await GetSource(),
                Employees = new SelectList(interviews, nameof(Employee.Id), nameof(Employee.NickName))
            };

            ViewBag.IsManager = IsCurrentUserManager();

            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create(CreateApplicantViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var cv = await UploadFileAsync(model.FileUpload);
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
                    ExpectedSalary = model.ExpectedSalary != null
                                    ? cryptoService.RsaEncrypt(model.ExpectedSalary) : null,
                    OfferSalary = model.OfferSalary != null
                                    ? cryptoService.RsaEncrypt(model.OfferSalary) : null,
                    FinalComment = string.IsNullOrWhiteSpace(model.FinalComment) ? string.Empty : cryptoService.RsaEncrypt(model.FinalComment),
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

                ViewBag.IsManager = IsCurrentUserManager();
                model.Positions = await GetPositions();
                model.StatusList = await GetCVStatus();
                model.ProLevelList = await GetProLevel();
                model.ResultList = await GetResult();
                model.GradeList = await promotionService.GetJobGrade();
                model.SourceList = await GetSource();
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

            var interviews = await baseService.GetDataAsync<Interviewer>(new GetInterviewerCriteria());

            EditApplicantViewModel applicant = await applicantService.GetById(criteria);
            if (applicant != null)
            {
                applicant.OfferSalary = DecryptValue(applicant.OfferSalary);
                applicant.ExpectedSalary = DecryptValue(applicant.ExpectedSalary);
                applicant.FinalComment = DecryptValue(applicant.FinalComment);
                applicant.Positions = await GetPositions();
                applicant.Recruiter = applicant.NickNameRecruiter + (string.IsNullOrWhiteSpace(applicant.FullNameRecruiter) ? string.Empty : " (" + applicant.FullNameRecruiter + ")");
                applicant.InterviewerIds = !string.IsNullOrEmpty(applicant.InterviewerIdString) ? applicant.InterviewerIdString.Split(',').Select(int.Parse).ToArray() : null;
                applicant.Employees = new SelectList(interviews, nameof(Employee.Id), nameof(Employee.NickName));
                applicant.StatusList = await GetCVStatus();
                applicant.ProLevelList = await GetProLevel();
                applicant.ResultList = await GetResult();
                applicant.GradeList = await promotionService.GetJobGrade();
                applicant.SourceList = await GetSource();
            }

            ViewBag.IsManager = IsCurrentUserManager();

            return View(applicant);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(EditApplicantViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var cv = model.FileUpload != null ? await UploadFileAsync(model.FileUpload) : model.Resume;
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
                    ExpectedSalary = model.ExpectedSalary != null ? cryptoService.RsaEncrypt(model.ExpectedSalary) : null,
                    OfferSalary = model.OfferSalary != null ? cryptoService.RsaEncrypt(model.OfferSalary) : null,
                    FinalComment = string.IsNullOrWhiteSpace(model.FinalComment) ? string.Empty : cryptoService.RsaEncrypt(model.FinalComment)
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

                ViewBag.IsManager = IsCurrentUserManager();
                model.Positions = await GetPositions();
                model.StatusList = await GetCVStatus();
                model.ProLevelList = await GetProLevel();
                model.ResultList = await GetResult();
                model.GradeList = await promotionService.GetJobGrade();
                model.SourceList = await GetSource();
            }

            return View(model);
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

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null)
            {
                return string.Empty;
            }
            CreateOrOpenDirectory();
            string savedFilePath = await SaveFile(file);

            return savedFilePath;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }
            var fileName = Path.GetFileName(file.FileName);
            var pathAttachFinal = CheckDuplicatedFileName(fileName);
            pathAttachFinal = RemoveAccents(pathAttachFinal);
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

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> UpdateOnTop(int id)
        {
            var criteria = new UpdateOnTopApplicantCriteria
            {
                Id = id
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(commandResult);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Delete(int id, string fileName)
        {
            var deleteApplicantCriteria = new DeleteApplicantCriteria { Id = id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(deleteApplicantCriteria);

            if (result.IsSuccessful && !string.IsNullOrEmpty(fileName))
            {
                string filePath = Path.Combine(environment.WebRootPath, UploadsFilePath, fileName);
                System.IO.File.Delete(filePath);

                DeleteFile(fileName);
            }

            return Json(result);
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