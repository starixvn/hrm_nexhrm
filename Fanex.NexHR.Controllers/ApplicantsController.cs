namespace Fanex.NexHR.Controllers
{
    using Core;
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

    public class ApplicantsController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IWebHostEnvironment environment;
        private readonly AppSetting appSetting;
        private readonly IApplicantService applicantService;
        private readonly IPositionService positionService;
        private readonly IEmployeeService employeeService;

        public ApplicantsController(ISessionService sessionService,
            IBaseService baseService,
            IWebHostEnvironment environment,
            AppSetting appSetting,
            IApplicantService applicantService,
            IPositionService positionService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.baseService = baseService;
            this.environment = environment;
            this.appSetting = appSetting;
            this.applicantService = applicantService;
            this.positionService = positionService;
            this.employeeService = employeeService;
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Index()
        {
            var model = new ApplicantViewModel
            {
                Positions = await positionService.GetAll(),
                StatusList = GetStatusList()
            };

            return View(model);
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

            var model = new CreateApplicantViewModel
            {
                Positions = await positionService.GetAll(),
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
                        Resume = model.Resume,
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
                        InterviewerIds = model.InterviewerIds,
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

                model.Positions = await positionService.GetAll();
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

            CreateApplicantViewModel applicant = await applicantService.GetById(criteria);
            if (applicant != null)
            {
                applicant.Positions = await positionService.GetAll();
                applicant.StatusList = GetStatusList();
                applicant.Recruiter = applicant.NickNameRecruiter + (string.IsNullOrWhiteSpace(applicant.FullNameRecruiter) ? string.Empty : " (" + applicant.FullNameRecruiter + ")");
            }


            return View(applicant);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(CreateApplicantViewModel model)
        {
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    var criteria = new UpdateApplicantCriteria
                    {
                        Id =  model.Id,
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
                        Resume = model.Resume,
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
                        InterviewerIds = model.Interviewer,
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

                model.Positions = await positionService.GetAll();
                model.StatusList = GetStatusList();
            }

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteApplicantCriteria = new DeleteApplicantCriteria { Id = id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(deleteApplicantCriteria);

            return Json(result);
        }
    }
}