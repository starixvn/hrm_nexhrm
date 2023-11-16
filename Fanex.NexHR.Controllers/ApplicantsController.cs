namespace Fanex.NexHR.Controllers
{
    using Core;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Services.Interfaces;
    using Fanex.NexHR.Controllers.Attributes;
    using ViewModels.Applicant;
    using Models.Applicant;
    using Criteria.Applicant;

    public class ApplicantsController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IWebHostEnvironment environment;
        private readonly AppSetting appSetting;
        private readonly IApplicantService applicantService;

        public ApplicantsController(ISessionService sessionService,
            IBaseService baseService,
            IWebHostEnvironment environment,
            AppSetting appSetting,
            IApplicantService applicantService) : base(sessionService)
        {
            this.baseService = baseService;
            this.environment = environment;
            this.appSetting = appSetting;
            this.applicantService = applicantService;
        }

         [HrmAuthorize(UserRoles = "HR")]
        public IActionResult Index()
        {
            var model = new ApplicantViewModel();
            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
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
                    FullName = model.FullName,
                };

                result = await applicantService.GetAll(criteria);
            }

            return PartialView("_ListApplicant", result);
        }
    }
}