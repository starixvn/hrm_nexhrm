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

    public class ApplicantsController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IWebHostEnvironment environment;
        private readonly AppSetting appSetting;
        private readonly IApplicantService applicantService;
        private readonly IPositionService positionService;

        public ApplicantsController(ISessionService sessionService,
            IBaseService baseService,
            IWebHostEnvironment environment,
            AppSetting appSetting,
            IApplicantService applicantService,
            IPositionService positionService) : base(sessionService)
        {
            this.baseService = baseService;
            this.environment = environment;
            this.appSetting = appSetting;
            this.applicantService = applicantService;
            this.positionService = positionService;
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
            IList<Applicant> result = new List<Applicant>();
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
                    applicant.SourceName = GetCVSourceName(applicant.Source); 
                }
            }

            return PartialView("_ListApplicant", result);
        }

        private static string GetCVSourceName(int sourceId)
        {
            CVSource source = (CVSource)sourceId;
            return source.ToString();
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create()
        {
            var model = new CreateApplicantViewModel
            {
                Positions = await positionService.GetAll(),
                StatusList = GetStatusList()
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
                        Recruiter = model.Recruiter,
                        EducationBackground = model.EducationBackground,
                        InterviewerComment = model.InterviewerComment,
                        Grade = model.Grade,
                        Note = model.Note,
                        InterviewerId = model.InterviewerId,
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