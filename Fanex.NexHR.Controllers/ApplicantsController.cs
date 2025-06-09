namespace Fanex.NexHR.Controllers
{
    using Criteria.Applicant;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Position;
    using Fanex.NexHR.Models;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Applicant;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using ViewModels.Applicant;
    using Fanex.NexHR.Core;

    public class ApplicantsController(ISessionService sessionService,
            IBaseService baseService,
            IWebHostEnvironment environment,
            IApplicantService applicantService,
            IPositionService positionService,
            ICryptoService cryptoService1,
            IPromotionService promotionService) : BaseController(sessionService)
    {
        private const string UploadsFilePath = "common_files/cv";

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
            if (applicant == null)
            {
                return View(new DetailApplicantViewModel());
            }

            var model = await MapApplicantToViewModel(applicant);
            ViewBag.IsManager = IsCurrentUserManager();

            return View(model);
        }

        private async Task<DetailApplicantViewModel> MapApplicantToViewModel(EditApplicantViewModel applicant)
        {
            var positions = await GetPositions();
            var statusList = await GetCVStatus();
            var proLevelList = await GetProLevel();
            var resultList = await GetResult();
            var gradeList = await promotionService.GetJobGrade();
            var managementGradeList = await promotionService.GetManagementGrade();
            var sourceList = await GetSource();

            return new DetailApplicantViewModel
            {
                FullName = applicant.FullName,
                Phone = applicant.Phone,
                DOB = applicant.DOB,
                Email = applicant.Email,
                YearsOfExperience = applicant.YearsOfExperience,
                ApplyDate = applicant.ApplyDate,
                ProfessionalMajor = applicant.ProfessionalMajor,
                InterviewDate1 = applicant.InterviewDate1,
                InterviewDate2 = applicant.InterviewDate2,
                JoinDate = applicant.JoinDate,
                EducationBackground = applicant.EducationBackground,
                InterviewerComment = applicant.InterviewerComment,
                Note = applicant.Note,
                FinalComment = DecryptValue(applicant.FinalComment),
                OfferSalary = DecryptValue(applicant.OfferSalary),
                ExpectedSalary = DecryptValue(applicant.ExpectedSalary),
                Recruiter = FormatRecruiter(applicant.NickNameRecruiter, applicant.FullNameRecruiter),
                Position = GetPositionName(positions, applicant.PositionId),
                Status = GetSourceData(statusList, applicant.Status),
                ProfessionalLevel = GetSourceData(proLevelList, applicant.ProfessionalLevel),
                Result1 = GetSourceData(resultList, applicant.Result1),
                Result2 = GetSourceData(resultList, applicant.Result2),
                Source = GetSourceData(sourceList, applicant.Source),
                Grade = GetGradeName(gradeList, applicant.Grade),
                ManagementGrade = GetManagementGradeName(managementGradeList, applicant.ManagementGrade),
                Interviewer = await GetInterviewerNames(applicant.InterviewerIdString),
                Resume = applicant.Resume
            };
        }

        private string FormatRecruiter(string nickName, string fullName)
        {
            return string.IsNullOrWhiteSpace(fullName) ? nickName : $"{nickName} ({fullName})";
        }

        private string GetPositionName(List<Position> positions, int? positionId)
        {
            return positions.Find(p => p.PositionId == positionId)?.PositionName;
        }

        private string GetSourceData(List<Source> sourceList, int? value)
        {
            return sourceList.Find(p => p.Value == value?.ToString())?.Name;
        }

        private string GetGradeName(IList<JobGrade> gradeList, int? grade)
        {
            return gradeList.ToList().Find(p => p.Id == grade)?.Name;
        }

        private string GetManagementGradeName(IList<ManagementGrade> managementGradeList, int? managementGrade)
        {
            return managementGradeList.ToList().Find(p => p.Id == managementGrade)?.Name;
        }

        private bool IsCurrentUserManager()
        {
            return CurrentUser.CodeOfTeam.Equals("COR") || CurrentUser.IsLeaderHR;
        }

        private string DecryptValue(string encryptedValue)
        {
            return encryptedValue != null ? cryptoService1.RsaDecrypt(encryptedValue) : null;
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
                ManagementGradeList = await promotionService.GetManagementGrade(),
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
                var command = new InsertApplicantCommand
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
                    ManagementGrade = model.ManagementGrade,
                    Note = model.Note,
                    InterviewerIds = model.InterviewerIds != null ? string.Join(",", model.InterviewerIds) : null,
                    ExpectedSalary = model.ExpectedSalary != null
                                    ? cryptoService1.RsaEncrypt(model.ExpectedSalary) : null,
                    OfferSalary = model.OfferSalary != null
                                    ? cryptoService1.RsaEncrypt(model.OfferSalary) : null,
                    FinalComment = string.IsNullOrWhiteSpace(model.FinalComment) ? string.Empty : cryptoService1.RsaEncrypt(model.FinalComment),
                    CreatedBy = CurrentUser.Id
                };

                CommandResult result = await baseService.ExecuteNonQueryAsync(command);

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
                model.ManagementGradeList = await promotionService.GetManagementGrade();
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
                applicant.ManagementGradeList = await promotionService.GetManagementGrade();
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
                var command = new UpdateApplicantCommand
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
                    ManagementGrade = model.ManagementGrade,
                    Note = model.Note,
                    InterviewerIds = model.InterviewerIds != null ? string.Join(",", model.InterviewerIds) : null,
                    ExpectedSalary = model.ExpectedSalary != null ? cryptoService1.RsaEncrypt(model.ExpectedSalary) : null,
                    OfferSalary = model.OfferSalary != null ? cryptoService1.RsaEncrypt(model.OfferSalary) : null,
                    FinalComment = string.IsNullOrWhiteSpace(model.FinalComment) ? string.Empty : cryptoService1.RsaEncrypt(model.FinalComment)
                };

                CommandResult result = await baseService.ExecuteNonQueryAsync(command);

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
                model.ManagementGradeList = await promotionService.GetManagementGrade();
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
                var newFileName = $"{baseFileName}({DateTimeOffset.Now.DateTime.ToClientTimeZone().ToString("yyyy-MM-dd-HHmmss")}){fileExtension}";

                return newFileName;
            }

            return fileName;
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> UpdateOnTop(int id)
        {
            var command = new UpdateOnTopApplicantCommand
            {
                Id = id
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Delete(int id, string fileName)
        {
            var command = new DeleteApplicantCommand { Id = id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(command);

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